using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Profiling;
using UnityEngine;

namespace Ashar.Combat
{
    /// <summary>
    /// Records the frames of the performance test bench (story E2-01), shows them on screen and writes the results.
    /// RESPONSIBILITIES: while a measure runs, note the duration of every frame, the memory allocated and the garbage
    /// collector passes; at the end of each phase, compute the numbers; at the end of the run, write a CSV file.
    /// HOW IT WORKS: the bench calls BeginMeasure and EndMeasure around the measured period. Between them, Update()
    /// stores each frame's duration (unscaled, in milliseconds) and the bytes allocated since the last frame. The
    /// calculations themselves are in PerformanceStats, which has no Unity code and is unit-tested.
    /// WHY: an average frame rate alone hides stutter, and stutter caused by garbage collection is exactly what the
    /// story looks for. So the reporter also keeps the worst frames, the "1 % low", and the memory allocated per frame.
    /// </summary>
    public class PerformanceReporterController : MonoBehaviour
    {
        private const float LongFrameMilliseconds = 25f; // A frame longer than this is a visible stutter (criterion of the story).

        [Header("References")]
        [SerializeField, Tooltip("The bench, read to show the current phase and bullet count on screen.")]
        private ProjectileStressTestController _bench;

        [Header("Display")]
        [SerializeField, Tooltip("Show the phase, bullet count and frame time on screen.")]
        private bool _showOverlay = true;

        [SerializeField, Min(8), Tooltip("Font size of the on-screen text, in pixels.")]
        private int _fontSize = 22;

        [Header("Output")]
        [SerializeField, Tooltip("Folder for the results, relative to the folder that holds the game (or the project, in the Editor).")]
        private string _outputFolder = "PerfResults";

        private readonly List<float> _frames = new List<float>();     // Frame durations of the current measure, in milliseconds.
        private readonly List<PhaseResult> _results = new List<PhaseResult>(); // One line per finished phase.
        private bool _measuring;          // True between BeginMeasure and EndMeasure.
        private string _label;            // Spawn mode of the current phase.
        private int _target;              // Bullet target of the current phase.
        private long _heapBefore;         // Managed heap size at the last frame, for the fallback way of counting allocations.
        private ProfilerRecorder _gcAllocRecorder; // Unity's own counter of bytes allocated in the last frame, when available.
        private long _allocatedSum;       // Bytes allocated during the current measure.
        private int _gcBefore;            // Garbage collector passes at the start of the measure.
        private long _aliveSum;           // Sum of the bullet counts of every measured frame, for the average.
        private float _smoothedMs;        // Frame time smoothed over a few frames, for the on-screen display.

        /// <summary>The numbers of one finished phase, ready to be written as a line of the CSV.</summary>
        private struct PhaseResult
        {
            public string Mode;
            public int Target;
            public int Frames;
            public float Seconds;
            public float AverageMs;
            public float AverageFps;
            public float OnePercentLowFps;
            public float WorstMs;
            public float PercentOver25Ms;
            public float AllocBytesPerFrame;
            public int GcPasses;
            public float AverageAlive;
            public bool Holds60;
        }

        /// <summary>Starts the memory counter, if this build offers it.</summary>
        private void OnEnable()
        {
            _gcAllocRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
        }

        /// <summary>Releases the memory counter.</summary>
        private void OnDisable()
        {
            _gcAllocRecorder.Dispose();
        }

        /// <summary>Starts recording the frames of a phase.</summary>
        public void BeginMeasure(string label, int targetCount)
        {
            _label = label;
            _target = targetCount;
            _frames.Clear();
            _allocatedSum = 0;
            _aliveSum = 0;
            _heapBefore = GC.GetTotalMemory(false);
            _gcBefore = GC.CollectionCount(0);
            _measuring = true;
        }

        /// <summary>Stops recording and turns the frames of the phase into one result line.</summary>
        public void EndMeasure()
        {
            _measuring = false;
            if (_frames.Count == 0)
            {
                return;
            }

            float averageMs = PerformanceStats.AverageMilliseconds(_frames);
            float over25 = PerformanceStats.FractionAbove(_frames, LongFrameMilliseconds);
            float totalSeconds = 0f;
            for (int i = 0; i < _frames.Count; i++)
            {
                totalSeconds += _frames[i] / 1000f;
            }

            _results.Add(new PhaseResult
            {
                Mode = _label,
                Target = _target,
                Frames = _frames.Count,
                Seconds = totalSeconds,
                AverageMs = averageMs,
                AverageFps = averageMs > 0f ? 1000f / averageMs : 0f,
                OnePercentLowFps = PerformanceStats.OnePercentLowFps(_frames),
                WorstMs = PerformanceStats.WorstMilliseconds(_frames),
                PercentOver25Ms = over25 * 100f,
                AllocBytesPerFrame = _allocatedSum / (float)_frames.Count,
                GcPasses = GC.CollectionCount(0) - _gcBefore,
                AverageAlive = _aliveSum / (float)_frames.Count,
                Holds60 = PerformanceStats.Holds60Fps(averageMs, over25),
            });
        }

        /// <summary>Writes every phase result to a CSV file, with a few lines describing the machine and settings.</summary>
        public void WriteResults()
        {
            string root = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string folder = Path.Combine(root, _outputFolder);
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, $"E2-01_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

            var text = new StringBuilder();
            text.AppendLine($"# Unity {Application.unityVersion}, {(Application.isEditor ? "EDITOR" : "BUILD")}, {Screen.width}x{Screen.height}, {Screen.currentResolution.refreshRateRatio.value:F0} Hz, vSync {QualitySettings.vSyncCount}");
            text.AppendLine($"# CPU {SystemInfo.processorType} ({SystemInfo.processorCount} threads), GPU {SystemInfo.graphicsDeviceName}, RAM {SystemInfo.systemMemorySize} MB");
            text.AppendLine($"# memory counter: {(_gcAllocRecorder.Valid ? "ProfilerRecorder GC Allocated In Frame" : "managed heap growth (fallback)")}");
            text.AppendLine("mode,target,frames,seconds,avgMs,avgFps,onePercentLowFps,worstMs,percentOver25ms,allocBytesPerFrame,gcPasses,avgAlive,holds60fps");
            foreach (PhaseResult r in _results)
            {
                text.AppendLine(string.Join(",",
                    r.Mode, r.Target.ToString(CultureInfo.InvariantCulture), r.Frames.ToString(CultureInfo.InvariantCulture),
                    r.Seconds.ToString("F1", CultureInfo.InvariantCulture), r.AverageMs.ToString("F2", CultureInfo.InvariantCulture),
                    r.AverageFps.ToString("F0", CultureInfo.InvariantCulture), r.OnePercentLowFps.ToString("F0", CultureInfo.InvariantCulture),
                    r.WorstMs.ToString("F1", CultureInfo.InvariantCulture), r.PercentOver25Ms.ToString("F2", CultureInfo.InvariantCulture),
                    r.AllocBytesPerFrame.ToString("F0", CultureInfo.InvariantCulture), r.GcPasses.ToString(CultureInfo.InvariantCulture),
                    r.AverageAlive.ToString("F0", CultureInfo.InvariantCulture), r.Holds60 ? "yes" : "NO"));
            }

            File.WriteAllText(path, text.ToString());
            Debug.Log($"[E2-01] Results written to {path}");
        }

        /// <summary>Records the frame that has just been drawn, while a measure is running.</summary>
        private void Update()
        {
            float milliseconds = Time.unscaledDeltaTime * 1000f;
            _smoothedMs = Mathf.Lerp(_smoothedMs, milliseconds, 0.05f);

            if (!_measuring)
            {
                return;
            }

            // Bytes allocated this frame: Unity's counter if it works in this build, otherwise the growth of the managed
            // heap (a drop means the collector ran, so only growth is counted; a slight under-estimate).
            long heap = GC.GetTotalMemory(false);
            if (_gcAllocRecorder.Valid)
            {
                _allocatedSum += _gcAllocRecorder.LastValue;
            }
            else if (heap > _heapBefore)
            {
                _allocatedSum += heap - _heapBefore;
            }

            _heapBefore = heap;

            _frames.Add(milliseconds);
            _aliveSum += _bench != null ? _bench.Alive : 0;
        }

        /// <summary>Draws the phase, the bullet count and the frame time in the corner of the screen.</summary>
        private void OnGUI()
        {
            if (!_showOverlay || _bench == null)
            {
                return;
            }

            var style = new GUIStyle(GUI.skin.label) { fontSize = _fontSize, fontStyle = FontStyle.Bold };
            style.normal.textColor = Color.white;
            string text = $"{_bench.Status}\nbullets: {_bench.Alive}   frame: {_smoothedMs:F1} ms ({(_smoothedMs > 0f ? 1000f / _smoothedMs : 0f):F0} fps)";
            GUI.Label(new Rect(12f, 8f, Screen.width - 24f, _fontSize * 3.5f), text, style);
        }
    }
}
