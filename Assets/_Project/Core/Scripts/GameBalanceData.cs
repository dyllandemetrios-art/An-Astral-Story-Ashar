using UnityEngine;

namespace Ashar.Core
{
    /// <summary>
    /// The global tuning values of the game: enemy hit points, fire density, bullet speed, the bullet limit, lives, continues
    /// and the protection after a respawn.
    /// RESPONSIBILITIES: hold the numbers that shape the difficulty of the whole game in one asset, so they can be tuned in
    /// one place at playtests without touching code (spec §6 and §7.9). There is a single asset, GameBalanceData_Default.
    /// HOW IT WORKS: this is a ScriptableObject. GameSessionController copies its values into the running GameSession every
    /// frame, so they can be tuned live during a playtest. Enemies read them when they are created and when they fire.
    /// PATTERN: data-driven design.
    /// </summary>
    [CreateAssetMenu(fileName = "GameBalanceData_Default", menuName = "Ashar/Game Balance Data")]
    public class GameBalanceData : ScriptableObject
    {
        [Header("Enemies")]
        [SerializeField, Min(0.1f), Tooltip("Multiplier of the hit points of every enemy (spec §7.9: 1). Applied when an enemy is created.")]
        private float _enemyHpMultiplier = 1f;

        [SerializeField, Min(0.1f), Tooltip("Fire density: how many bullets enemies fire compared with their pattern data (spec §7.9: 0.8, a little lighter than the design). The firing interval is divided by it, so 0.8 means shots 25 % further apart.")]
        private float _fireDensityMultiplier = 0.8f;

        [SerializeField, Min(0.1f), Tooltip("Multiplier of the speed of every enemy bullet (spec §7.9: 1).")]
        private float _bulletSpeedMultiplier = 1f;

        [SerializeField, Min(0), Tooltip("Most bullets alive at once. Enemies stop firing when it is reached. 0 = no limit. Decided by the measure of story E2-01: 2000 costs 1.4 ms per frame on the development machine; the second machine may ask for less.")]
        private int _maxProjectiles = 2000;

        [Header("Lives")]
        [SerializeField, Min(1), Tooltip("Lives at the start of a game and after a continue (spec §7.9: 3).")]
        private int _startLives = 3;

        [SerializeField, Min(-1), Tooltip("How many continues the player has after a game over. -1 = unlimited (spec §7.9). A continue puts the lives back to their start value and the score back to 0.")]
        private int _continues = -1;

        [SerializeField, Min(0f), Tooltip("Seconds during which the ship cannot be hit after it reappears (spec §7.9: 2).")]
        private float _respawnInvulnTime = 2f;

        /// <summary>Multiplier of the hit points of every enemy.</summary>
        public float EnemyHpMultiplier => _enemyHpMultiplier;

        /// <summary>Fire density multiplier: the firing interval is divided by it.</summary>
        public float FireDensityMultiplier => _fireDensityMultiplier;

        /// <summary>Multiplier of the speed of every enemy bullet.</summary>
        public float BulletSpeedMultiplier => _bulletSpeedMultiplier;

        /// <summary>Most bullets alive at once (0 = no limit).</summary>
        public int MaxProjectiles => _maxProjectiles;

        /// <summary>Lives at the start of a game and after a continue.</summary>
        public int StartLives => _startLives;

        /// <summary>Number of continues, or -1 for unlimited.</summary>
        public int Continues => _continues;

        /// <summary>Seconds of protection after a respawn.</summary>
        public float RespawnInvulnTime => _respawnInvulnTime;
    }
}
