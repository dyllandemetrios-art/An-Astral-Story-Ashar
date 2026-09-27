using Ashar.Core;
using UnityEngine;

namespace Ashar.Dialogue
{
    /// <summary>
    /// PROVISIONAL stand-in for a boss (spec E3-02, QA "second cas"): attacks on a timer while in normal play, and
    /// stops the instant a dialogue starts, so the suspend/resume boundary can be checked before any real boss exists.
    /// RESPONSIBILITIES: log a fake attack every few seconds while GameState is Gameplay, and stay silent otherwise.
    /// HOW IT WORKS: it listens to GameEvents.OnGameStateChanged, the same way PlayerRespawnController reacts to
    /// GameOver, instead of being told about dialogues directly: it does not know DialogueController exists. This
    /// script is a throw-away test tool: it lives in the TestBed scene only, and will be deleted once a real boss
    /// (E4/E5/E6) replaces it.
    /// </summary>
    public class DebugTestBossController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Min(0.1f), Tooltip("Seconds between two fake attacks while allowed to attack.")]
        private float _attackInterval = 2f;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: true while GameState allows attacking (Gameplay).")]
        private bool _canAttack = true;

        private float _timer; // Seconds since the last fake attack.

        /// <summary>Starts listening to the game state.</summary>
        private void OnEnable()
        {
            GameEvents.OnGameStateChanged += HandleStateChanged;
        }

        /// <summary>Stops listening to the game state.</summary>
        private void OnDisable()
        {
            GameEvents.OnGameStateChanged -= HandleStateChanged;
        }

        /// <summary>Counts down and logs a fake attack while allowed to.</summary>
        private void Update()
        {
            if (!_canAttack)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer >= _attackInterval)
            {
                _timer = 0f;
                Debug.Log($"{nameof(DebugTestBossController)} on '{name}': fake attack (would fire on a real boss).", this);
            }
        }

        /// <summary>Only Gameplay allows attacking; Dialogue (and any other state) freezes the timer where it stands.</summary>
        private void HandleStateChanged(GameState state)
        {
            _canAttack = state == GameState.Gameplay;
        }
    }
}
