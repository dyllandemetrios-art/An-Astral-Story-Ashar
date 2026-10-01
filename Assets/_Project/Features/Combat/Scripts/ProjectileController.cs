using UnityEngine;

namespace Ashar.Combat
{
    /// <summary>
    /// A projectile that flies in a straight line and removes itself when it has left the screen.
    /// RESPONSIBILITIES: move at a constant speed along a direction, carry its damage, and destroy itself out of range.
    /// HOW IT WORKS: whoever fires the projectile calls Initialize() right after Instantiate(), giving the direction,
    /// speed, damage and the rectangle in which the projectile may live. Every frame it moves; once its position is
    /// outside that rectangle, it is destroyed, so no projectile lives on off screen.
    /// WHY: the prefab never looks for the screen itself (no scene lookups), so the same projectile works for the
    /// player and, later, for enemies. Instantiate/Destroy is enough for the prototype; pooling waits for the E2-01 measure.
    /// </summary>
    public class ProjectileController : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: damage this projectile deals on impact.")]
        private float _damage;

        private Vector2 _direction = Vector2.up; // Unit vector along which the projectile flies.
        private float _speed;                    // World units per second.
        private Rect _lifeBounds;                // The projectile is destroyed when it leaves this rectangle.
        private bool _initialized;               // False until Initialize() has been called.
        private bool _hasBeenGrazed;             // True once counted as a graze or a hit: a projectile never counts twice.
        private bool _pulseErasable;              // True when the player's Pulse (spec E5-06) may destroy this projectile in range.

        /// <summary>Damage this projectile deals on impact.</summary>
        public float Damage => _damage;

        /// <summary>True once this projectile has been counted as a graze or a hit.</summary>
        public bool HasBeenGrazed => _hasBeenGrazed;

        /// <summary>True when the player's Pulse may destroy this projectile when it is in range.</summary>
        public bool PulseErasable => _pulseErasable;

        /// <summary>Marks the projectile as counted, so it can never be counted a second time.</summary>
        public void MarkGrazed()
        {
            _hasBeenGrazed = true;
        }

        /// <summary>
        /// Sets how the projectile flies. Must be called once, right after the projectile is created. pulseErasable
        /// defaults to false: only an enemy pattern explicitly marked erasable (spec E5-06) passes true; the player's
        /// own bullets never do.
        /// </summary>
        public void Initialize(Vector2 direction, float speed, float damage, Rect lifeBounds, bool pulseErasable = false)
        {
            _direction = direction.normalized;
            _speed = speed;
            _damage = damage;
            _lifeBounds = lifeBounds;
            _pulseErasable = pulseErasable;
            _initialized = true;

            // The sprite art points up, so turning the object makes it face its flight direction.
            transform.up = _direction;
        }

        /// <summary>Moves the projectile and destroys it once it is outside its life bounds.</summary>
        private void Update()
        {
            if (!_initialized)
            {
                return;
            }

            Vector2 next = ComputeNextPosition(transform.position, _direction, _speed, Time.deltaTime);
            transform.position = new Vector3(next.x, next.y, transform.position.z);

            if (IsOutside(_lifeBounds, next))
            {
                Destroy(gameObject);
            }
        }

        /// <summary>Returns where a projectile is after one frame of straight flight.</summary>
        public static Vector2 ComputeNextPosition(Vector2 position, Vector2 direction, float speed, float deltaTime)
        {
            return position + direction * (speed * deltaTime);
        }

        /// <summary>True when the position is outside the rectangle (the projectile has left the play space).</summary>
        public static bool IsOutside(Rect bounds, Vector2 position)
        {
            return !bounds.Contains(position);
        }
    }
}
