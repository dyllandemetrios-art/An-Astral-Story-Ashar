using Ashar.Combat;
using Ashar.Core;
using UnityEngine;

namespace Ashar.Player
{
    /// <summary>
    /// Rewards near misses: an enemy bullet that comes close to the ship without touching the hitbox is a graze.
    /// RESPONSIBILITIES: keep the graze zone at the size given by PlayerShipData, and turn a bullet entering it into a
    /// GameEvents.OnPlayerGrazed event carrying the points earned.
    /// HOW IT WORKS: the graze zone is a trigger capsule (the hitbox capsule grown by 0.6 u all round) on its own child object, on the PlayerGraze
    /// layer, which the collision matrix lets meet only enemy bullets. When a bullet enters, it is counted if it
    /// has not been counted before, the ship is not dashing, and it is not already touching the hitbox. The bullet is
    /// then marked, so it can never be counted twice.
    /// WHY: the graze links aggressive flying to the score (spec §7.1). Rules that could double-count are checked in
    /// one static method, ShouldCountGraze, so they can be unit-tested. A bullet that hits is an impact, never a graze.
    /// </summary>
    public class PlayerGrazeController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField, Tooltip("Tuning values of the ship: graze radius and points per graze.")]
        private PlayerShipData _shipData;

        [Header("References")]
        [SerializeField, Tooltip("Trigger capsule of the graze zone, on the PlayerGraze layer (a child of the ship).")]
        private CapsuleCollider2D _grazeZone;

        [SerializeField, Tooltip("Relay on the graze zone object: tells this script when something enters the zone.")]
        private PlayerTriggerRelay _grazeRelay;

        [SerializeField, Tooltip("Trigger collider of the hitbox. A bullet already touching it is an impact, not a graze.")]
        private Collider2D _hitbox;

        [SerializeField, Tooltip("Optional. No graze is counted while the ship is dashing.")]
        private PlayerDashController _dash;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: number of grazes counted since the scene started.")]
        private int _grazeCount;

        [SerializeField, Tooltip("Read-only: total score points earned by grazes since the scene started.")]
        private int _grazePoints;

        /// <summary>Checks that every reference is set, and disables the component if one is missing.</summary>
        private void Awake()
        {
            if (_shipData == null || _grazeZone == null || _grazeRelay == null || _hitbox == null)
            {
                Debug.LogError($"{nameof(PlayerGrazeController)} on '{name}' is missing its ship data, graze zone, relay or hitbox. Graze disabled.", this);
                enabled = false;
            }
        }

        /// <summary>Starts listening to the graze zone contacts.</summary>
        private void OnEnable()
        {
            if (_grazeRelay != null)
            {
                _grazeRelay.Entered += HandleGrazeEntered;
            }
        }

        /// <summary>Stops listening to the graze zone contacts.</summary>
        private void OnDisable()
        {
            if (_grazeRelay != null)
            {
                _grazeRelay.Entered -= HandleGrazeEntered;
            }
        }

        /// <summary>Applies the graze zone size from the ship data, so it can be tuned live.</summary>
        private void Update()
        {
            Vector2 size = ComputeGrazeSize(_shipData.GrazeRadius, _shipData.HitboxRadius, _shipData.HitboxHeight);
            if (_grazeZone.size != size)
            {
                _grazeZone.size = size;
            }
        }

        /// <summary>Called when another collider enters the graze zone: counts a graze if the rules allow it.</summary>
        private void HandleGrazeEntered(Collider2D other)
        {
            if (other.gameObject.layer != Layers.EnemyBullet)
            {
                return;
            }

            ProjectileController bullet = other.GetComponentInParent<ProjectileController>();
            if (bullet == null)
            {
                return;
            }

            bool dashing = _dash != null && _dash.IsDashing;
            bool touchingHitbox = _hitbox.IsTouching(other);
            if (!ShouldCountGraze(bullet.HasBeenGrazed, dashing, touchingHitbox))
            {
                return;
            }

            bullet.MarkGrazed();
            _grazeCount++;
            _grazePoints += _shipData.GrazeScore;
            GameEvents.RaisePlayerGrazed(_shipData.GrazeScore);
        }

        /// <summary>
        /// Returns the size of the graze capsule: the hitbox capsule grown by the graze margin on every side. The margin
        /// is the graze radius minus the hitbox radius, so the zone is 2 x grazeRadius wide. Static and free of Unity state
        /// so it can be unit-tested.
        /// </summary>
        public static Vector2 ComputeGrazeSize(float grazeRadius, float hitboxRadius, float hitboxHeight)
        {
            Vector2 hitbox = PlayerHealthController.ComputeHitboxSize(hitboxRadius, hitboxHeight);
            float margin = Mathf.Max(0f, grazeRadius - hitboxRadius);
            return new Vector2(hitbox.x + 2f * margin, hitbox.y + 2f * margin);
        }

        /// <summary>
        /// True when a bullet that enters the graze zone counts as a graze: it has not been counted yet, the ship is
        /// not dashing, and the bullet is not already on the hitbox (then it is an impact, which wins).
        /// Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static bool ShouldCountGraze(bool alreadyCounted, bool isDashing, bool touchingHitbox)
        {
            return !alreadyCounted && !isDashing && !touchingHitbox;
        }
    }
}
