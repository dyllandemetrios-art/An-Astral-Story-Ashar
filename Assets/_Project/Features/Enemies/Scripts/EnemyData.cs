using UnityEngine;

namespace Ashar.Enemies
{
    /// <summary>The ways an enemy can move across the screen (spec §6).</summary>
    public enum EnemyMovementType
    {
        /// <summary>Straight line down the screen.</summary>
        Straight,

        /// <summary>Down the screen with a sideways swing (a sine wave).</summary>
        Sine,

        /// <summary>Fast straight line towards the player's position at the moment the enemy appears.</summary>
        Dive,

        /// <summary>Across the screen from one side to the other, with an optional up-and-down swing.</summary>
        LateralSweep,

        /// <summary>Circles around the point where it appeared.</summary>
        Orbit,

        /// <summary>Stays where it appeared.</summary>
        Static,
    }

    /// <summary>
    /// Everything that defines one kind of enemy: its toughness, reward, look and way of moving.
    /// RESPONSIBILITIES: hold the numbers of an enemy type so that a new enemy is a new asset, not new code.
    /// HOW IT WORKS: this is a ScriptableObject asset (EnemyData_GuardDrone...). The wave system reads it to know what to
    /// create, and EnemyController reads it to know how to behave. The look is the prefab it points to.
    /// PATTERN: data-driven design. Pulse response and power-up drops are added by the stories that use them.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData_New", menuName = "Ashar/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField, Tooltip("Short unique name of the enemy, used in wave tables and logs (for example guard_drone).")]
        private string _id = "enemy";

        [SerializeField, Tooltip("Key of the display name in the text table. The visible name never lives in code or in this asset.")]
        private string _displayNameKey = "enemy.name";

        [SerializeField, Tooltip("Prefab to create for this enemy. It must have an EnemyController; its Visual child holds the sprite.")]
        private EnemyController _prefab;

        [Header("Durability and reward")]
        [SerializeField, Min(1f), Tooltip("Hit points before the global enemy HP multiplier. The player deals 100 damage per second, so 50 HP is half a second of fire (spec §7.3).")]
        private float _maxHp = 50f;

        [SerializeField, Min(0), Tooltip("Score points earned when the enemy is destroyed by the player.")]
        private int _score = 100;

        [Header("Movement")]
        [SerializeField, Tooltip("How the enemy moves.")]
        private EnemyMovementType _movement = EnemyMovementType.Straight;

        [SerializeField, Min(0f), Tooltip("Speed in world units per second (for Orbit, the speed along the circle).")]
        private float _speed = 4.5f;

        [SerializeField, Min(0f), Tooltip("Swing distance in world units: sideways for Sine, vertical for LateralSweep, radius of the circle for Orbit.")]
        private float _amplitude = 0f;

        [SerializeField, Min(0f), Tooltip("Swings per second, for Sine and LateralSweep.")]
        private float _frequency = 0.5f;

        [Header("Weapon")]
        [SerializeField, Tooltip("How the enemy fires. Leave empty for an enemy that never fires.")]
        private FirePatternData _firePattern;

        /// <summary>Short unique name of the enemy.</summary>
        public string Id => _id;

        /// <summary>Key of the display name in the text table.</summary>
        public string DisplayNameKey => _displayNameKey;

        /// <summary>Prefab to create for this enemy.</summary>
        public EnemyController Prefab => _prefab;

        /// <summary>Hit points before the global multiplier.</summary>
        public float MaxHp => _maxHp;

        /// <summary>Score points earned when the enemy is destroyed.</summary>
        public int Score => _score;

        /// <summary>How the enemy moves.</summary>
        public EnemyMovementType Movement => _movement;

        /// <summary>Speed in world units per second.</summary>
        public float Speed => _speed;

        /// <summary>How the enemy fires, or null if it never fires.</summary>
        public FirePatternData FirePattern => _firePattern;

        /// <summary>Swing distance or circle radius, in world units.</summary>
        public float Amplitude => _amplitude;

        /// <summary>Swings per second.</summary>
        public float Frequency => _frequency;
    }
}
