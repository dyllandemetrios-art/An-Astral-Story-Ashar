using Ashar.Enemies;
using NUnit.Framework;
using UnityEngine;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the firing clock and the bullet directions of the enemy patterns (spec §7.4).
    /// WHY: how often an enemy fires and where its bullets go is what makes a fight fair or unfair, and both are pure
    /// calculations that are cheap to check exactly without playing.
    /// </summary>
    public class FirePatternTests
    {
        private const float Delta = 0.0001f; // Tolerance for float comparisons.
        private const float Frame = 0.01f;   // Frame time used to step the timer in the tests.

        /// <summary>Runs the timer for a number of seconds and returns the times at which it asked to fire.</summary>
        private static System.Collections.Generic.List<float> FireTimes(FireSequenceTimer timer, float seconds)
        {
            var times = new System.Collections.Generic.List<float>();
            int frames = Mathf.RoundToInt(seconds / Frame);
            for (int i = 1; i <= frames; i++)
            {
                if (timer.Advance(Frame) == FireSequenceEvent.Fire)
                {
                    times.Add(i * Frame);
                }
            }

            return times;
        }

        /// <summary>A pattern with an interval of 2 s fires every 2 s, the first time after 2 s.</summary>
        [Test]
        public void Timer_SimpleInterval_FiresEveryInterval()
        {
            var timer = new FireSequenceTimer(2f, 0f, 1, 0.1f);

            var times = FireTimes(timer, 6.05f);

            Assert.AreEqual(3, times.Count);
            Assert.AreEqual(2f, times[0], 0.02f);
            Assert.AreEqual(4f, times[1], 0.02f);
            Assert.AreEqual(6f, times[2], 0.02f);
        }

        /// <summary>Restarting with a skip shortens only the first wait, so enemies created together do not fire in step.</summary>
        [Test]
        public void Timer_RestartWithSkip_FirstShotComesEarlier()
        {
            var timer = new FireSequenceTimer(2f, 0f, 1, 0.1f);
            timer.Restart(0.5f);

            var times = FireTimes(timer, 4.05f);

            Assert.AreEqual(1.5f, times[0], 0.02f);
            Assert.AreEqual(3.5f, times[1], 0.02f);
        }

        /// <summary>With a warning time, the warning starts that long before each shot, and the period stays the interval.</summary>
        [Test]
        public void Timer_WithTelegraph_WarnsThenFiresAtTheSamePeriod()
        {
            var timer = new FireSequenceTimer(3f, 1f, 1, 0.1f);
            float warningStart = -1f;
            float firstFire = -1f;
            for (int i = 1; i <= 500 && firstFire < 0f; i++)
            {
                FireSequenceEvent fireEvent = timer.Advance(Frame);
                if (fireEvent == FireSequenceEvent.TelegraphStarted)
                {
                    warningStart = i * Frame;
                }
                else if (fireEvent == FireSequenceEvent.Fire)
                {
                    firstFire = i * Frame;
                }
            }

            Assert.AreEqual(2f, warningStart, 0.02f);  // 3 s interval - 1 s warning.
            Assert.AreEqual(3f, firstFire, 0.02f);     // The shot comes 1 s after the warning starts.
        }

        /// <summary>The warning is on between its start and the shot, and off otherwise.</summary>
        [Test]
        public void Timer_IsTelegraphing_OnlyBetweenWarningAndShot()
        {
            var timer = new FireSequenceTimer(3f, 1f, 1, 0.1f);

            for (int i = 0; i < 150; i++)
            {
                timer.Advance(Frame);
            }

            Assert.IsFalse(timer.IsTelegraphing);   // 1.5 s: still waiting.

            for (int i = 0; i < 80; i++)
            {
                timer.Advance(Frame);
            }

            Assert.IsTrue(timer.IsTelegraphing);    // 2.3 s: warning.
            Assert.Greater(timer.TelegraphProgress, 0.2f);
            Assert.Less(timer.TelegraphProgress, 0.5f);
        }

        /// <summary>A burst of 3 fires 3 times, 0.1 s apart, and the whole burst repeats at the interval.</summary>
        [Test]
        public void Timer_Burst_FiresCountTimesThenRepeatsAtInterval()
        {
            var timer = new FireSequenceTimer(3f, 0f, 3, 0.1f);

            var times = FireTimes(timer, 6.35f);

            Assert.AreEqual(6, times.Count);
            Assert.AreEqual(0.1f, times[1] - times[0], 0.02f);
            Assert.AreEqual(0.1f, times[2] - times[1], 0.02f);
            Assert.AreEqual(3f, times[3] - times[0], 0.03f); // The second burst starts one interval after the first.
        }

        /// <summary>A fan of three spread over 30 degrees points at -15, 0 and +15 degrees from its middle.</summary>
        [Test]
        public void FanOffsetDegrees_ThreeBulletsOverThirtyDegrees()
        {
            Assert.AreEqual(-15f, EnemyShootController.FanOffsetDegrees(0, 3, 30f), Delta);
            Assert.AreEqual(0f, EnemyShootController.FanOffsetDegrees(1, 3, 30f), Delta);
            Assert.AreEqual(15f, EnemyShootController.FanOffsetDegrees(2, 3, 30f), Delta);
        }

        /// <summary>A fan of one bullet points at its middle.</summary>
        [Test]
        public void FanOffsetDegrees_SingleBullet_IsZero()
        {
            Assert.AreEqual(0f, EnemyShootController.FanOffsetDegrees(0, 1, 90f), Delta);
        }

        /// <summary>The spiral turns by a fixed step at each shot, starting straight down.</summary>
        [Test]
        public void SpiralAngleDegrees_TurnsByStepEachShot()
        {
            Assert.AreEqual(0f, EnemyShootController.SpiralAngleDegrees(0, 15f), Delta);
            Assert.AreEqual(45f, EnemyShootController.SpiralAngleDegrees(3, 15f), Delta);
        }

        /// <summary>Turning "down" by 90 degrees anticlockwise gives "right", and a turn keeps the length of the direction.</summary>
        [Test]
        public void Rotate_DownByNinetyDegrees_IsRight()
        {
            Vector2 rotated = EnemyShootController.Rotate(Vector2.down, 90f);

            Assert.AreEqual(1f, rotated.x, Delta);
            Assert.AreEqual(0f, rotated.y, Delta);
            Assert.AreEqual(1f, EnemyShootController.Rotate(new Vector2(0.6f, -0.8f), 33f).magnitude, Delta);
        }
    }
}
