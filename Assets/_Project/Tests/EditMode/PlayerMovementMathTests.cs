using Ashar.Player;
using NUnit.Framework;
using UnityEngine;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the movement calculation of the player ship.
    /// WHY: "diagonals are not faster" and "the ship never leaves the play area" are acceptance criteria of the
    /// story, and both come from pure formulas that are cheap to check without playing.
    /// </summary>
    public class PlayerMovementMathTests
    {
        private const float Delta = 0.0001f;                           // Tolerance for float comparisons.
        private static readonly Rect PlayArea = new Rect(-9.3f, -4.925f, 18.6f, 9.85f); // Default play area (spec §4).

        /// <summary>Straight movement travels exactly the ship speed.</summary>
        [Test]
        public void ComputeVelocity_FullRight_EqualsSpeed()
        {
            Vector2 velocity = PlayerMovementController.ComputeVelocity(Vector2.right, 9f);

            Assert.AreEqual(9f, velocity.magnitude, Delta);
        }

        /// <summary>A raw (1,1) input would be 1.41 times faster; it must be brought back to the ship speed.</summary>
        [Test]
        public void ComputeVelocity_RawDiagonal_IsNotFasterThanStraight()
        {
            Vector2 velocity = PlayerMovementController.ComputeVelocity(new Vector2(1f, 1f), 9f);

            Assert.AreEqual(9f, velocity.magnitude, Delta);
        }

        /// <summary>A gamepad stick pushed halfway gives half the speed.</summary>
        [Test]
        public void ComputeVelocity_HalfStick_GivesHalfSpeed()
        {
            Vector2 velocity = PlayerMovementController.ComputeVelocity(new Vector2(0.5f, 0f), 9f);

            Assert.AreEqual(4.5f, velocity.magnitude, Delta);
        }

        /// <summary>With no input the ship does not move.</summary>
        [Test]
        public void ComputeVelocity_NoInput_IsZero()
        {
            Vector2 velocity = PlayerMovementController.ComputeVelocity(Vector2.zero, 9f);

            Assert.AreEqual(0f, velocity.magnitude, Delta);
        }

        /// <summary>One frame of movement inside the play area simply adds speed x time.</summary>
        [Test]
        public void ComputeNextPosition_InsideArea_MovesBySpeedTimesTime()
        {
            Vector2 next = PlayerMovementController.ComputeNextPosition(Vector2.zero, Vector2.up, 9f, 0.1f, PlayArea);

            Assert.AreEqual(0f, next.x, Delta);
            Assert.AreEqual(0.9f, next.y, Delta);
        }

        /// <summary>Pushing against an edge stops the ship on that edge, never past it.</summary>
        [Test]
        public void ComputeNextPosition_AgainstRightEdge_StaysOnEdge()
        {
            Vector2 next = PlayerMovementController.ComputeNextPosition(new Vector2(9.2f, 0f), Vector2.right, 9f, 0.5f, PlayArea);

            Assert.AreEqual(PlayArea.xMax, next.x, Delta);
        }

        /// <summary>A very long frame (a hitch) must not throw the ship out of the play area.</summary>
        [Test]
        public void ComputeNextPosition_VeryLongFrame_StillInsideArea()
        {
            Vector2 next = PlayerMovementController.ComputeNextPosition(Vector2.zero, new Vector2(-1f, -1f), 9f, 10f, PlayArea);

            Assert.AreEqual(PlayArea.xMin, next.x, Delta);
            Assert.AreEqual(PlayArea.yMin, next.y, Delta);
        }
    }
}
