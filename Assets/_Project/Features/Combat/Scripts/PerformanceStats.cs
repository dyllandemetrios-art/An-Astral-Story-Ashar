using System.Collections.Generic;

namespace Ashar.Combat
{
    /// <summary>
    /// Pure calculations on a list of frame times, used by the performance test bench (story E2-01).
    /// RESPONSIBILITIES: turn raw frame durations into the numbers the decision is based on.
    /// HOW IT WORKS: every method takes the frame times in milliseconds and returns one number. Nothing here touches
    /// Unity, so the formulas can be unit-tested and reused.
    /// WHY: an average frame rate hides stutter. The "1 % low" and the share of long frames are what a player feels,
    /// so they are measured next to the average.
    /// </summary>
    public static class PerformanceStats
    {
        /// <summary>Returns the average frame time in milliseconds, or 0 for an empty list.</summary>
        public static float AverageMilliseconds(IReadOnlyList<float> frameMilliseconds)
        {
            if (frameMilliseconds == null || frameMilliseconds.Count == 0)
            {
                return 0f;
            }

            float sum = 0f;
            for (int i = 0; i < frameMilliseconds.Count; i++)
            {
                sum += frameMilliseconds[i];
            }

            return sum / frameMilliseconds.Count;
        }

        /// <summary>Returns the longest frame time in milliseconds, or 0 for an empty list.</summary>
        public static float WorstMilliseconds(IReadOnlyList<float> frameMilliseconds)
        {
            float worst = 0f;
            if (frameMilliseconds == null)
            {
                return worst;
            }

            for (int i = 0; i < frameMilliseconds.Count; i++)
            {
                if (frameMilliseconds[i] > worst)
                {
                    worst = frameMilliseconds[i];
                }
            }

            return worst;
        }

        /// <summary>
        /// Returns the "1 % low" frame rate: the frame rate of the slowest 1 % of frames (at least one frame).
        /// A steady game has a 1 % low close to its average; a game that stutters has a much lower one.
        /// </summary>
        public static float OnePercentLowFps(IReadOnlyList<float> frameMilliseconds)
        {
            if (frameMilliseconds == null || frameMilliseconds.Count == 0)
            {
                return 0f;
            }

            var sorted = new List<float>(frameMilliseconds);
            sorted.Sort();

            int worstCount = System.Math.Max(1, sorted.Count / 100);
            float sum = 0f;
            for (int i = sorted.Count - worstCount; i < sorted.Count; i++)
            {
                sum += sorted[i];
            }

            float averageWorst = sum / worstCount;
            return averageWorst > 0f ? 1000f / averageWorst : 0f;
        }

        /// <summary>Returns the share of frames (0 to 1) that took longer than a threshold, in milliseconds.</summary>
        public static float FractionAbove(IReadOnlyList<float> frameMilliseconds, float thresholdMilliseconds)
        {
            if (frameMilliseconds == null || frameMilliseconds.Count == 0)
            {
                return 0f;
            }

            int count = 0;
            for (int i = 0; i < frameMilliseconds.Count; i++)
            {
                if (frameMilliseconds[i] > thresholdMilliseconds)
                {
                    count++;
                }
            }

            return count / (float)frameMilliseconds.Count;
        }

        /// <summary>
        /// True when the run holds 60 frames per second by the criterion of the story: the average frame is at most
        /// 16.7 ms and no more than 1 % of the frames exceed 25 ms.
        /// </summary>
        public static bool Holds60Fps(float averageMilliseconds, float fractionAbove25Ms)
        {
            return averageMilliseconds > 0f && averageMilliseconds <= 16.7f && fractionAbove25Ms <= 0.01f;
        }
    }
}
