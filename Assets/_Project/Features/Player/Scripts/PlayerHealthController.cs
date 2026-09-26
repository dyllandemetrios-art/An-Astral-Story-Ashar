using Ashar.Combat;
using Ashar.Core;
using UnityEngine;

namespace Ashar.Player
{
    /// <summary>
    /// Detects the hits on the player ship and announces them.
    /// RESPONSIBILITIES: keep the hitbox at the size given by PlayerShipData, show or hide the silver hitbox dot,
    /// and turn an enemy bullet touching the hitbox into a GameEvents.OnPlayerHit event.
    /// HOW IT WORKS: the hitbox is a small trigger circle on its own child object, on the PlayerHitbox layer. The
    /// collision matrix lets it meet only enemy bullets and enemies. The ship root has a kinematic Rigidbody2D,
    /// which is what makes Unity deliver the trigger message to this script. A bullet that touches the hitbox is
    /// destroyed and the event is raised, unless the ship is invulnerable (dash): then the bullet flies on.
    /// WHY: only the tiny hitbox counts, not the whole sprite, so a bullet may graze the wing without hitting (the
    /// genre's convention, which makes dense bullet patterns fair). Lives and respawn come with later stories.
    /// </summary>
    public class PlayerHealthController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField, Tooltip("Tuning values of the ship: hitbox radius and whether its dot is shown.")]
        private PlayerShipData _shipData;

        [Header("References")]
        [SerializeField, Tooltip("Trigger circle of the hitbox, on the PlayerHitbox layer (a child of the ship).")]
        private CircleCollider2D _hitbox;

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
            if (_shipData == null || _hitbox == null)
            {
                Debug.LogError($"{nameof(PlayerHealthController)} on '{name}' is missing its ship data or hitbox. Hit detection disabled.", this);
                enabled = false;
            }
        }

        /// <summary>Applies the hitbox size and the dot visibility from the ship data, so they can be tuned live.</summary>
        private void Update()
        {
            if (!Mathf.Approximately(_hitbox.radius, _shipData.HitboxRadius))
            {
                _hitbox.radius = _shipData.HitboxRadius;
            }

            if (_hitboxDot != null && _hitboxDot.activeSelf != _shipData.ShowHitbox)
            {
                _hitboxDot.SetActive(_shipData.ShowHitbox);
            }
        }

        /// <summary>Called by Unity when a trigger of this ship (the hitbox) meets another collider.</summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != Layers.EnemyBullet)
            {
                return; // Enemy bodies and power-ups have their own stories.
            }

            bool invulnerable = _dash != null && _dash.IsInvulnerable;
            if (!ShouldRegisterHit(invulnerable))
            {
                return; // The bullet passes through a dashing ship.
            }

            _hitCount++;
            GameEvents.RaisePlayerHit();

            // The bullet is spent: destroy its whole object (the collider may sit on a child).
            GameObject bulletObject = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
            Destroy(bulletObject);
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
