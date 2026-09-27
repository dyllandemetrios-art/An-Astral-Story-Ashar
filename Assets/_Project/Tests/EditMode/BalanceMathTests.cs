using Ashar.Core;
using NUnit.Framework;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests how the global balance multipliers (spec §7.9) are applied to enemies and bullets.
    /// WHY: "fire density 0.8" must really mean shots 25 % further apart, and a wrong formula here would silently change the
    /// difficulty of the whole game. The rules are pure, so they are checked exactly.
    /// </summary>
    public class BalanceMathTests
    {
        private const float Delta = 0.0001f; // Tolerance for float comparisons.

        /// <summary>Hit points are multiplied, and never fall below 1.</summary>
        [Test]
        public void ScaledHp_MultipliesAndKeepsAtLeastOne()
        {
            Assert.AreEqual(100f, BalanceMath.ScaledHp(50f, 2f), Delta);
            Assert.AreEqual(50f, BalanceMath.ScaledHp(50f, 1f), Delta);
            Assert.AreEqual(1f, BalanceMath.ScaledHp(1f, 0.1f), Delta);
        }

        /// <summary>A density of 0.8 stretches a 2 s interval to 2.5 s (25 % further apart); 2 halves it.</summary>
        [Test]
        public void ScaledFireInterval_DensityDividesTheInterval()
        {
            Assert.AreEqual(2.5f, BalanceMath.ScaledFireInterval(2f, 0.8f), Delta);
            Assert.AreEqual(1f, BalanceMath.ScaledFireInterval(2f, 2f), Delta);
            Assert.AreEqual(2f, BalanceMath.ScaledFireInterval(2f, 1f), Delta);
        }

        /// <summary>A density of 0 or less leaves the interval alone instead of dividing by zero.</summary>
        [Test]
        public void ScaledFireInterval_ZeroDensity_LeavesIntervalUnchanged()
        {
            Assert.AreEqual(2f, BalanceMath.ScaledFireInterval(2f, 0f), Delta);
            Assert.AreEqual(2f, BalanceMath.ScaledFireInterval(2f, -1f), Delta);
        }

        /// <summary>Bullet speed is simply multiplied.</summary>
        [Test]
        public void ScaledBulletSpeed_Multiplies()
        {
            Assert.AreEqual(11.2f, BalanceMath.ScaledBulletSpeed(5.6f, 2f), Delta);
            Assert.AreEqual(5.6f, BalanceMath.ScaledBulletSpeed(5.6f, 1f), Delta);
        }

        /// <summary>A bullet may be created while fewer than the maximum are alive; a maximum of 0 means no limit.</summary>
        [TestCase(0, 2000, true)]
        [TestCase(1999, 2000, true)]
        [TestCase(2000, 2000, false)]
        [TestCase(5000, 2000, false)]
        [TestCase(99999, 0, true)]
        public void CanCreateProjectile_RespectsTheLimit(int alive, int max, bool expected)
        {
            Assert.AreEqual(expected, BalanceMath.CanCreateProjectile(alive, max));
        }

        /// <summary>A new session has neutral multipliers (1) and no limit until the balance is set.</summary>
        [Test]
        public void GameSession_DefaultBalance_IsNeutral()
        {
            var session = new GameSession(3, -1);

            Assert.AreEqual(1f, session.EnemyHpMultiplier, Delta);
            Assert.AreEqual(1f, session.FireDensityMultiplier, Delta);
            Assert.AreEqual(1f, session.BulletSpeedMultiplier, Delta);
            Assert.AreEqual(0, session.MaxProjectiles);
        }

        /// <summary>SetBalance replaces the multipliers, so tuning the asset live changes the running game.</summary>
        [Test]
        public void GameSession_SetBalance_ReplacesTheValues()
        {
            var session = new GameSession(3, -1);

            session.SetBalance(1.5f, 0.8f, 1.2f, 2000);

            Assert.AreEqual(1.5f, session.EnemyHpMultiplier, Delta);
            Assert.AreEqual(0.8f, session.FireDensityMultiplier, Delta);
            Assert.AreEqual(1.2f, session.BulletSpeedMultiplier, Delta);
            Assert.AreEqual(2000, session.MaxProjectiles);
        }
    }
}
