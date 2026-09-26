using Ashar.Combat;
using NUnit.Framework;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the calculations of the performance test bench (story E2-01).
    /// WHY: the decision on pooling rests on these numbers, so the formulas must be right before any measure is trusted.
    /// </summary>
    public class PerformanceStatsTests
    {
        private const float Delta = 0.001f; // Tolerance for float comparisons.

        /// <summary>The average of a few frame times is their mean.</summary>
        [Test]
        public void AverageMilliseconds_SimpleList_IsMean()
        {
            Assert.AreEqual(10f, PerformanceStats.AverageMilliseconds(new[] { 8f, 10f, 12f }), Delta);
        }

        /// <summary>An empty list gives 0 instead of dividing by zero.</summary>
        [Test]
        public void AverageMilliseconds_EmptyList_IsZero()
        {
            Assert.AreEqual(0f, PerformanceStats.AverageMilliseconds(new float[0]), Delta);
        }

        /// <summary>The worst frame is the longest one.</summary>
        [Test]
        public void WorstMilliseconds_ReturnsLongestFrame()
        {
            Assert.AreEqual(40f, PerformanceStats.WorstMilliseconds(new[] { 8f, 40f, 12f }), Delta);
        }

        /// <summary>A perfectly steady 10 ms game has a 1 % low of 100 fps, equal to its average.</summary>
        [Test]
        public void OnePercentLowFps_SteadyFrames_EqualsAverageFps()
        {
            var frames = new float[200];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = 10f;
            }

            Assert.AreEqual(100f, PerformanceStats.OnePercentLowFps(frames), Delta);
        }

        /// <summary>Two long frames out of 200 (1 %) pull the 1 % low well below the average frame rate.</summary>
        [Test]
        public void OnePercentLowFps_WithStutter_IsMuchLowerThanAverage()
        {
            var frames = new float[200];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = 10f;
            }

            frames[50] = 50f;
            frames[150] = 50f;

            Assert.AreEqual(20f, PerformanceStats.OnePercentLowFps(frames), Delta); // Worst 2 frames average 50 ms = 20 fps.
        }

        /// <summary>The share of frames above a threshold is counted exactly.</summary>
        [Test]
        public void FractionAbove_CountsFramesOverThreshold()
        {
            Assert.AreEqual(0.25f, PerformanceStats.FractionAbove(new[] { 10f, 30f, 12f, 11f }, 25f), Delta);
        }

        /// <summary>The 60 fps criterion: average at most 16.7 ms and at most 1 % of frames over 25 ms.</summary>
        [TestCase(10f, 0f, true)]
        [TestCase(16.7f, 0.01f, true)]
        [TestCase(17f, 0f, false)]
        [TestCase(10f, 0.05f, false)]
        [TestCase(0f, 0f, false)]
        public void Holds60Fps_Combinations_ReturnsExpected(float averageMs, float overShare, bool expected)
        {
            Assert.AreEqual(expected, PerformanceStats.Holds60Fps(averageMs, overShare));
        }
    }
}
