using System;

namespace Ashar.Core
{
    /// <summary>
    /// The game's event bus: one static place where systems announce what happened, without knowing who listens.
    /// RESPONSIBILITIES: declare the events of the game and let systems raise them.
    /// HOW IT WORKS: a system calls a Raise method; every script subscribed to the event is told. Scripts subscribe in
    /// OnEnable and unsubscribe in OnDisable, so a destroyed object is never called.
    /// WHY: a static bus lets, for example, the health, the HUD and the sound react to a hit without any of them
    /// holding a reference to the others. An event can only be raised from inside its own class, hence the
    /// Raise methods.
    /// PATTERN: publish/subscribe (called EventSystem in the author's other projects; renamed here so it does not
    /// clash with UnityEngine.EventSystems.EventSystem, used by the UI).
    /// </summary>
    public static class GameEvents
    {
        /// <summary>Raised when the player ship takes a hit that counts (not while invulnerable).</summary>
        public static event Action OnPlayerHit;

        /// <summary>Raised each time the player ship fires a bullet.</summary>
        public static event Action OnPlayerFired;

        /// <summary>Raised when the player ship starts a dash.</summary>
        public static event Action OnPlayerDashed;

        /// <summary>Raised when the player ship successfully activates its shield (spec E5-05). Used by the tutorial (E3-08) to validate the action.</summary>
        public static event Action OnPlayerShielded;

        /// <summary>Raised when an enemy bullet grazes the player ship. The value is the score points earned.</summary>
        public static event Action<int> OnPlayerGrazed;

        /// <summary>Raised when the player destroys an enemy. The value is the score points it is worth.</summary>
        public static event Action<int> OnEnemyKilled;

        /// <summary>Raised when the wave table reaches its End event: the mission is over.</summary>
        public static event Action OnMissionEnded;

        /// <summary>Raised when the score changes. The value is the new score.</summary>
        public static event Action<int> OnScoreChanged;

        /// <summary>Raised when the number of lives changes. The value is the lives left.</summary>
        public static event Action<int> OnLivesChanged;

        /// <summary>Raised when the game state changes (for example to GameOver, or back to Gameplay after a continue).</summary>
        public static event Action<GameState> OnGameStateChanged;

        /// <summary>Raised when the player has lost a life but has another one: the ship must reappear.</summary>
        public static event Action OnPlayerRespawnRequested;

        /// <summary>Announces that the player ship has been hit.</summary>
        public static void RaisePlayerHit()
        {
            OnPlayerHit?.Invoke();
        }

        /// <summary>Announces that an enemy bullet has grazed the player ship, with the points it earns.</summary>
        public static void RaisePlayerGrazed(int points)
        {
            OnPlayerGrazed?.Invoke(points);
        }

        /// <summary>Announces that an enemy has been destroyed, with the score points it is worth.</summary>
        public static void RaiseEnemyKilled(int points)
        {
            OnEnemyKilled?.Invoke(points);
        }

        /// <summary>Announces that the mission has ended.</summary>
        public static void RaiseMissionEnded()
        {
            OnMissionEnded?.Invoke();
        }

        /// <summary>Announces the new score.</summary>
        public static void RaiseScoreChanged(int score)
        {
            OnScoreChanged?.Invoke(score);
        }

        /// <summary>Announces the lives left.</summary>
        public static void RaiseLivesChanged(int lives)
        {
            OnLivesChanged?.Invoke(lives);
        }

        /// <summary>Announces the new game state.</summary>
        public static void RaiseGameStateChanged(GameState state)
        {
            OnGameStateChanged?.Invoke(state);
        }

        /// <summary>Announces that the ship must reappear after losing a life.</summary>
        public static void RaisePlayerRespawnRequested()
        {
            OnPlayerRespawnRequested?.Invoke();
        }

        /// <summary>Announces that the player ship has fired a bullet.</summary>
        public static void RaisePlayerFired()
        {
            OnPlayerFired?.Invoke();
        }

        /// <summary>Announces that the player ship has started a dash.</summary>
        public static void RaisePlayerDashed()
        {
            OnPlayerDashed?.Invoke();
        }

        /// <summary>Announces that the player ship has successfully activated its shield.</summary>
        public static void RaisePlayerShielded()
        {
            OnPlayerShielded?.Invoke();
        }
    }
}
