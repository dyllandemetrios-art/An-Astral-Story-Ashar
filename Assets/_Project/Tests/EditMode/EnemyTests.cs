using Ashar.Combat;
using Ashar.Enemies;
using NUnit.Framework;
using UnityEngine;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the pure calculations of the generic enemy: movement paths, damage, hit flash and explosion frames.
    /// WHY: an enemy is data plus a few formulas. If the formulas are right, a new enemy that only changes numbers in an
    /// asset cannot break the game, and the paths never drift because they are computed from the time alone.
    /// </summary>
    public class EnemyTests
    {
        private const float Delta = 0.0001f; // Tolerance for float comparisons.

        /// <summary>Straight and sine enemies go down; static and orbiting ones do not go anywhere.</summary>
        [Test]
        public void ChooseDirection_DefaultTypes_GoDownOrNowhere()
        {
            Assert.AreEqual(Vector2.down, EnemyMovement.ChooseDirection(EnemyMovementType.Straight, Vector2.zero, Vector2.zero, false));
            Assert.AreEqual(Vector2.down, EnemyMovement.ChooseDirection(EnemyMovementType.Sine, Vector2.zero, Vector2.zero, false));
            Assert.AreEqual(Vector2.zero, EnemyMovement.ChooseDirection(EnemyMovementType.Static, Vector2.zero, Vector2.zero, false));
            Assert.AreEqual(Vector2.zero, EnemyMovement.ChooseDirection(EnemyMovementType.Orbit, Vector2.zero, Vector2.zero, false));
        }

        /// <summary>A dive heads for the target position, with a unit-length direction.</summary>
        [Test]
        public void ChooseDirection_Dive_PointsAtTarget()
        {
            Vector2 direction = EnemyMovement.ChooseDirection(EnemyMovementType.Dive, new Vector2(0f, 6f), new Vector2(3f, 2f), true);

            Assert.AreEqual(1f, direction.magnitude, Delta);
            Assert.AreEqual(0.6f, direction.x, Delta); // (3, -4) / 5.
            Assert.AreEqual(-0.8f, direction.y, Delta);
        }

        /// <summary>A dive with no target (or on top of it) falls straight down instead of dividing by zero.</summary>
        [Test]
        public void ChooseDirection_DiveWithoutTarget_FallsDown()
        {
            Assert.AreEqual(Vector2.down, EnemyMovement.ChooseDirection(EnemyMovementType.Dive, new Vector2(1f, 1f), Vector2.zero, false));
            Assert.AreEqual(Vector2.down, EnemyMovement.ChooseDirection(EnemyMovementType.Dive, new Vector2(1f, 1f), new Vector2(1f, 1f), true));
        }

        /// <summary>A sweep goes away from the side it came from.</summary>
        [Test]
        public void ChooseDirection_LateralSweep_CrossesTheScreen()
        {
            Assert.AreEqual(Vector2.left, EnemyMovement.ChooseDirection(EnemyMovementType.LateralSweep, new Vector2(11f, 2f), Vector2.zero, false));
            Assert.AreEqual(Vector2.right, EnemyMovement.ChooseDirection(EnemyMovementType.LateralSweep, new Vector2(-11f, 2f), Vector2.zero, false));
        }

        /// <summary>A straight enemy covers speed x time along its direction.</summary>
        [Test]
        public void ComputePosition_Straight_MovesBySpeedTimesTime()
        {
            Vector2 position = EnemyMovement.ComputePosition(EnemyMovementType.Straight, new Vector2(1f, 6f), Vector2.down, 4.5f, 0f, 0f, 2f);

            Assert.AreEqual(1f, position.x, Delta);
            Assert.AreEqual(-3f, position.y, Delta);
        }

        /// <summary>A sine enemy swings sideways by its amplitude a quarter of a period after it appears.</summary>
        [Test]
        public void ComputePosition_Sine_SwingsByAmplitudeAtQuarterPeriod()
        {
            // Frequency 0.5 per second: a quarter period is 0.5 s. Going down, "sideways" is to the left (-x).
            Vector2 position = EnemyMovement.ComputePosition(EnemyMovementType.Sine, Vector2.zero, Vector2.down, 4f, 1.5f, 0.5f, 0.5f);

            Assert.AreEqual(-2f, position.y, Delta);
            Assert.AreEqual(1.5f, Mathf.Abs(position.x), Delta);
        }

        /// <summary>A dive does not swing, even if its data has an amplitude.</summary>
        [Test]
        public void ComputePosition_Dive_IgnoresSwing()
        {
            Vector2 position = EnemyMovement.ComputePosition(EnemyMovementType.Dive, Vector2.zero, Vector2.down, 13.5f, 2f, 1f, 0.25f);

            Assert.AreEqual(0f, position.x, Delta);
        }

        /// <summary>An orbiting enemy starts at its start point and always stays one radius from the circle centre.</summary>
        [Test]
        public void ComputePosition_Orbit_StaysOnCircleThroughStart()
        {
            var start = new Vector2(2f, 3f);
            const float radius = 1.5f;
            // The enemy starts at the rightmost point of its circle, so the circle is centred one radius to the left.
            var centre = new Vector2(start.x - radius, start.y);

            Vector2 atStart = EnemyMovement.ComputePosition(EnemyMovementType.Orbit, start, Vector2.zero, 2.25f, radius, 0f, 0f);
            Assert.AreEqual(start.x, atStart.x, Delta);
            Assert.AreEqual(start.y, atStart.y, Delta);

            for (float time = 0.3f; time < 6f; time += 0.7f)
            {
                Vector2 position = EnemyMovement.ComputePosition(EnemyMovementType.Orbit, start, Vector2.zero, 2.25f, radius, 0f, time);
                Assert.AreEqual(radius, Vector2.Distance(position, centre), Delta);
            }
        }

        /// <summary>A static enemy, or an orbit of radius 0, never moves.</summary>
        [Test]
        public void ComputePosition_StaticAndZeroRadiusOrbit_StayAtStart()
        {
            var start = new Vector2(1f, 2f);

            Assert.AreEqual(start, EnemyMovement.ComputePosition(EnemyMovementType.Static, start, Vector2.zero, 5f, 1f, 1f, 3f));
            Assert.AreEqual(start, EnemyMovement.ComputePosition(EnemyMovementType.Orbit, start, Vector2.zero, 5f, 0f, 1f, 3f));
        }

        /// <summary>Damage removes hit points, never goes below zero, and a negative damage cannot heal.</summary>
        [Test]
        public void ApplyDamage_RemovesHpAndClampsAtZero()
        {
            Assert.AreEqual(40f, EnemyController.ApplyDamage(50f, 10f), Delta);
            Assert.AreEqual(0f, EnemyController.ApplyDamage(50f, 500f), Delta);
            Assert.AreEqual(50f, EnemyController.ApplyDamage(50f, -10f), Delta);
        }

        /// <summary>The hit flash starts at full strength and fades to nothing.</summary>
        [Test]
        public void ComputeHitFlash_StartsAtStrengthAndFades()
        {
            Assert.AreEqual(0.9f, EnemyController.ComputeHitFlash(0f, 0.06f, 0.9f), Delta);
            Assert.AreEqual(0.45f, EnemyController.ComputeHitFlash(0.03f, 0.06f, 0.9f), Delta);
            Assert.AreEqual(0f, EnemyController.ComputeHitFlash(0.06f, 0.06f, 0.9f), Delta);
        }

        /// <summary>The explosion shows one sprite per frame period, then reports it is over.</summary>
        [Test]
        public void ComputeFrameIndex_AdvancesAndEnds()
        {
            Assert.AreEqual(0, FrameAnimationController.ComputeFrameIndex(0f, 20f, 12));
            Assert.AreEqual(3, FrameAnimationController.ComputeFrameIndex(0.16f, 20f, 12));
            Assert.AreEqual(11, FrameAnimationController.ComputeFrameIndex(0.59f, 20f, 12));
            Assert.AreEqual(-1, FrameAnimationController.ComputeFrameIndex(0.6f, 20f, 12));
        }

        /// <summary>An animation with no frames, or a null frame rate, is over at once instead of failing.</summary>
        [Test]
        public void ComputeFrameIndex_NothingToShow_IsOver()
        {
            Assert.AreEqual(-1, FrameAnimationController.ComputeFrameIndex(0f, 20f, 0));
            Assert.AreEqual(-1, FrameAnimationController.ComputeFrameIndex(0f, 0f, 5));
        }

        /// <summary>A stun simply counts down towards zero.</summary>
        [Test]
        public void TickStun_CountsDown()
        {
            Assert.AreEqual(1f, EnemyController.TickStun(3f, 2f), 0.0001f);
        }

        /// <summary>A stun never counts below zero, however large the step.</summary>
        [Test]
        public void TickStun_NeverGoesNegative()
        {
            Assert.AreEqual(0f, EnemyController.TickStun(1f, 5f), 0.0001f);
        }

        /// <summary>Stunning an enemy that already has more time left keeps the longer duration (spec E5-06: no stacking).</summary>
        [Test]
        public void ComputeRenewedStun_KeepsTheLongerDuration()
        {
            Assert.AreEqual(5f, EnemyController.ComputeRenewedStun(5f, 3f), 0.0001f);
            Assert.AreEqual(3f, EnemyController.ComputeRenewedStun(1f, 3f), 0.0001f);
        }
    }
}
