using Ashar.Player;
using Ashar.UI;
using NUnit.Framework;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the pure state machine of the Pulse (spec E5-06): the Recharging/Ready transition, the graze recharge
    /// discount, and the HUD text.
    /// WHY: PlayerPulseController.Tick, CanActivate and ApplyGrazeDiscount are static and free of Unity state
    /// precisely so every timing and discount case can be checked without a scene or a frame-by-frame Play Mode
    /// session.
    /// </summary>
    public class PulseTests
    {
        private const float Cooldown = 45f;
        private const float Delta = 0.0001f;

        /// <summary>Ready never changes on its own: only an explicit activation (not part of Tick) starts a recharge.</summary>
        [Test]
        public void Tick_WhileReady_StaysReadyRegardlessOfTime()
        {
            (PulseState state, float timeLeft) = PlayerPulseController.Tick(PulseState.Ready, 0f, 100f);

            Assert.AreEqual(PulseState.Ready, state);
            Assert.AreEqual(0f, timeLeft);
        }

        /// <summary>A small step during Recharging simply counts the recharge down.</summary>
        [Test]
        public void Tick_DuringRecharging_CountsDown()
        {
            (PulseState state, float timeLeft) = PlayerPulseController.Tick(PulseState.Recharging, Cooldown, 10f);

            Assert.AreEqual(PulseState.Recharging, state);
            Assert.AreEqual(35f, timeLeft, Delta);
        }

        /// <summary>At the exact cooldown boundary, the Pulse becomes Ready.</summary>
        [Test]
        public void Tick_AtCooldownBoundary_BecomesReady()
        {
            (PulseState state, float timeLeft) = PlayerPulseController.Tick(PulseState.Recharging, Cooldown, Cooldown);

            Assert.AreEqual(PulseState.Ready, state);
            Assert.AreEqual(0f, timeLeft, Delta);
        }

        /// <summary>A frame overshooting the cooldown still lands on Ready, with no negative time left.</summary>
        [Test]
        public void Tick_FrameOvershootingCooldown_StaysReadyWithoutNegativeTime()
        {
            (PulseState state, float timeLeft) = PlayerPulseController.Tick(PulseState.Recharging, Cooldown, Cooldown + 10f);

            Assert.AreEqual(PulseState.Ready, state);
            Assert.AreEqual(0f, timeLeft, Delta);
        }

        /// <summary>A Pulse press may only fire the wave while Ready.</summary>
        [Test]
        public void CanActivate_OnlyWhileReady()
        {
            Assert.IsTrue(PlayerPulseController.CanActivate(PulseState.Ready));
            Assert.IsFalse(PlayerPulseController.CanActivate(PulseState.Recharging));
        }

        /// <summary>An admissible graze shortens the recharge by the discount.</summary>
        [Test]
        public void ApplyGrazeDiscount_DuringRecharging_ShortensIt()
        {
            (PulseState state, float timeLeft) = PlayerPulseController.ApplyGrazeDiscount(PulseState.Recharging, 10f, 0.25f);

            Assert.AreEqual(PulseState.Recharging, state);
            Assert.AreEqual(9.75f, timeLeft, Delta);
        }

        /// <summary>A graze that would take the recharge below zero instead lands exactly on Ready.</summary>
        [Test]
        public void ApplyGrazeDiscount_PastZero_BecomesReadyWithoutNegativeTime()
        {
            (PulseState state, float timeLeft) = PlayerPulseController.ApplyGrazeDiscount(PulseState.Recharging, 0.1f, 0.25f);

            Assert.AreEqual(PulseState.Ready, state);
            Assert.AreEqual(0f, timeLeft, Delta);
        }

        /// <summary>A graze while already Ready changes nothing: there is no recharge left to shorten.</summary>
        [Test]
        public void ApplyGrazeDiscount_WhileReady_ChangesNothing()
        {
            (PulseState state, float timeLeft) = PlayerPulseController.ApplyGrazeDiscount(PulseState.Ready, 0f, 0.25f);

            Assert.AreEqual(PulseState.Ready, state);
            Assert.AreEqual(0f, timeLeft, Delta);
        }

        /// <summary>The HUD shows the plain Ready label with no timer.</summary>
        [Test]
        public void ComputeStatusText_Ready_ShowsPlainLabel()
        {
            string text = PulseHudController.ComputeStatusText(true, 0f, "PRETE", "CHARGE");

            Assert.AreEqual("PRETE", text);
        }

        /// <summary>The HUD shows the charging label with the seconds left, rounded up.</summary>
        [Test]
        public void ComputeStatusText_Charging_ShowsLabelAndRoundedUpTime()
        {
            string text = PulseHudController.ComputeStatusText(false, 44.2f, "PRETE", "CHARGE");

            Assert.AreEqual("CHARGE 45s", text);
        }
    }
}
