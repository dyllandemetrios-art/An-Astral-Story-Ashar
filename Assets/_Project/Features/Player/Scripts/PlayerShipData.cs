using UnityEngine;

namespace Ashar.Player
{
    /// <summary>
    /// Tuning values of the player ship, shared by every player component.
    /// RESPONSIBILITIES: hold the numbers a designer may want to change, so that no gameplay value lives in code.
    /// HOW IT WORKS: this is a ScriptableObject asset (PlayerShipData_Default). Components read it through
    /// read-only properties; the values are edited in the Inspector, even during Play Mode.
    /// PATTERN: data-driven design. Later stories add their own fields here (fire rate, dash, hitbox...).
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerShipData_Default", menuName = "Ashar/Player Ship Data")]
    public class PlayerShipData : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField, Min(0f), Tooltip("Ship speed in world units per second. 9 crosses the 20-unit wide screen in about 2.2 seconds (spec §4).")]
        private float _moveSpeed = 9f;

        /// <summary>Ship speed in world units per second.</summary>
        public float MoveSpeed => _moveSpeed;
    }
}
