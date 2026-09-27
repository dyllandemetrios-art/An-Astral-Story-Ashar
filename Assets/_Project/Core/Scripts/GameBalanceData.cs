using UnityEngine;

namespace Ashar.Core
{
    /// <summary>
    /// The global tuning values of the game: lives, continues and the protection after a respawn.
    /// RESPONSIBILITIES: hold the numbers that shape the difficulty of the whole game in one asset, so they can be tuned in
    /// one place at playtests without touching code (spec §6 and §7.9). There is a single asset, GameBalanceData_Default.
    /// HOW IT WORKS: this is a ScriptableObject. GameSessionController reads it when a game starts. The enemy and bullet
    /// multipliers are added by the story that applies them (E2-06).
    /// PATTERN: data-driven design.
    /// </summary>
    [CreateAssetMenu(fileName = "GameBalanceData_Default", menuName = "Ashar/Game Balance Data")]
    public class GameBalanceData : ScriptableObject
    {
        [Header("Lives")]
        [SerializeField, Min(1), Tooltip("Lives at the start of a game and after a continue (spec §7.9: 3).")]
        private int _startLives = 3;

        [SerializeField, Min(-1), Tooltip("How many continues the player has after a game over. -1 = unlimited (spec §7.9). A continue puts the lives back to their start value and the score back to 0.")]
        private int _continues = -1;

        [SerializeField, Min(0f), Tooltip("Seconds during which the ship cannot be hit after it reappears (spec §7.9: 2).")]
        private float _respawnInvulnTime = 2f;

        /// <summary>Lives at the start of a game and after a continue.</summary>
        public int StartLives => _startLives;

        /// <summary>Number of continues, or -1 for unlimited.</summary>
        public int Continues => _continues;

        /// <summary>Seconds of protection after a respawn.</summary>
        public float RespawnInvulnTime => _respawnInvulnTime;
    }
}
