using Ashar.Environment;
using UnityEngine;

namespace Ashar.Combat
{
    /// <summary>
    /// Test tool: fires dummy enemy bullets straight down, lined up with the player plus a sideways offset.
    /// RESPONSIBILITIES: give the developer a repeatable stream of bullets that hit or narrowly miss the ship, to
    /// check hit detection and, later, the graze. It is only placed in the TestBed scene.
    /// HOW IT WORKS: every interval, one bullet is created at the emitter height, at the player's x plus the next
    /// offset of the list (the list repeats). An offset of 0 hits a ship that stands still; a small one grazes it.
    /// WHY: real enemies and patterns come in later stories; this keeps the collision rules testable now.
    /// </summary>
    public class DebugBulletEmitterController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Dummy enemy bullet to instantiate. It must have a ProjectileController.")]
        private ProjectileController _bulletPrefab;

        [SerializeField, Tooltip("The ship the bullets are aimed at. Set it on the scene instance.")]
        private Transform _target;

        [SerializeField, Tooltip("Scene object that holds the bullets (Runtime/Projectiles). Set it on the scene instance.")]
        private Transform _projectileParent;

        [SerializeField, Tooltip("Play area, used to know where the screen is. Set it on the scene instance.")]
        private PlayAreaController _playArea;

        [Header("Settings")]
        [SerializeField, Min(0.05f), Tooltip("Seconds between two bullets.")]
        private float _interval = 0.6f;

        [SerializeField, Min(0.1f), Tooltip("Bullet speed in world units per second (spec §4: 5.6 for a standard enemy bullet).")]
        private float _bulletSpeed = 5.6f;

        [SerializeField, Min(0f), Tooltip("How far outside the screen a bullet may go before it is destroyed, in world units.")]
        private float _despawnMargin = 1f;

        [SerializeField, Tooltip("Sideways offsets from the player, in world units, used one after the other. 0 is a hit; a value just above the hitbox is a near miss.")]
        private float[] _offsets = { 0f, 0.35f, -0.35f, 0.2f, -0.2f };

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: number of bullets fired since the scene started.")]
        private int _firedCount;

        private float _timer;      // Seconds left before the next bullet.
        private int _offsetIndex;  // Index of the next offset to use.

        /// <summary>Checks that every reference is set, and disables the component if one is missing.</summary>
        private void Awake()
        {
            if (_bulletPrefab == null || _target == null || _projectileParent == null || _playArea == null || _offsets == null || _offsets.Length == 0)
            {
                Debug.LogError($"{nameof(DebugBulletEmitterController)} on '{name}' is missing a reference or has no offsets. Emitter disabled.", this);
                enabled = false;
            }
        }

        /// <summary>Counts down and fires a bullet each time the interval has passed.</summary>
        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer > 0f)
            {
                return;
            }

            _timer += _interval;
            Fire();
        }

        /// <summary>Creates one bullet above the target (plus the next offset) and sends it straight down.</summary>
        private void Fire()
        {
            float x = _target.position.x + _offsets[_offsetIndex];
            _offsetIndex = (_offsetIndex + 1) % _offsets.Length;

            var position = new Vector3(x, transform.position.y, 0f);
            ProjectileController bullet = Instantiate(_bulletPrefab, position, Quaternion.identity, _projectileParent);

            Rect lifeBounds = PlayAreaController.Inflate(_playArea.ScreenBounds, _despawnMargin);
            bullet.Initialize(Vector2.down, _bulletSpeed, 1f, lifeBounds);
            _firedCount++;
        }
    }
}
