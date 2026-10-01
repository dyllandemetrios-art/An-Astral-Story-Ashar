using Ashar.Combat;
using UnityEngine;

namespace Ashar.Enemies
{
    /// <summary>The four ways an enemy can fire (spec §7.4).</summary>
    public enum FirePatternType
    {
        /// <summary>Bullets sent straight at the player's position at the moment of firing.</summary>
        Aimed,

        /// <summary>Several bullets spread over an angle, centred on the player or straight down.</summary>
        Fan,

        /// <summary>One bullet per shot, each turned a little further than the last, drawing a spiral.</summary>
        Spiral,

        /// <summary>Several aimed shots one after the other, a short time apart.</summary>
        Burst,
    }

    /// <summary>
    /// How an enemy fires: the type of pattern, how many bullets, how often, how fast and with which bullet.
    /// RESPONSIBILITIES: hold the numbers of a firing pattern so that a new pattern is a new asset, not new code.
    /// HOW IT WORKS: this is a ScriptableObject asset (FirePattern_...). An EnemyData points to one; EnemyShootController
    /// reads it and fires accordingly. Several enemies can share the same pattern asset.
    /// PATTERN: data-driven design.
    /// </summary>
    [CreateAssetMenu(fileName = "FirePattern_New", menuName = "Ashar/Fire Pattern Data")]
    public class FirePatternData : ScriptableObject
    {
        [Header("Pattern")]
        [SerializeField, Tooltip("Which pattern to fire: Aimed, Fan, Spiral or Burst.")]
        private FirePatternType _type = FirePatternType.Aimed;

        [SerializeField, Min(1), Tooltip("Bullets per shot: for Aimed the bullets fired together, for Fan the bullets in the fan, for Burst the shots in the burst. Spiral fires one per shot and ignores it.")]
        private int _count = 1;

        [SerializeField, Range(0f, 360f), Tooltip("Fan only: the angle covered by the whole fan, in degrees.")]
        private float _spreadAngle = 30f;

        [SerializeField, Tooltip("Fan only: aim the middle of the fan at the player. Off = the middle of the fan points straight down.")]
        private bool _aimAtPlayer = true;

        [SerializeField, Tooltip("Spiral only: how many degrees each shot is turned compared with the previous one.")]
        private float _spiralAngleStep = 15f;

        [Header("Timing")]
        [SerializeField, Min(0.05f), Tooltip("Seconds between one shot (or burst) and the next. It includes the warning time.")]
        private float _interval = 2f;

        [SerializeField, Min(0.01f), Tooltip("Burst only: seconds between two shots of the burst (spec §7.4: 0.1).")]
        private float _burstSpacing = 0.1f;

        [SerializeField, Min(0f), Tooltip("Seconds of warning line before the shot. 0 = no warning.")]
        private float _telegraphTime = 0f;

        [Header("Bullet")]
        [SerializeField, Min(0.1f), Tooltip("Bullet speed in world units per second, before the global bullet speed multiplier (spec §4: 5.6 standard, 7.9 fast).")]
        private float _bulletSpeed = 5.6f;

        [SerializeField, Tooltip("Bullet to fire. It must have a ProjectileController.")]
        private ProjectileController _bulletPrefab;

        [Header("Pulse response (spec E5-06)")]
        [SerializeField, Tooltip("Whether the player's Pulse erases this pattern's bullets when they are in range. Never applies to the player's own bullets.")]
        private bool _pulseErasable;

        /// <summary>Which pattern to fire.</summary>
        public FirePatternType Type => _type;

        /// <summary>Bullets per shot (see the tooltip: it depends on the type).</summary>
        public int Count => _count;

        /// <summary>Fan only: the angle covered by the whole fan, in degrees.</summary>
        public float SpreadAngle => _spreadAngle;

        /// <summary>Fan only: true if the middle of the fan points at the player.</summary>
        public bool AimAtPlayer => _aimAtPlayer;

        /// <summary>Spiral only: degrees each shot is turned compared with the previous one.</summary>
        public float SpiralAngleStep => _spiralAngleStep;

        /// <summary>Seconds between one shot (or burst) and the next.</summary>
        public float Interval => _interval;

        /// <summary>Burst only: seconds between two shots of the burst.</summary>
        public float BurstSpacing => _burstSpacing;

        /// <summary>Seconds of warning line before the shot.</summary>
        public float TelegraphTime => _telegraphTime;

        /// <summary>Bullet speed in world units per second.</summary>
        public float BulletSpeed => _bulletSpeed;

        /// <summary>Bullet to fire.</summary>
        public ProjectileController BulletPrefab => _bulletPrefab;

        /// <summary>Whether the player's Pulse erases this pattern's bullets when they are in range.</summary>
        public bool PulseErasable => _pulseErasable;
    }
}
