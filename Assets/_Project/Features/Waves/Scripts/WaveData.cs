using System;
using System.Collections.Generic;
using Ashar.Enemies;
using UnityEngine;

namespace Ashar.Waves
{
    /// <summary>The kinds of event a wave timeline can contain (spec §6). Dialogue, Zone and Boss are added by their own stories.</summary>
    public enum WaveEventType
    {
        /// <summary>Creates a group of enemies.</summary>
        Spawn,

        /// <summary>Pauses the timeline until every enemy of a group has been destroyed or has left the screen.</summary>
        WaitForClear,

        /// <summary>Changes the scrolling speed of the background, smoothly.</summary>
        ScrollSpeed,

        /// <summary>Ends the mission.</summary>
        End,
    }

    /// <summary>How a group of enemies is laid out when it appears.</summary>
    public enum WaveFormation
    {
        /// <summary>One enemy.</summary>
        Single,

        /// <summary>Enemies side by side along the edge they enter from.</summary>
        Line,

        /// <summary>Enemies in a V shape, the middle one leading.</summary>
        V,

        /// <summary>Enemies one behind the other: they share the same entry point and are separated by the delay between units.</summary>
        Column,
    }

    /// <summary>Where a group of enemies enters the screen.</summary>
    public enum WaveEntry
    {
        /// <summary>Above the top edge of the screen.</summary>
        Top,

        /// <summary>Beside the left edge of the screen.</summary>
        Left,

        /// <summary>Beside the right edge of the screen.</summary>
        Right,

        /// <summary>Directly on the screen (for enemies that do not move, such as turrets).</summary>
        Screen,
    }

    /// <summary>
    /// One line of a wave table: when it happens, what it is, and its settings. Only the settings of its own type are used.
    /// </summary>
    [Serializable]
    public class WaveEvent
    {
        [SerializeField, Min(0f), Tooltip("Seconds since the start of the mission, or since the last WaitForClear that ended, when this event happens. Keep the list in increasing order of time.")]
        private float _time;

        [SerializeField, Tooltip("What kind of event this is. Only the settings under its own heading are used.")]
        private WaveEventType _type = WaveEventType.Spawn;

        [Header("Spawn")]
        [SerializeField, Tooltip("Spawn: the kind of enemy to create.")]
        private EnemyData _enemy;

        [SerializeField, Min(1), Tooltip("Spawn: how many enemies.")]
        private int _count = 1;

        [SerializeField, Tooltip("Spawn: how the group is laid out.")]
        private WaveFormation _formation = WaveFormation.Single;

        [SerializeField, Tooltip("Spawn: where the group enters the screen.")]
        private WaveEntry _entry = WaveEntry.Top;

        [SerializeField, Tooltip("Spawn: shifts the entry point. Top and Screen: sideways from the middle of the screen. Left and Right: up (positive) or down from the middle, in world units.")]
        private float _entryOffset;

        [SerializeField, Min(0f), Tooltip("Spawn, entry Screen only: how far below the top edge of the screen the enemies appear, in world units.")]
        private float _entryDepth = 2.5f;

        [SerializeField, Min(0.1f), Tooltip("Spawn: distance between neighbours in a Line or a V, in world units.")]
        private float _spacing = 1.8f;

        [SerializeField, Min(0f), Tooltip("Spawn: seconds between one enemy and the next of the group. Needed to separate a Column.")]
        private float _delayBetweenUnits = 0.3f;

        [SerializeField, Tooltip("Spawn and WaitForClear: name of the group. A WaitForClear waits for the enemies of the group with the same name.")]
        private string _groupTag = "";

        [Header("Scroll speed")]
        [SerializeField, Min(0f), Tooltip("ScrollSpeed: the scroll speed to reach, in world units per second.")]
        private float _targetSpeed = 4f;

        [SerializeField, Min(0f), Tooltip("ScrollSpeed: seconds taken to reach it. 0 = at once.")]
        private float _transitionDuration = 2f;

        /// <summary>Seconds since the start of the block when this event happens.</summary>
        public float Time => _time;

        /// <summary>What kind of event this is.</summary>
        public WaveEventType Type => _type;

        /// <summary>Spawn: the kind of enemy.</summary>
        public EnemyData Enemy => _enemy;

        /// <summary>Spawn: how many enemies.</summary>
        public int Count => _count;

        /// <summary>Spawn: how the group is laid out.</summary>
        public WaveFormation Formation => _formation;

        /// <summary>Spawn: where the group enters the screen.</summary>
        public WaveEntry Entry => _entry;

        /// <summary>Spawn: shift of the entry point.</summary>
        public float EntryOffset => _entryOffset;

        /// <summary>Spawn, entry Screen: depth below the top edge.</summary>
        public float EntryDepth => _entryDepth;

        /// <summary>Spawn: distance between neighbours.</summary>
        public float Spacing => _spacing;

        /// <summary>Spawn: seconds between two enemies of the group.</summary>
        public float DelayBetweenUnits => _delayBetweenUnits;

        /// <summary>Spawn and WaitForClear: name of the group.</summary>
        public string GroupTag => _groupTag;

        /// <summary>ScrollSpeed: the speed to reach.</summary>
        public float TargetSpeed => _targetSpeed;

        /// <summary>ScrollSpeed: seconds taken to reach it.</summary>
        public float TransitionDuration => _transitionDuration;
    }

    /// <summary>
    /// A timeline of events for one part of a mission: what appears, when, and when to wait.
    /// RESPONSIBILITIES: hold the wave table so that a level is written as data, and can be read as a table.
    /// HOW IT WORKS: this is a ScriptableObject asset (WaveData_...). The MissionRunnerController plays its events in order.
    /// Times are in seconds and run from the start of the mission, or from the end of the last WaitForClear, so blocks can
    /// be reordered without recalculating every time (spec §6).
    /// PATTERN: data-driven design; the level design lives in assets, not in code.
    /// </summary>
    [CreateAssetMenu(fileName = "WaveData_New", menuName = "Ashar/Wave Data")]
    public class WaveData : ScriptableObject
    {
        [SerializeField, Tooltip("The events, in order. Times must not go back inside a block (after a WaitForClear, times start again from 0).")]
        private List<WaveEvent> _events = new List<WaveEvent>();

        /// <summary>The events of the timeline, in order.</summary>
        public IReadOnlyList<WaveEvent> Events => _events;

        /// <summary>Warns in the console when the times of a block go back, which would make an event wait for the next one.</summary>
        private void OnValidate()
        {
            int line = FindTimeOrderProblem(_events);
            if (line >= 0)
            {
                Debug.LogWarning($"{name}: event {line} happens before the event just above it in the same block. Put the events of a block in increasing order of time.", this);
            }
        }

        /// <summary>
        /// Returns the index of the first event whose time is earlier than the previous event of the same block, or -1
        /// if the order is right. A block ends at a WaitForClear: after it, times start again from 0.
        /// Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static int FindTimeOrderProblem(IReadOnlyList<WaveEvent> events)
        {
            float previous = 0f;
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].Time < previous)
                {
                    return i;
                }

                previous = events[i].Type == WaveEventType.WaitForClear ? 0f : events[i].Time;
            }

            return -1;
        }
    }
}
