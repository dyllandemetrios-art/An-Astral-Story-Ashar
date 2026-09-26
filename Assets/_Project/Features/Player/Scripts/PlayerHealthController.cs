using Ashar.Combat;
using Ashar.Core;
using UnityEngine;

namespace Ashar.Player
{
    /// <summary>
    /// Detects the hits on the player ship and announces them.
    /// RESPONSIBILITIES: keep the hitbox at the size given by PlayerShipData, show or hide the silver hitbox dot,
    /// and turn an enemy bullet touching the hitbox into a GameEvents.OnPlayerHit event.
    /// HOW IT WORKS: the hitbox is a trigger capsule on its own child object, on the PlayerHitbox layer. The
    /// collision matrix lets it meet only enemy bullets and enemies. The ship root has a kinematic Rigidbody2D,
    /// which Unity needs to deliver trigger contacts, and a PlayerTriggerRelay on the hitbox tells this script when
    /// something enters it (the graze zone has its own relay, so the two are never mixed up). A bullet that touches the hitbox is
    /// destroyed and the event is raised, unless the ship is invulnerable (dash): then the bullet flies on.
    /// WHY: only the tiny hitbox counts, not the whole sprite, so a bullet may graze the wing without hitting (the
    /// genre's convention, which makes dense bullet patterns fair). Lives and respawn come with later stories.
    /// </summary>
    public class PlayerHealthController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField, Tooltip("Tuning values of the ship: hitbox size and whether its dot is shown.")]
        private PlayerShipData _shipData;

        [Header("References")]
        [SerializeField, Tooltip("Trigger capsule of the hitbox, on the PlayerHitbox layer (a child of the ship).")]
        private CapsuleCollider2D _hitbox;

        [SerializeField, Tooltip("Relay on the hitbox object: tells this script when something enters the hitbox.")]
        private PlayerTriggerRelay _hitboxRelay;

        [SerializeField, Tooltip("Silver dot that marks the hitbox. Shown or hidden according to the ship data.")]
        private GameObject _hitboxDot;

        [SerializeField, Tooltip("Optional. While the dash is invulnerable, hits are ignored.")]
        private PlayerDashController _dash;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: number of hits that counted since the scene started.")]
        private int _hitCount;

        /// <summary>Checks that the required references are set, and disables the component if one is missing.</summary>
        private void Awake()
        {
            if (_shipData == null || _hitbox == null || _hitboxRelay == null)
            {
                Debug.LogError($"{nameof(PlayerHealthController)} on '{name}' is missing its ship data, hitbox or hitbox relay. Hit detection disabled.", this);
                enabled = false;
            }
        }

        /// <summary>Starts listening to the hitbox contacts.</summary>
        private void OnEnable()
        {
            if (_hitboxRelay != null)
            {
                _hitboxRelay.Entered += HandleHitboxEntered;
            }
        }

        /// <summary>Stops listening to the hitbox contacts.</summary>
        private void OnDisable()
        {
            if (_hitboxRelay != null)
            {
                _hitboxRelay.Entered -= HandleHitboxEntered;
            }
        }

        /// <summary>Applies the hitbox size and the dot visibility from the ship data, so they can be tuned live.</summary>
        private void Update()
        {
            Vector2 size = ComputeHitboxSize(_shipData.HitboxRadius, _shipData.HitboxHeight);
            if (_hitbox.size != size)
            {
                _hitbox.size = size;
            }

            if (_hitboxDot != null && _hitboxDot.activeSelf != _shipData.ShowHitbox)
            {
                _hitboxDot.SetActive(_shipData.ShowHitbox);
            }
        }

        /// <summary>Called when another collider enters the hitbox.</summary>
        private void HandleHitboxEntered(Collider2D other)
        {
            int layer = other.gameObject.layer;
            if (layer != Layers.EnemyBullet && layer != Layers.Enemy)
            {
                return; // Power-ups have their own story.
            }

            bool invulnerable = _dash != null && _dash.IsInvulnerable;
            if (!ShouldRegisterHit(invulnerable))
            {
                return; // The bullet passes through a dashing ship.
            }

            _hitCount++;
            GameEvents.RaisePlayerHit();

            // A bullet that hits is an impact, never also a graze, even if the graze zone sees it in the same frame.
            ProjectileController projectile = other.GetComponentInParent<ProjectileController>();
            if (projectile != null)
            {
                projectile.MarkGrazed();
            }

            // A bullet is spent by the hit: destroy its whole object (the collider may sit on a child). An enemy that
            // rams the ship is not destroyed by it: it is the player who takes the hit.
            if (layer == Layers.EnemyBullet)
            {
                GameObject bulletObject = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
                Destroy(bulletObject);
            }
        }

        /// <summary>
        /// Returns the size of the hitbox capsule: twice the radius wide, and as tall as asked but never shorter than
        /// it is wide (a capsule cannot be shorter than its own rounded ends). Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static Vector2 ComputeHitboxSize(float radius, float height)
        {
            float width = 2f * radius;
            return new Vector2(width, Mathf.Max(height, width));
        }

        /// <summary>
        /// True when a bullet that reaches the hitbox counts as a hit: not while the ship is invulnerable.
        /// Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static bool ShouldRegisterHit(bool isInvulnerable)
        {
            return !isInvulnerable;
        }
    }
}
