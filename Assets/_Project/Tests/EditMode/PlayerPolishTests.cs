using Ashar.Player;
using NUnit.Framework;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the ship banking calculation: the tilt angle it asks for, and how it eases towards it.
    /// WHY: "leans right when moving right" is easy to get backwards (a sign mistake), and the easing must never
    /// overshoot its target, so both are cheap to pin down here without playing.
    /// </summary>
    public class PlayerPolishTests
    {
        private const float Delta = 0.0001f; // Tolerance for float comparisons.

        /// <summary>Full input gives the full tilt angle, leaning towards the side of the input.</summary>
        [Test]
        public void ComputeTargetBankAngle_FullInput_IsFullAngle()
        {
            Assert.AreEqual(-14f, PlayerBankController.ComputeTargetBankAngle(1f, 14f), Delta);
            Assert.AreEqual(14f, PlayerBankController.ComputeTargetBankAngle(-1f, 14f), Delta);
        }

        /// <summary>No horizontal input means no tilt.</summary>
        [Test]
        public void ComputeTargetBankAngle_NoInput_IsZero()
        {
            Assert.AreEqual(0f, PlayerBankController.ComputeTargetBankAngle(0f, 14f), Delta);
        }

        /// <summary>An input beyond 1 (should not normally happen) is clamped, never over-tilting the ship.</summary>
        [Test]
        public void ComputeTargetBankAngle_InputBeyondOne_IsClamped()
        {
            Assert.AreEqual(-14f, PlayerBankController.ComputeTargetBankAngle(2.5f, 14f), Delta);
        }

        /// <summary>The angle moves towards its target by the allowed step, without passing it.</summary>
        [Test]
        public void ComputeNextAngle_StepsTowardsTargetWithoutOvershooting()
        {
            Assert.AreEqual(-5f, PlayerBankController.ComputeNextAngle(0f, -14f, 5f), Delta);
            Assert.AreEqual(-14f, PlayerBankController.ComputeNextAngle(-12f, -14f, 5f), Delta); // Would overshoot to -17: clamped.
        }

        /// <summary>Once at the target, the angle stays there.</summary>
        [Test]
        public void ComputeNextAngle_AtTarget_StaysThere()
        {
            Assert.AreEqual(-14f, PlayerBankController.ComputeNextAngle(-14f, -14f, 5f), Delta);
        }
    }
}
