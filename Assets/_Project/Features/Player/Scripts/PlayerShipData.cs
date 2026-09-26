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

        [Header("Main weapon")]
        [SerializeField, Min(0.1f), Tooltip("Shots per second while the Fire button is held. With the damage below, 10 x 10 = 100 damage per second, the reference for all enemy HP (spec §7.1).")]
        private float _fireRate = 10f;

        [SerializeField, Min(0f), Tooltip("Damage of one bullet.")]
        private float _bulletDamage = 10f;

        [SerializeField, Min(0.1f), Tooltip("Bullet speed in world units per second (spec §4: 22.5).")]
        private float _bulletSpeed = 22.5f;

        /// <summary>Ship speed in world units per second.</summary>
        public float MoveSpeed => _moveSpeed;

        /// <summary>Shots per second while the Fire button is held.</summary>
        public float FireRate => _fireRate;

        /// <summary>Damage of one bullet.</summary>
        public float BulletDamage => _bulletDamage;

        /// <summary>Bullet speed in world units per second.</summary>
        public float BulletSpeed => _bulletSpeed;
    }
}
