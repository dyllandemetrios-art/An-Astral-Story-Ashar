using System.Collections.Generic;
using Ashar.Core;
using Ashar.Dialogue;
using Ashar.Enemies;
using Ashar.Environment;
using UnityEngine;

namespace Ashar.Waves
{
    /// <summary>
    /// Plays a wave table: creates the enemies at the right moments, waits for groups to be cleared, and ends the mission.
    /// RESPONSIBILITIES: run the timeline of a WaveData, create enemy groups, count the enemies of each group, change the
    /// scroll speed smoothly, and announce the end of the mission.
    /// HOW IT WORKS: a clock counts the seconds of the current block. Each event whose time has come is run in order. A
    /// Spawn creates its enemies one after the other (the delay between units); a WaitForClear stops the clock until
    /// every enemy of its group has been destroyed or has left the screen, then starts a new block from 0. A group
    /// is only clear when none of its enemies is alive and none is still waiting to appear.
    /// WHY: the timeline is in seconds and independent of the music (spec §7.5). Counting enemies through an event
    /// (EnemyController.Removed) avoids searching the scene every frame.
    /// </summary>
    public class MissionRunnerController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField, Tooltip("The wave table to play.")]
        private WaveData _waves;

        [Header("References")]
        [SerializeField, Tooltip("Play area, used to know where the screen is.")]
        private PlayAreaController _playArea;

        [SerializeField, Tooltip("The player ship, aimed at by the enemies.")]
        private Transform _target;

        [SerializeField, Tooltip("Scene object that holds the enemies (Runtime/Enemies).")]
        private Transform _enemyParent;

        [SerializeField, Tooltip("Scene object that holds the effects (Runtime/FX).")]
        private Transform _fxParent;

        [SerializeField, Tooltip("Scene object that holds the enemy bullets (Runtime/Projectiles).")]
        private Transform _projectileParent;

        [SerializeField, Tooltip("Optional. The background whose scroll speed the ScrollSpeed events change.")]
        private BackgroundScrollController _background;

        [SerializeField, Tooltip("Required for Dialogue events: plays the sequence and reports back when it closes.")]
        private DialogueController _dialogueController;

        [Header("Settings")]
        [SerializeField, Tooltip("Start playing the wave table when the scene starts. Off = call StartMission() from another script.")]
        private bool _autoStart = true;

        [SerializeField, Min(0f), Tooltip("How far outside the screen the enemies appear, in world units.")]
        private float _spawnMargin = 1f;

        [SerializeField, Min(0f), Tooltip("How far outside the screen an enemy may go before it is removed, in world units. Must be larger than the spawn margin plus the depth of a V formation.")]
        private float _enemyLifeMargin = 5f;

        [SerializeField, Min(0f), Tooltip("How far outside the screen an enemy bullet may go before it is destroyed, in world units.")]
        private float _bulletMargin = 1f;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: what the runner is doing.")]
        private string _status = "Not started";

        [SerializeField, Tooltip("Read-only: seconds of the current block.")]
        private float _blockTime;

        [SerializeField, Tooltip("Read-only: index of the next event.")]
        private int _eventIndex;

        [SerializeField, Tooltip("Read-only: enemies alive that came from the wave table.")]
        private int _aliveTotal;

        private readonly Dictionary<string, int> _aliveByTag = new Dictionary<string, int>();   // Enemies alive, per group.
        private readonly Dictionary<string, int> _pendingByTag = new Dictionary<string, int>(); // Enemies waiting to appear, per group.
        private readonly List<PendingSpawn> _pending = new List<PendingSpawn>();                // Enemies waiting to appear.
        private float _clock;              // Seconds since the mission started, never reset: used for the delays between units.
        private bool _running;             // True while the timeline runs.
        private bool _finished;            // True after the End event.
        private string _waitingTag;        // Group being waited for, or null.
        private bool _waitingForDialogue;  // True while a Dialogue event blocks the timeline.
        private float _scrollFrom;         // Scroll speed at the start of the current transition.
        private float _scrollTo;           // Scroll speed to reach.
        private float _scrollDuration;     // Length of the current transition.
        private float _scrollElapsed;      // Time since the start of the current transition; negative when there is none.

        /// <summary>An enemy that has been scheduled but has not appeared yet.</summary>
        private struct PendingSpawn
        {
            public EnemyData Data;
            public Vector2 Position;
            public float DueClock;
            public string Tag;
        }

        /// <summary>True once the End event has been reached.</summary>
        public bool Finished => _finished;

        /// <summary>Enemies of the wave table alive right now.</summary>
        public int AliveTotal => _aliveTotal;

        /// <summary>Checks the references and starts the mission if asked to.</summary>
        private void Awake()
        {
            if (_waves == null || _playArea == null || _enemyParent == null || _fxParent == null || _projectileParent == null)
            {
                Debug.LogError($"{nameof(MissionRunnerController)} on '{name}' is missing its wave table or a scene reference. Runner disabled.", this);
                enabled = false;
                return;
            }

            _scrollElapsed = -1f;
            if (_autoStart)
            {
                StartMission();
            }
        }

        /// <summary>Starts (or restarts) the wave table from its first event. Also in the component's context menu, to start it by hand in Play Mode.</summary>
        [ContextMenu("Start mission")]
        public void StartMission()
        {
            _clock = 0f;
            _blockTime = 0f;
            _eventIndex = 0;
            _waitingTag = null;
            _finished = false;
            _running = true;
            _pending.Clear();
            _status = "Running";
        }

        /// <summary>Runs the clock, creates the enemies that are due, and plays the events whose time has come.</summary>
        private void Update()
        {
            if (!_running || _finished)
            {
                return;
            }

            if (_waitingForDialogue)
            {
                return; // The clock, the spawns and the events are all frozen until the dialogue closes.
            }

            float deltaTime = Time.deltaTime;
            _clock += deltaTime;
            UpdateScrollTransition(deltaTime);
            SpawnDueEnemies();

            if (_waitingTag != null)
            {
                if (!IsGroupClear(_waitingTag))
                {
                    return; // The clock of the block stays stopped.
                }

                _waitingTag = null;
                _blockTime = 0f;
                _eventIndex++;
                _status = "Running";
            }

            _blockTime += deltaTime;
            IReadOnlyList<WaveEvent> events = _waves.Events;
            while (_eventIndex < events.Count && events[_eventIndex].Time <= _blockTime && !_finished && _waitingTag == null && !_waitingForDialogue)
            {
                RunEvent(events[_eventIndex]);
            }
        }

        /// <summary>Plays one event of the table.</summary>
        private void RunEvent(WaveEvent waveEvent)
        {
            switch (waveEvent.Type)
            {
                case WaveEventType.Spawn:
                    ScheduleGroup(waveEvent);
                    _eventIndex++;
                    break;

                case WaveEventType.ScrollSpeed:
                    StartScrollTransition(waveEvent.TargetSpeed, waveEvent.TransitionDuration);
                    _eventIndex++;
                    break;

                case WaveEventType.WaitForClear:
                    if (IsGroupClear(waveEvent.GroupTag))
                    {
                        _blockTime = 0f; // Nothing to wait for: the next block starts at once.
                        _eventIndex++;
                    }
                    else
                    {
                        _waitingTag = waveEvent.GroupTag;
                        _status = $"Waiting for group '{waveEvent.GroupTag}'";
                    }

                    break;

                case WaveEventType.Dialogue:
                    PlayDialogue(waveEvent.Dialogue);
                    break;

                default: // End
                    _finished = true;
                    _status = "Finished";
                    GameEvents.RaiseMissionEnded();
                    break;
            }
        }

        /// <summary>Blocks the timeline and hands the sequence to the DialogueController, or skips it with a diagnostic if it cannot be played.</summary>
        private void PlayDialogue(DialogueData dialogue)
        {
            if (_dialogueController == null)
            {
                Debug.LogError($"{nameof(MissionRunnerController)}: a Dialogue event needs a DialogueController reference; skipped.", this);
                _eventIndex++;
                return;
            }

            _waitingForDialogue = true;
            _status = dialogue != null ? $"Dialogue '{dialogue.Id}'" : "Dialogue";
            _dialogueController.Play(dialogue, HandleDialogueFinished);
        }

        /// <summary>Called by the DialogueController once its sequence closes: resumes the timeline from a fresh block, like WaitForClear.</summary>
        private void HandleDialogueFinished()
        {
            if (this == null || !_waitingForDialogue)
            {
                return; // The runner is gone, or this call does not match an event we are waiting on.
            }

            _waitingForDialogue = false;
            _blockTime = 0f;
            _eventIndex++;
            _status = "Running";
        }

        /// <summary>Schedules every enemy of a Spawn event, one after the other.</summary>
        private void ScheduleGroup(WaveEvent waveEvent)
        {
            if (waveEvent.Enemy == null || waveEvent.Enemy.Prefab == null)
            {
                Debug.LogWarning($"{nameof(MissionRunnerController)}: a Spawn event has no enemy or the enemy has no prefab; skipped.", this);
                return;
            }

            string tag = waveEvent.GroupTag ?? "";
            Rect screen = _playArea.ScreenBounds;
            for (int i = 0; i < waveEvent.Count; i++)
            {
                _pending.Add(new PendingSpawn
                {
                    Data = waveEvent.Enemy,
                    Position = WaveMath.ComputeSpawnPosition(waveEvent.Entry, waveEvent.Formation, waveEvent.EntryOffset, waveEvent.EntryDepth,
                        i, waveEvent.Count, waveEvent.Spacing, screen, _spawnMargin),
                    DueClock = _clock + i * waveEvent.DelayBetweenUnits,
                    Tag = tag,
                });
                AddToCount(_pendingByTag, tag, 1);
            }
        }

        /// <summary>Creates the scheduled enemies whose time has come.</summary>
        private void SpawnDueEnemies()
        {
            for (int i = _pending.Count - 1; i >= 0; i--)
            {
                if (_pending[i].DueClock <= _clock)
                {
                    SpawnEnemy(_pending[i]);
                    _pending.RemoveAt(i);
                }
            }
        }

        /// <summary>Creates one enemy and starts counting it in its group.</summary>
        private void SpawnEnemy(PendingSpawn pending)
        {
            Rect screen = _playArea.ScreenBounds;
            EnemyController enemy = Instantiate(pending.Data.Prefab, new Vector3(pending.Position.x, pending.Position.y, 0f), Quaternion.identity, _enemyParent);
            var context = new EnemySpawnContext(_target, PlayAreaController.Inflate(screen, _enemyLifeMargin), _fxParent,
                _projectileParent, PlayAreaController.Inflate(screen, _bulletMargin));
            enemy.Initialize(pending.Data, context);

            AddToCount(_pendingByTag, pending.Tag, -1);
            AddToCount(_aliveByTag, pending.Tag, 1);
            _aliveTotal++;
            string tag = pending.Tag;
            enemy.Removed += _ => OnEnemyRemoved(tag);
        }

        /// <summary>Called when an enemy of a group is destroyed or leaves the screen.</summary>
        private void OnEnemyRemoved(string tag)
        {
            if (this == null)
            {
                return; // The runner itself is being destroyed (scene change): nothing to count.
            }

            AddToCount(_aliveByTag, tag, -1);
            _aliveTotal--;
        }

        /// <summary>True when no enemy of the group is alive or waiting to appear.</summary>
        private bool IsGroupClear(string tag)
        {
            return GetCount(_aliveByTag, tag) <= 0 && GetCount(_pendingByTag, tag) <= 0;
        }

        /// <summary>Starts a smooth change of the scroll speed, from the current speed.</summary>
        private void StartScrollTransition(float targetSpeed, float duration)
        {
            if (_background == null)
            {
                return;
            }

            _scrollFrom = _background.BaseSpeed;
            _scrollTo = targetSpeed;
            _scrollDuration = duration;
            _scrollElapsed = 0f;
            if (duration <= 0f)
            {
                _background.BaseSpeed = targetSpeed;
                _scrollElapsed = -1f;
            }
        }

        /// <summary>Moves the scroll speed along the current transition, if there is one.</summary>
        private void UpdateScrollTransition(float deltaTime)
        {
            if (_scrollElapsed < 0f || _background == null)
            {
                return;
            }

            _scrollElapsed += deltaTime;
            _background.BaseSpeed = WaveMath.ComputeScrollSpeed(_scrollFrom, _scrollTo, _scrollElapsed, _scrollDuration);
            if (_scrollElapsed >= _scrollDuration)
            {
                _scrollElapsed = -1f;
            }
        }

        /// <summary>Adds a value to the count of a group.</summary>
        private static void AddToCount(Dictionary<string, int> counts, string tag, int delta)
        {
            counts.TryGetValue(tag, out int current);
            counts[tag] = current + delta;
        }

        /// <summary>Returns the count of a group, 0 if it has none.</summary>
        private static int GetCount(Dictionary<string, int> counts, string tag)
        {
            return counts.TryGetValue(tag, out int current) ? current : 0;
        }
    }
}
