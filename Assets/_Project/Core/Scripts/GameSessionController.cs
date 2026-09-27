using UnityEngine;
using UnityEngine.InputSystem;

namespace Ashar.Core
{
    /// <summary>
    /// Runs the arcade loop: creates the GameSession, turns game events into score and lost lives, and announces the changes.
    /// RESPONSIBILITIES: give points for kills and grazes, take a life when the player is hit, ask for the ship to respawn or
    /// declare the game over, and start a continue when the player asks for one.
    /// HOW IT WORKS: it subscribes to GameEvents in OnEnable. It owns the GameSession (also reachable through
    /// GameSession.Current) and raises OnScoreChanged, OnLivesChanged, OnGameStateChanged and OnPlayerRespawnRequested for the
    /// HUD and the ship to react. While the game is over, pressing the Fire button continues (provisional: the game over
    /// screen with "Restart the mission" and "Menu" comes with the menus).
    /// WHY: the rules live in GameSession (pure, tested); this class is only the link with the game events.
    /// </summary>
    public class GameSessionController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField, Tooltip("Global tuning values: lives, continues.")]
        private GameBalanceData _balance;

        [Header("References")]
        [SerializeField, Tooltip("Fire action (Button) of the AsharControls input asset. Pressed during a game over, it starts a continue.")]
        private InputActionReference _continueAction;

        /// <summary>Checks the data, creates the session and makes it the current one.</summary>
        private void Awake()
        {
            if (_balance == null)
            {
                Debug.LogError($"{nameof(GameSessionController)} on '{name}' is missing its balance data. Game session disabled.", this);
                enabled = false;
                return;
            }

            GameSession.Current = new GameSession(_balance.StartLives, _balance.Continues);
            ApplyBalance();
        }

        /// <summary>Starts listening to the game events and announces the starting numbers.</summary>
        private void OnEnable()
        {
            GameEvents.OnEnemyKilled += HandlePoints;
            GameEvents.OnPlayerGrazed += HandlePoints;
            GameEvents.OnPlayerHit += HandlePlayerHit;
            GameEvents.OnMissionEnded += HandleMissionEnded;

            if (_continueAction != null)
            {
                _continueAction.action.Enable();
            }

            if (GameSession.Current != null)
            {
                GameEvents.RaiseLivesChanged(GameSession.Current.Lives);
                GameEvents.RaiseScoreChanged(GameSession.Current.Score);
            }
        }

        /// <summary>Stops listening to the game events.</summary>
        private void OnDisable()
        {
            GameEvents.OnEnemyKilled -= HandlePoints;
            GameEvents.OnPlayerGrazed -= HandlePoints;
            GameEvents.OnPlayerHit -= HandlePlayerHit;
            GameEvents.OnMissionEnded -= HandleMissionEnded;
        }

        /// <summary>Forgets the session when this object goes away, so no script keeps reading an old game.</summary>
        private void OnDestroy()
        {
            GameSession.Current = null;
        }

        /// <summary>Copies the balance values into the session, and starts a continue when Fire is pressed during a game over.</summary>
        private void Update()
        {
            ApplyBalance();
            GameSession session = GameSession.Current;
            if (session != null && session.CanContinue && _continueAction != null && _continueAction.action.WasPressedThisFrame())
            {
                Continue();
            }
        }

        /// <summary>Copies the global balance values of the asset into the running session (a few assignments, done every frame).</summary>
        private void ApplyBalance()
        {
            GameSession.Current?.SetBalance(_balance.EnemyHpMultiplier, _balance.FireDensityMultiplier, _balance.BulletSpeedMultiplier, _balance.MaxProjectiles);
        }

        /// <summary>Adds points to the score and announces it.</summary>
        private void HandlePoints(int points)
        {
            GameSession session = GameSession.Current;
            if (session == null)
            {
                return;
            }

            int before = session.Score;
            session.AddScore(points);
            if (session.Score != before)
            {
                GameEvents.RaiseScoreChanged(session.Score);
            }
        }

        /// <summary>Takes a life: the ship reappears, or the game is over.</summary>
        private void HandlePlayerHit()
        {
            GameSession session = GameSession.Current;
            if (session == null || session.State != GameState.Gameplay)
            {
                return;
            }

            bool hasLifeLeft = session.LoseLife();
            GameEvents.RaiseLivesChanged(session.Lives);
            if (hasLifeLeft)
            {
                GameEvents.RaisePlayerRespawnRequested();
            }
            else
            {
                GameEvents.RaiseGameStateChanged(session.State);
            }
        }

        /// <summary>Marks the mission as ended: the score is final.</summary>
        private void HandleMissionEnded()
        {
            GameSession session = GameSession.Current;
            if (session == null)
            {
                return;
            }

            session.EndMission();
            GameEvents.RaiseGameStateChanged(session.State);
        }

        /// <summary>Starts a continue: fresh lives, score back to 0, and the ship comes back.</summary>
        private void Continue()
        {
            GameSession session = GameSession.Current;
            if (session == null || !session.Continue())
            {
                return;
            }

            GameEvents.RaiseGameStateChanged(session.State);
            GameEvents.RaiseLivesChanged(session.Lives);
            GameEvents.RaiseScoreChanged(session.Score);
            GameEvents.RaisePlayerRespawnRequested();
        }
    }
}
