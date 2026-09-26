using System;
using Ashar.Environment;
using UnityEngine;

namespace Ashar.Combat
{
    /// <summary>How the test bench gets its bullets: created and destroyed each time, or reused from a pool.</summary>
    public enum StressSpawnMode
    {
        /// <summary>Instantiate a new bullet and Destroy it when it leaves the screen (what the game does today).</summary>
        InstantiateDestroy,

        /// <summary>Reuse bullets from a minimal pool instead of creating and destroying them.</summary>
        Pooled,
    }

    /// <summary>
    /// The performance test bench (story E2-01): keeps a fixed number of bullets alive and measures the cost.
    /// RESPONSIBILITIES: run the phases (each target count in each spawn mode), keep the bullet count at the target by
    /// creating bullets as others leave the screen, and tell the reporter when to start and stop measuring.
    /// HOW IT WORKS: a phase has a warm-up, when the count is filled and things settle, then a measured period. After
    /// each phase all bullets are removed and the memory is collected, so one phase does not disturb the next. When the
    /// last phase is over, the results are written to a CSV file and the program quits.
    /// WHY: the choice between Instantiate/Destroy and pooling, and the maximum number of bullets, must come from a
    /// measure in a real build, not from a guess (spec section 8). This scene is a test bench only, outside the game.
    /// </summary>
    public class ProjectileStressTestController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Bullet to create. It must have a StressBulletController, a kinematic body and a trigger collider.")]
        private StressBulletController _bulletPrefab;

        [SerializeField, Tooltip("Scene object that holds all the bullets (Runtime/Projectiles).")]
        private Transform _projectileParent;

        [SerializeField, Tooltip("Play area, used to know where the screen is.")]
        private PlayAreaController _playArea;

        [SerializeField, Tooltip("Records the frames and writes the results.")]
        private PerformanceReporterController _reporter;

        [Header("Phases")]
        [SerializeField, Tooltip("Numbers of bullets alive at once to test, in order. Each one is run in both spawn modes.")]
        private int[] _targetCounts = { 500, 1000, 2000 };

        [SerializeField, Min(0f), Tooltip("Seconds at the start of each phase before measuring, so the count fills up and the pool is warm.")]
        private float _warmupSeconds = 5f;

        [SerializeField, Min(1f), Tooltip("Seconds measured in each phase.")]
        private float _measureSeconds = 20f;

        [SerializeField, Min(0f), Tooltip("Seconds to wait after the scene starts before the first phase, so start-up hitches are not measured.")]
        private float _startDelaySeconds = 2f;

        [Header("Bullets")]
        [SerializeField, Min(0.1f), Tooltip("Slowest bullet speed, in world units per second.")]
        private float _minSpeed = 3f;

        [SerializeField, Min(0.1f), Tooltip("Fastest bullet speed, in world units per second.")]
        private float _maxSpeed = 8f;

        [SerializeField, Min(0f), Tooltip("Largest sideways speed, in world units per second. Gives the bullets slightly different paths.")]
        private float _maxSidewaysSpeed = 2f;

        [SerializeField, Min(1), Tooltip("Most bullets created in one frame, so filling the screen does not create one giant hitch.")]
        private int _maxSpawnPerFrame = 300;

        [SerializeField, Min(0f), Tooltip("How far outside the screen a bullet may go before it is released, in world units.")]
        private float _despawnMargin = 1f;

        [Header("Run")]
        [SerializeField, Tooltip("Turn VSync off and remove the frame rate cap, so the measure shows the real cost of a frame instead of a locked 60 or 120.")]
        private bool _disableVSync = true;

        [SerializeField, Tooltip("Quit the program when the last phase is over (used for the automatic run of the build).")]
        private bool _quitWhenDone = true;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: index of the current phase.")]
        private int _phaseIndex = -1;

        [SerializeField, Tooltip("Read-only: bullets alive right now.")]
        private int _alive;

        [SerializeField, Tooltip("Read-only: what the bench is doing.")]
        private string _status = "Waiting";

        private StressTestPool _pool;       // Pool of the current phase; null outside Pooled phases.
        private StressSpawnMode _mode;      // Spawn mode of the current phase.
        private int _target;                // Bullets to keep alive in the current phase.
        private float _phaseTimer;          // Seconds since the current phase started.
        private float _startTimer;          // Seconds since the scene started, for the start delay.
        private bool _measuring;            // True while the reporter records frames.
        private bool _filled;               // True once the count has reached the target in this phase.
        private bool _finished;             // True after the last phase.
        private Rect _lifeBounds;           // Where bullets may live: the screen grown by the despawn margin.

        /// <summary>Bullets alive right now.</summary>
        public int Alive => _alive;

        /// <summary>True when the last phase is over.</summary>
        public bool Finished => _finished;

        /// <summary>Human-readable description of the current phase, for the on-screen display.</summary>
        public string Status => _status;

        /// <summary>Number of phases: each target count in both spawn modes.</summary>
        public int PhaseCount => _targetCounts.Length * 2;

        /// <summary>Checks the references, applies the VSync setting and prepares the life bounds.</summary>
        private void Awake()
        {
            if (_bulletPrefab == null || _projectileParent == null || _playArea == null || _reporter == null || _targetCounts == null || _targetCounts.Length == 0)
            {
                Debug.LogError($"{nameof(ProjectileStressTestController)} on '{name}' is missing a reference or has no target counts. Bench disabled.", this);
                enabled = false;
                return;
            }

            if (_disableVSync)
            {
                // FrameRateSettings sets a 60 fps cap at start-up; the bench must run uncapped to show real headroom.
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = -1;
            }

            _lifeBounds = PlayAreaController.Inflate(_playArea.ScreenBounds, _despawnMargin);
        }

        /// <summary>Runs the start delay, then the phases one after the other.</summary>
        private void Update()
        {
            if (_finished)
            {
                return;
            }

            if (_phaseIndex < 0)
            {
                _startTimer += Time.unscaledDeltaTime;
                if (_startTimer >= _startDelaySeconds)
                {
                    StartPhase(0);
                }

                return;
            }

            _phaseTimer += Time.unscaledDeltaTime;
            TopUpBullets();

            if (!_measuring && _phaseTimer >= _warmupSeconds)
            {
                _measuring = true;
                _status = $"Phase {_phaseIndex + 1}/{PhaseCount}: {_target} bullets, {_mode}, MEASURING";
                _reporter.BeginMeasure(_mode.ToString(), _target);
            }

            if (_measuring && _phaseTimer >= _warmupSeconds + _measureSeconds)
            {
                _reporter.EndMeasure();
                _measuring = false;

                if (_phaseIndex + 1 < PhaseCount)
                {
                    StartPhase(_phaseIndex + 1);
                }
                else
                {
                    Finish();
                }
            }
        }

        /// <summary>Gives a bullet back: destroyed or sent to the pool, depending on the current mode. Called by the bullets.</summary>
        public void ReleaseBullet(StressBulletController bullet)
        {
            _alive--;
            if (_mode == StressSpawnMode.Pooled)
            {
                _pool.Release(bullet);
            }
            else
            {
                Destroy(bullet.gameObject);
            }
        }

        /// <summary>Removes every bullet, collects memory, and starts the given phase.</summary>
        private void StartPhase(int index)
        {
            ClearAllBullets();
            GC.Collect(); // Level the ground: garbage left by the last phase must not be counted in this one.

            _phaseIndex = index;
            _target = _targetCounts[index / 2];
            _mode = (StressSpawnMode)(index % 2);
            _pool = _mode == StressSpawnMode.Pooled ? new StressTestPool(CreateBullet) : null;
            _phaseTimer = 0f;
            _alive = 0;
            _filled = false;
            _measuring = false;
            _status = $"Phase {index + 1}/{PhaseCount}: {_target} bullets, {_mode}, warming up";
        }

        /// <summary>Ends the run: writes the results and quits if asked to.</summary>
        private void Finish()
        {
            ClearAllBullets();
            _finished = true;
            _alive = 0;
            _status = "Finished";
            _reporter.WriteResults();

            if (_quitWhenDone)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }

        /// <summary>Creates bullets until the target is reached, at most a fixed number per frame.</summary>
        private void TopUpBullets()
        {
            int missing = Mathf.Min(_target - _alive, _maxSpawnPerFrame);
            for (int i = 0; i < missing; i++)
            {
                SpawnBullet();
            }

            if (!_filled && _alive >= _target)
            {
                _filled = true; // From now on, new bullets come from the top of the screen instead of anywhere.
            }
        }

        /// <summary>Creates or reuses one bullet with a random position and velocity, and launches it.</summary>
        private void SpawnBullet()
        {
            Rect screen = _playArea.ScreenBounds;
            float x = UnityEngine.Random.Range(screen.xMin, screen.xMax);
            // While filling, bullets appear anywhere on screen; afterwards they enter from the top like real ones.
            float y = _filled ? screen.yMax + 0.3f : UnityEngine.Random.Range(screen.yMin, screen.yMax);
            var position = new Vector3(x, y, 0f);

            StressBulletController bullet = _mode == StressSpawnMode.Pooled ? _pool.Get(position) : CreateBullet(position);
            var velocity = new Vector2(UnityEngine.Random.Range(-_maxSidewaysSpeed, _maxSidewaysSpeed), -UnityEngine.Random.Range(_minSpeed, _maxSpeed));
            bullet.Launch(velocity, _lifeBounds, this);
            _alive++;
        }

        /// <summary>Instantiates a new bullet under the projectile holder, at the origin.</summary>
        private StressBulletController CreateBullet()
        {
            return CreateBullet(Vector3.zero);
        }

        /// <summary>Instantiates a new bullet under the projectile holder, at the given position.</summary>
        private StressBulletController CreateBullet(Vector3 position)
        {
            return Instantiate(_bulletPrefab, position, Quaternion.identity, _projectileParent);
        }

        /// <summary>Destroys every bullet under the projectile holder, pooled and switched off ones included.</summary>
        private void ClearAllBullets()
        {
            for (int i = _projectileParent.childCount - 1; i >= 0; i--)
            {
                // Destroy() only takes effect at the end of the frame: switch the bullet off first, so it cannot
                // run one more Update and release itself a second time.
                GameObject bulletObject = _projectileParent.GetChild(i).gameObject;
                bulletObject.SetActive(false);
                Destroy(bulletObject);
            }
        }
    }
}
