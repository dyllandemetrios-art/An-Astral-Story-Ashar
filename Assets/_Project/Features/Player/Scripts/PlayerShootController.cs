using Ashar.Combat;
using Ashar.Environment;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ashar.Player
{
    /// <summary>
    /// Fires the main weapon while the Fire button is held.
    /// RESPONSIBILITIES: count down the cooldown, create bullets at the muzzle, and file them under Runtime/Projectiles.
    /// HOW IT WORKS: each frame the cooldown shrinks. While Fire is held and the cooldown is over, a bullet is
    /// instantiated at the muzzle and the cooldown restarts at 1 / fireRate. The bullet then flies and removes
    /// itself (see ProjectileController). Rate, damage and speed come from PlayerShipData.
    /// WHY: the timing is in a static method with no Unity state so it can be unit-tested. The cooldown is kept
    /// when the button is released, so tapping Fire faster than the fire rate gives no advantage.
    /// </summary>
    public class PlayerShootController : MonoBehaviour
    {
        private const int MaxShotsPerFrame = 4; // Safety cap so one very long frame cannot flood the scene with bullets.

        [Header("Data")]
        [SerializeField, Tooltip("Tuning values of the ship: fire rate, bullet damage and bullet speed.")]
        private PlayerShipData _shipData;

        [Header("References")]
        [SerializeField, Tooltip("Bullet prefab to instantiate. It must have a ProjectileController.")]
        private ProjectileController _bulletPrefab;

        [SerializeField, Tooltip("Point where bullets appear, usually at the nose of the ship.")]
        private Transform _muzzle;

        [SerializeField, Tooltip("Scene object that holds the bullets (Runtime/Projectiles). Set it on the scene instance.")]
        private Transform _projectileParent;

        [SerializeField, Tooltip("Play area, used to know where the screen is. Set it on the scene instance.")]
        private PlayAreaController _playArea;

        [SerializeField, Tooltip("Fire action (Button) of the AsharControls input asset.")]
        private InputActionReference _fireAction;

        [Header("Settings")]
        [SerializeField, Min(0f), Tooltip("How far outside the screen a bullet may go before it is destroyed, in world units. Keeps the whole sprite from vanishing while still visible.")]
        private float _despawnMargin = 1f;

        [Header("Debug")]
        [SerializeField, Tooltip("Fire without pressing the button. For tests and performance measures (E2-01); leave off otherwise.")]
        private bool _debugAlwaysFire;

        private float _cooldown; // Seconds left before the next shot is allowed.

        /// <summary>Checks that every reference is set, and disables the component if one is missing.</summary>
        private void Awake()
        {
            if (_shipData == null || _bulletPrefab == null || _muzzle == null || _projectileParent == null || _playArea == null || _fireAction == null)
            {
                Debug.LogError($"{nameof(PlayerShootController)} on '{name}' is missing a reference (ship data, bullet, muzzle, projectile parent, play area or fire action). Shooting disabled.", this);
                enabled = false;
            }
        }

        /// <summary>Makes sure the Fire action is enabled.</summary>
        private void OnEnable()
        {
            // The action is shared with other player components, so it is deliberately never disabled here.
            if (_fireAction != null)
            {
                _fireAction.action.Enable();
            }
        }

        /// <summary>Counts the cooldown and fires the bullets due this frame.</summary>
        private void Update()
        {
            bool held = _debugAlwaysFire || _fireAction.action.IsPressed();
            int shots = ConsumeShots(ref _cooldown, Time.deltaTime, 1f / _shipData.FireRate, held, MaxShotsPerFrame);

            for (int i = 0; i < shots; i++)
            {
                Fire();
            }
        }

        /// <summary>Creates one bullet at the muzzle and sends it straight up.</summary>
        private void Fire()
        {
            ProjectileController bullet = Instantiate(_bulletPrefab, _muzzle.position, Quaternion.identity, _projectileParent);
            Rect lifeBounds = Inflate(_playArea.ScreenBounds, _despawnMargin);
            bullet.Initialize(Vector2.up, _shipData.BulletSpeed, _shipData.BulletDamage, lifeBounds);
        }

        /// <summary>
        /// Advances the cooldown and returns how many shots are due. The cooldown never goes below zero while the
        /// button is up, so releasing and pressing again does not give extra shots.
        /// Static and free of Unity state so it can be unit-tested. An interval of 0 or less fires nothing.
        /// </summary>
        public static int ConsumeShots(ref float cooldown, float deltaTime, float fireInterval, bool fireHeld, int maxShots)
        {
            // A weapon that is already ready has no time to carry over: only a cooldown that runs out during
            // this frame passes its leftover time on. Otherwise the very first shot would borrow time it never had.
            cooldown = cooldown > 0f ? cooldown - deltaTime : 0f;

            if (!fireHeld || fireInterval <= 0f)
            {
                cooldown = Mathf.Max(0f, cooldown);
                return 0;
            }

            // While the button is held, the time left over after a shot is kept (the cooldown may be slightly
            // negative), so the real fire rate stays exact whatever the frame rate.
            int shots = 0;
            while (cooldown <= 0f && shots < maxShots)
            {
                shots++;
                cooldown += fireInterval;
            }

            // If the safety cap was hit, drop the backlog instead of firing it on the next frames.
            cooldown = Mathf.Max(0f, cooldown);
            return shots;
        }

        /// <summary>Returns the rectangle grown by the margin on every side.</summary>
        public static Rect Inflate(Rect rect, float margin)
        {
            return new Rect(rect.xMin - margin, rect.yMin - margin, rect.width + 2f * margin, rect.height + 2f * margin);
        }
    }
}
