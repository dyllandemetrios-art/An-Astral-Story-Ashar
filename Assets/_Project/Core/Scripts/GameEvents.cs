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

        /// <summary>Announces that the player ship has been hit.</summary>
        public static void RaisePlayerHit()
        {
            OnPlayerHit?.Invoke();
        }
    }
}
