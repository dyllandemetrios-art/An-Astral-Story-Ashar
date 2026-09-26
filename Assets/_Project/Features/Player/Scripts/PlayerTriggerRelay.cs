using System;
using UnityEngine;

namespace Ashar.Player
{
    /// <summary>
    /// Passes the trigger contacts of one collider on to whoever is interested, and says which collider it was.
    /// RESPONSIBILITIES: turn Unity's OnTriggerEnter2D message into a C# event on the object that owns the collider.
    /// HOW IT WORKS: it sits on the same GameObject as a trigger collider (the hitbox, the graze zone). Unity calls
    /// OnTriggerEnter2D on it when something enters that collider, and it raises the Entered event.
    /// WHY: the ship has several triggers on one Rigidbody2D. Unity also delivers all their contacts to the ship root,
    /// where a script could not tell the hitbox from the graze zone. One relay per collider keeps them apart.
    /// </summary>
    public class PlayerTriggerRelay : MonoBehaviour
    {
        /// <summary>Raised when another collider enters the trigger collider on this GameObject.</summary>
        public event Action<Collider2D> Entered;

        /// <summary>Called by Unity when another collider enters this trigger; passes it on.</summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            Entered?.Invoke(other);
        }
    }
}
