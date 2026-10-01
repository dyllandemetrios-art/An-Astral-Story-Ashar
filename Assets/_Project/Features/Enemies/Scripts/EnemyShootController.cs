using Ashar.Combat;
using Ashar.Core;
using UnityEngine;

namespace Ashar.Enemies
{
    /// <summary>
    /// Makes an enemy fire the pattern of its FirePatternData: aimed, fan, spiral or burst, with an optional warning line.
    /// RESPONSIBILITIES: run the firing clock, work out the direction of each bullet, create the bullets under
    /// Runtime/Projectiles, and show the warning line before a shot.
    /// HOW IT WORKS: EnemyController calls Initialize() when the enemy appears. Each frame the FireSequenceTimer says
    /// "warn" or "fire"; on "fire" the pattern type decides how many bullets go where (a switch over the four types,
    /// spec §7.4). The bullets are ordinary ProjectileControllers on the EnemyBullet layer.
    /// WHY: one component reads the pattern data, so a new pattern is a new asset. There is deliberately no system of
    /// composable patterns: the boss will ask for one only if it needs it (spec §7.4).
    /// </summary>
    public class EnemyShootController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Where the bullets appear. Leave empty to fire from the centre of the enemy.")]
        private Transform _muzzle;

        [SerializeField, Tooltip("Optional line shown before a shot when the pattern has a warning time.")]
        private LineRenderer _telegraphLine;

        [Header("Warning line")]
        [SerializeField, Min(0.1f), Tooltip("Length of the warning line, in world units. It should reach across the screen.")]
        private float _telegraphLength = 12f;

        [SerializeField, Tooltip("Colour of the warning line at its faintest and at its brightest (just before the shot).")]
        private Gradient _telegraphColors;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: bullets fired by this enemy so far.")]
        private int _firedCount;

        private FirePatternData _pattern;      // What to fire.
        private EnemySpawnContext _context;    // Where the player and the projectile holder are.
        private FireSequenceTimer _timer;      // When to warn and fire.
        private int _spiralShot;               // Number of the next spiral shot, which sets its angle.
        private bool _initialized;             // False until Initialize() has been called.

        /// <summary>Sets what to fire. Called by EnemyController when the enemy appears.</summary>
        public void Initialize(FirePatternData pattern, EnemySpawnContext context)
        {
            _pattern = pattern;
            _context = context;
            int shots = pattern.Type == FirePatternType.Burst ? pattern.Count : 1;
            // The global fire density stretches or shrinks the interval (spec §7.9), from the moment the enemy appears.
            float density = GameSession.Current != null ? GameSession.Current.FireDensityMultiplier : 1f;
            float interval = BalanceMath.ScaledFireInterval(pattern.Interval, density);
            _timer = new FireSequenceTimer(interval, pattern.TelegraphTime, shots, pattern.BurstSpacing);
            // Start part-way through the wait, so a group of enemies created together does not fire all at the same moment.
            _timer.Restart(Random.Range(0f, interval * 0.5f));
            SetTelegraph(0f, false);
            _initialized = true;
        }

        /// <summary>Runs the clock and reacts to its events.</summary>
        private void Update()
        {
            if (!_initialized)
            {
                return;
            }

            FireSequenceEvent fireEvent = _timer.Advance(Time.deltaTime);
            if (fireEvent == FireSequenceEvent.Fire)
            {
                SetTelegraph(0f, false);
                FireShot();
            }
            else if (_timer.IsTelegraphing)
            {
                SetTelegraph(_timer.TelegraphProgress, true);
            }
        }

        /// <summary>Fires one shot of the pattern: creates its bullet or bullets in the right directions.</summary>
        private void FireShot()
        {
            Vector2 origin = _muzzle != null ? (Vector2)_muzzle.position : (Vector2)transform.position;
            Vector2 toTarget = DirectionToTarget(origin);

            switch (_pattern.Type)
            {
                case FirePatternType.Aimed:
                    for (int i = 0; i < _pattern.Count; i++)
                    {
                        Spawn(origin, toTarget);
                    }

                    break;

                case FirePatternType.Fan:
                    Vector2 middle = _pattern.AimAtPlayer ? toTarget : Vector2.down;
                    for (int i = 0; i < _pattern.Count; i++)
                    {
                        Spawn(origin, Rotate(middle, FanOffsetDegrees(i, _pattern.Count, _pattern.SpreadAngle)));
                    }

                    break;

                case FirePatternType.Spiral:
                    Spawn(origin, Rotate(Vector2.down, SpiralAngleDegrees(_spiralShot, _pattern.SpiralAngleStep)));
                    _spiralShot++;
                    break;

                default: // Burst: each call of this method is one shot of the burst, aimed at where the player is now.
                    Spawn(origin, toTarget);
                    break;
            }
        }

        /// <summary>Creates one bullet at the origin, flying in the given direction.</summary>
        private void Spawn(Vector2 origin, Vector2 direction)
        {
            GameSession session = GameSession.Current;
            int alive = _context.ProjectileParent != null ? _context.ProjectileParent.childCount : 0;
            if (session != null && !BalanceMath.CanCreateProjectile(alive, session.MaxProjectiles))
            {
                return; // The limit of bullets alive at once is reached: this shot is skipped, the frame rate stays safe.
            }

            float speedMultiplier = session != null ? session.BulletSpeedMultiplier : 1f;
            ProjectileController bullet = Instantiate(_pattern.BulletPrefab, origin, Quaternion.identity, _context.ProjectileParent);
            bullet.Initialize(direction, BalanceMath.ScaledBulletSpeed(_pattern.BulletSpeed, speedMultiplier), 1f, _context.ProjectileBounds, _pattern.PulseErasable);
            _firedCount++;
        }

        /// <summary>The direction from a point to the player, or straight down when there is no player to aim at.</summary>
        private Vector2 DirectionToTarget(Vector2 origin)
        {
            if (_context.Target == null)
            {
                return Vector2.down;
            }

            Vector2 toTarget = (Vector2)_context.Target.position - origin;
            return toTarget.sqrMagnitude > 0.0001f ? toTarget.normalized : Vector2.down;
        }

        /// <summary>Shows or hides the warning line, brighter as the shot gets closer.</summary>
        private void SetTelegraph(float progress, bool visible)
        {
            if (_telegraphLine == null)
            {
                return;
            }

            _telegraphLine.enabled = visible;
            if (!visible)
            {
                return;
            }

            Vector2 origin = _muzzle != null ? (Vector2)_muzzle.position : (Vector2)transform.position;
            Vector2 direction = _pattern.Type == FirePatternType.Spiral
                ? Rotate(Vector2.down, SpiralAngleDegrees(_spiralShot, _pattern.SpiralAngleStep))
                : DirectionToTarget(origin);
            _telegraphLine.positionCount = 2;
            _telegraphLine.SetPosition(0, origin);
            _telegraphLine.SetPosition(1, origin + direction * _telegraphLength);

            if (_telegraphColors != null)
            {
                Color color = _telegraphColors.Evaluate(progress);
                _telegraphLine.startColor = color;
                _telegraphLine.endColor = new Color(color.r, color.g, color.b, 0f);
            }
        }

        /// <summary>
        /// Returns the angle, in degrees, of one bullet of a fan compared with the middle of the fan: the first bullet is
        /// at minus half the spread, the last at plus half, the others evenly between. A fan of one bullet points at the
        /// middle. Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static float FanOffsetDegrees(int index, int count, float spreadAngle)
        {
            if (count <= 1)
            {
                return 0f;
            }

            return -spreadAngle * 0.5f + spreadAngle * index / (count - 1);
        }

        /// <summary>Returns the angle, in degrees, of a spiral shot: the first shot goes straight down, each next one is turned further.</summary>
        public static float SpiralAngleDegrees(int shotIndex, float angleStep)
        {
            return shotIndex * angleStep;
        }

        /// <summary>Returns a direction turned by an angle in degrees (positive turns anticlockwise).</summary>
        public static Vector2 Rotate(Vector2 direction, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            return new Vector2(direction.x * cos - direction.y * sin, direction.x * sin + direction.y * cos);
        }
    }
}
