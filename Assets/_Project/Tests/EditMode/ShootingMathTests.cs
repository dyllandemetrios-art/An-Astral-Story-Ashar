using Ashar.Combat;
using Ashar.Environment;
using Ashar.Player;
using NUnit.Framework;
using UnityEngine;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the timing of the main weapon and the flight of a projectile.
    /// WHY: "the fire rate is exact" and "no projectile survives off screen" are acceptance criteria, and both
    /// come from pure formulas that are cheap to check without playing.
    /// </summary>
    public class ShootingMathTests
    {
        private const float Delta = 0.0001f; // Tolerance for float comparisons.
        private const float Interval = 0.1f; // 10 shots per second.
        private const int Cap = 4;           // Same safety cap as the controller.

        /// <summary>The first frame with the button held fires at once, without waiting for a cooldown.</summary>
        [Test]
        public void ConsumeShots_FirstFrameHeld_FiresImmediately()
        {
            float cooldown = 0f;

            int shots = PlayerShootController.ConsumeShots(ref cooldown, 0.016f, Interval, true, Cap);

            Assert.AreEqual(1, shots);
        }

        /// <summary>With the button up, nothing is fired.</summary>
        [Test]
        public void ConsumeShots_ButtonUp_FiresNothing()
        {
            float cooldown = 0f;

            int shots = PlayerShootController.ConsumeShots(ref cooldown, 0.016f, Interval, false, Cap);

            Assert.AreEqual(0, shots);
        }

        /// <summary>Held for one second at 60 frames per second, the weapon fires 10 times: the rate is exact.</summary>
        [Test]
        public void ConsumeShots_HeldOneSecond_FiresTenTimes()
        {
            float cooldown = 0f;
            int total = 0;

            for (int frame = 0; frame < 60; frame++)
            {
                total += PlayerShootController.ConsumeShots(ref cooldown, 1f / 60f, Interval, true, Cap);
            }

            Assert.AreEqual(10, total);
        }

        /// <summary>The rate does not depend on the frame rate: 30 frames per second also gives 10 shots.</summary>
        [Test]
        public void ConsumeShots_HeldOneSecondAtLowFrameRate_StillFiresTenTimes()
        {
            float cooldown = 0f;
            int total = 0;

            for (int frame = 0; frame < 30; frame++)
            {
                total += PlayerShootController.ConsumeShots(ref cooldown, 1f / 30f, Interval, true, Cap);
            }

            Assert.AreEqual(10, total);
        }

        /// <summary>Releasing the button keeps the cooldown, so pressing again at once does not fire a second bullet.</summary>
        [Test]
        public void ConsumeShots_ReleasedThenPressedAgain_RespectsCooldown()
        {
            float cooldown = 0f;
            PlayerShootController.ConsumeShots(ref cooldown, 0.016f, Interval, true, Cap);      // First shot.
            PlayerShootController.ConsumeShots(ref cooldown, 0.016f, Interval, false, Cap);     // Released.

            int shots = PlayerShootController.ConsumeShots(ref cooldown, 0.016f, Interval, true, Cap); // Pressed again 32 ms later.

            Assert.AreEqual(0, shots);
        }

        /// <summary>One very long frame cannot flood the scene: the number of shots is capped and the backlog dropped.</summary>
        [Test]
        public void ConsumeShots_VeryLongFrame_IsCappedAndBacklogDropped()
        {
            float cooldown = Interval; // A shot has just been fired, so the cooldown is running.

            int shots = PlayerShootController.ConsumeShots(ref cooldown, 5f, Interval, true, Cap);
            int next = PlayerShootController.ConsumeShots(ref cooldown, 0.016f, Interval, true, Cap);

            Assert.AreEqual(Cap, shots); // 5 s at 10 shots per second would be 50 shots; only the cap is fired.
            Assert.AreEqual(1, next);    // The backlog is dropped: the next frame fires one normal shot, not 4 more.
        }

        /// <summary>A fire interval of 0 or less fires nothing instead of looping forever.</summary>
        [Test]
        public void ConsumeShots_ZeroInterval_FiresNothing()
        {
            float cooldown = 0f;

            int shots = PlayerShootController.ConsumeShots(ref cooldown, 0.016f, 0f, true, Cap);

            Assert.AreEqual(0, shots);
        }

        /// <summary>A projectile flies in a straight line: position + direction x speed x time.</summary>
        [Test]
        public void ComputeNextPosition_StraightUp_MovesBySpeedTimesTime()
        {
            Vector2 next = ProjectileController.ComputeNextPosition(new Vector2(1f, 2f), Vector2.up, 22.5f, 0.1f);

            Assert.AreEqual(1f, next.x, Delta);
            Assert.AreEqual(4.25f, next.y, Delta);
        }

        /// <summary>A point inside the life bounds is kept; a point beyond an edge is out.</summary>
        [Test]
        public void IsOutside_InsideAndBeyondEdge_ReturnsExpected()
        {
            var bounds = new Rect(-10f, -5f, 20f, 10f);

            Assert.IsFalse(ProjectileController.IsOutside(bounds, new Vector2(0f, 4.9f)));
            Assert.IsTrue(ProjectileController.IsOutside(bounds, new Vector2(0f, 5.1f)));
        }

        /// <summary>Growing a rectangle by a margin adds it on every side.</summary>
        [Test]
        public void Inflate_AddsMarginOnEverySide()
        {
            Rect inflated = PlayAreaController.Inflate(new Rect(-10f, -5.625f, 20f, 11.25f), 1f);

            Assert.AreEqual(-11f, inflated.xMin, Delta);
            Assert.AreEqual(11f, inflated.xMax, Delta);
            Assert.AreEqual(-6.625f, inflated.yMin, Delta);
            Assert.AreEqual(6.625f, inflated.yMax, Delta);
        }
    }
}
