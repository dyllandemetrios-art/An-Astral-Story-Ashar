using Ashar.Environment;
using UnityEngine;

namespace Ashar.Enemies
{
    /// <summary>
    /// Test tool: creates enemies one after the other, cycling through a list of EnemyData, so each kind can be watched.
    /// RESPONSIBILITIES: give the developer a repeatable stream of enemies to check movement, damage and score. It is only
    /// placed in the TestBed scene; the real enemies come from the wave system (story E2-04).
    /// HOW IT WORKS: every interval, one enemy of the next kind is created at a place that suits its movement (above the
    /// screen for lines and dives, beside it for sweeps, on the screen for circling and static ones).
    /// </summary>
    public class DebugEnemySpawnerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Kinds of enemy to create, used one after the other.")]
        private EnemyData[] _enemies;

        [SerializeField, Tooltip("The player ship, aimed at by Dive enemies. Set it on the scene instance.")]
        private Transform _target;

        [SerializeField, Tooltip("Scene object that holds the enemies (Runtime/Enemies). Set it on the scene instance.")]
        private Transform _enemyParent;

        [SerializeField, Tooltip("Scene object that holds the effects (Runtime/FX). Set it on the scene instance.")]
        private Transform _fxParent;

        [SerializeField, Tooltip("Scene object that holds the enemy bullets (Runtime/Projectiles). Set it on the scene instance.")]
        private Transform _projectileParent;

        [SerializeField, Tooltip("Play area, used to know where the screen is. Set it on the scene instance.")]
        private PlayAreaController _playArea;

        [Header("Settings")]
        [SerializeField, Min(0.1f), Tooltip("Seconds between two enemies.")]
        private float _interval = 1.5f;

        [SerializeField, Min(0f), Tooltip("How far outside the screen an enemy may go before it is removed, in world units. Must be larger than the distance it appears outside the screen.")]
        private float _lifeMargin = 3f;

        [SerializeField, Min(0f), Tooltip("How far outside the screen an enemy bullet may go before it is destroyed, in world units.")]
        private float _bulletMargin = 1f;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: enemies created since the scene started.")]
        private int _spawnedCount;

        private float _timer;     // Seconds left before the next enemy.
        private int _nextIndex;   // Index of the next kind of enemy.

        /// <summary>Checks that every reference is set, and disables the component if one is missing.</summary>
        private void Awake()
        {
            if (_enemies == null || _enemies.Length == 0 || _target == null || _enemyParent == null || _fxParent == null || _projectileParent == null || _playArea == null)
            {
                Debug.LogError($"{nameof(DebugEnemySpawnerController)} on '{name}' is missing a reference or has no enemy. Spawner disabled.", this);
                enabled = false;
            }
        }

        /// <summary>Counts down and creates an enemy each time the interval has passed.</summary>
        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer > 0f)
            {
                return;
            }

            _timer += _interval;
            Spawn(_enemies[_nextIndex]);
            _nextIndex = (_nextIndex + 1) % _enemies.Length;
        }

        /// <summary>Creates one enemy of the given kind, at a place that suits its movement.</summary>
        private void Spawn(EnemyData data)
        {
            Rect screen = _playArea.ScreenBounds;
            Vector2 position;
            switch (data.Movement)
            {
                case EnemyMovementType.LateralSweep:
                    position = new Vector2(Random.value < 0.5f ? screen.xMin - 1f : screen.xMax + 1f, Random.Range(1f, screen.yMax - 1f));
                    break;

                case EnemyMovementType.Orbit:
                case EnemyMovementType.Static:
                    position = new Vector2(Random.Range(screen.xMin + 3f, screen.xMax - 3f), Random.Range(1.5f, screen.yMax - 1.5f));
                    break;

                default:
                    position = new Vector2(Random.Range(screen.xMin + 1f, screen.xMax - 1f), screen.yMax + 1f);
                    break;
            }

            EnemyController enemy = Instantiate(data.Prefab, new Vector3(position.x, position.y, 0f), Quaternion.identity, _enemyParent);
            Rect life = PlayAreaController.Inflate(screen, _lifeMargin);
            Rect bulletBounds = PlayAreaController.Inflate(screen, _bulletMargin);
            enemy.Initialize(data, new EnemySpawnContext(_target, life, _fxParent, _projectileParent, bulletBounds));
            _spawnedCount++;
        }
    }
}
