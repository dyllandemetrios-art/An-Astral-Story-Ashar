using Ashar.Player;
using NUnit.Framework;
using UnityEngine;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the pure calculations of the dash: its direction, when it may start, and where it ends.
    /// WHY: "the dash never crosses the limits" and "no dash during the cooldown" are acceptance criteria,
    /// and both come from small formulas that are cheap to check without playing.
    /// </summary>
    public class DashMathTests
    {
        private const float Delta = 0.0001f;                                            // Tolerance for float comparisons.
        private static readonly Rect PlayArea = new Rect(-9.3f, -4.925f, 18.6f, 9.85f); // Default play area (spec §4).

        /// <summary>Without any input the dash goes straight up.</summary>
        [Test]
        public void ComputeDashDirection_NoInput_IsUp()
        {
            Vector2 direction = PlayerDashController.ComputeDashDirection(Vector2.zero);

            Assert.AreEqual(0f, direction.x, Delta);
            Assert.AreEqual(1f, direction.y, Delta);
        }

        /// <summary>A tiny stick drift counts as no input, so the dash still goes up.</summary>
        [Test]
        public void ComputeDashDirection_StickDrift_IsUp()
        {
            Vector2 direction = PlayerDashController.ComputeDashDirection(new Vector2(0.05f, -0.03f));

            Assert.AreEqual(Vector2.up, direction);
        }

        /// <summary>A diagonal input gives a unit-length direction, so a diagonal dash is not longer.</summary>
        [Test]
        public void ComputeDashDirection_Diagonal_IsNormalised()
        {
            Vector2 direction = PlayerDashController.ComputeDashDirection(new Vector2(1f, -1f));

            Assert.AreEqual(1f, direction.magnitude, Delta);
            Assert.Greater(direction.x, 0f);
            Assert.Less(direction.y, 0f);
        }

        /// <summary>A half-pushed stick still gives a full-length dash: only the direction matters.</summary>
        [Test]
        public void ComputeDashDirection_HalfStick_IsNormalised()
        {
            Vector2 direction = PlayerDashController.ComputeDashDirection(new Vector2(0.5f, 0f));

            Assert.AreEqual(1f, direction.magnitude, Delta);
        }

        /// <summary>A dash may start only when the cooldown is over and no dash is running.</summary>
        [TestCase(0f, false, true)]
        [TestCase(0.5f, false, false)]
        [TestCase(0f, true, false)]
        [TestCase(0.5f, true, false)]
        public void CanStartDash_Combinations_ReturnsExpected(float cooldownLeft, bool isDashing, bool expected)
        {
            Assert.AreEqual(expected, PlayerDashController.CanStartDash(cooldownLeft, isDashing));
        }

        /// <summary>Over the whole dash time, the ship covers exactly the dash distance (2.8 u in 0.18 s).</summary>
        [Test]
        public void ComputeDashPosition_FullDash_CoversDashDistance()
        {
            float speed = 2.8f / 0.18f;

            Vector2 end = PlayerDashController.ComputeDashPosition(Vector2.zero, Vector2.up, speed, 0.18f, PlayArea);

            Assert.AreEqual(2.8f, end.y, Delta);
        }

        /// <summary>A dash towards an edge stops on the edge: it never crosses the limits.</summary>
        [Test]
        public void ComputeDashPosition_TowardsEdge_StopsOnEdge()
        {
            float speed = 2.8f / 0.18f;

            Vector2 end = PlayerDashController.ComputeDashPosition(new Vector2(8.5f, 0f), Vector2.right, speed, 0.18f, PlayArea);

            Assert.AreEqual(PlayArea.xMax, end.x, Delta);
        }
    }
}
