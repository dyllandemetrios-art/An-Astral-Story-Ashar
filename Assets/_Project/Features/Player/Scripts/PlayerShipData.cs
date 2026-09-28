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

        [Header("Dash")]
        [SerializeField, Min(0f), Tooltip("Distance covered by one dash, in world units (spec §4: 2.8).")]
        private float _dashDistance = 2.8f;

        [SerializeField, Min(0.01f), Tooltip("Time taken to cover the dash distance, in seconds.")]
        private float _dashDuration = 0.18f;

        [SerializeField, Min(0f), Tooltip("Time during which the ship cannot be hit, counted from the start of the dash, in seconds. Longer than the dash itself, so the ship stays protected a moment after it.")]
        private float _dashInvulnTime = 0.28f;

        [SerializeField, Min(0f), Tooltip("Time between the start of one dash and the moment the next one is allowed, in seconds.")]
        private float _dashCooldown = 1.5f;

        [Header("Hitbox")]
        [SerializeField, Min(0.01f), Tooltip("Half the width of the hitbox capsule, in world units: the only part of the ship that can be hit (spec §4 started at 0.11, about 5 reference pixels).")]
        private float _hitboxRadius = 0.11f;

        [SerializeField, Min(0.02f), Tooltip("Total height of the hitbox capsule, rounded ends included, in world units. The ship is a long shape, so the capsule covers the nose and the engine. Never smaller than the width.")]
        private float _hitboxHeight = 1.1f;

        [SerializeField, Tooltip("Show the silver dot that marks the hitbox. Meant to become an option of the game (accessibility).")]
        private bool _showHitbox = true;

        [Header("Shield")]
        [SerializeField, Min(0f), Tooltip("Seconds of protection once the shield is activated (spec E5-05: 5).")]
        private float _shieldProtectionTime = 5f;

        [SerializeField, Min(0f), Tooltip("Seconds of recharge after the protection ends, before the shield can be activated again (spec E5-05: 20).")]
        private float _shieldCooldown = 20f;

        [Header("Graze")]
        [SerializeField, Min(0.01f), Tooltip("Half the width of the graze zone, in world units (spec §7.1: 0.6). The zone is the hitbox capsule grown by this much all round; a bullet entering it without touching the hitbox is a near miss that earns points.")]
        private float _grazeRadius = 0.6f;

        [SerializeField, Min(0), Tooltip("Score points earned by one graze (spec §7.1: 20). Each bullet counts only once.")]
        private int _grazeScore = 20;

        /// <summary>Ship speed in world units per second.</summary>
        public float MoveSpeed => _moveSpeed;

        /// <summary>Half the width of the graze zone, in world units.</summary>
        public float GrazeRadius => _grazeRadius;

        /// <summary>Score points earned by one graze.</summary>
        public int GrazeScore => _grazeScore;

        /// <summary>Half the width of the hitbox capsule, in world units.</summary>
        public float HitboxRadius => _hitboxRadius;

        /// <summary>Total height of the hitbox capsule, in world units.</summary>
        public float HitboxHeight => _hitboxHeight;

        /// <summary>True when the silver hitbox dot is shown.</summary>
        public bool ShowHitbox => _showHitbox;

        /// <summary>Distance covered by one dash, in world units.</summary>
        public float DashDistance => _dashDistance;

        /// <summary>Time taken to cover the dash distance, in seconds.</summary>
        public float DashDuration => _dashDuration;

        /// <summary>Invulnerability time from the start of the dash, in seconds.</summary>
        public float DashInvulnTime => _dashInvulnTime;

        /// <summary>Time from the start of a dash until the next one is allowed, in seconds.</summary>
        public float DashCooldown => _dashCooldown;

        /// <summary>Shots per second while the Fire button is held.</summary>
        public float FireRate => _fireRate;

        /// <summary>Damage of one bullet.</summary>
        public float BulletDamage => _bulletDamage;

        /// <summary>Bullet speed in world units per second.</summary>
        public float BulletSpeed => _bulletSpeed;

        /// <summary>Seconds of protection once the shield is activated.</summary>
        public float ShieldProtectionTime => _shieldProtectionTime;

        /// <summary>Seconds of recharge after the protection ends, before the shield can be activated again.</summary>
        public float ShieldCooldown => _shieldCooldown;
    }
}
