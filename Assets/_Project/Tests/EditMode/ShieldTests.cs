using Ashar.Player;
using Ashar.UI;
using NUnit.Framework;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the pure state machine of the ephemeral shield (spec E5-05): the Ready/Active/Recharging transitions,
    /// the 5 s / 20 s boundaries, and that one long frame lands on the same state as many short ones covering the
    /// same total time.
    /// WHY: PlayerShieldController.Tick and CanActivate are static and free of Unity state precisely so every timing
    /// case can be checked without a scene or a frame-by-frame Play Mode session.
    /// </summary>
    public class ShieldTests
    {
        private const float ProtectionTime = 5f;
        private const float Cooldown = 20f;
        private const float Delta = 0.0001f;

        /// <summary>Ready never changes on its own: only an explicit activation (not part of Tick) starts a protection.</summary>
        [Test]
        public void Tick_WhileReady_StaysReadyRegardlessOfTime()
        {
            (ShieldState state, float timeLeft) = PlayerShieldController.Tick(ShieldState.Ready, 0f, 100f, Cooldown);

            Assert.AreEqual(ShieldState.Ready, state);
            Assert.AreEqual(0f, timeLeft);
        }

        /// <summary>A small step during Active simply counts the protection down.</summary>
        [Test]
        public void Tick_DuringActive_CountsDown()
        {
            (ShieldState state, float timeLeft) = PlayerShieldController.Tick(ShieldState.Active, ProtectionTime, 2f, Cooldown);

            Assert.AreEqual(ShieldState.Active, state);
            Assert.AreEqual(3f, timeLeft, Delta);
        }

        /// <summary>At the exact protection boundary (spec: protection on [0,5[), the shield has already switched to Recharging.</summary>
        [Test]
        public void Tick_AtProtectionBoundary_SwitchesToRecharging()
        {
            (ShieldState state, float timeLeft) = PlayerShieldController.Tick(ShieldState.Active, ProtectionTime, ProtectionTime, Cooldown);

            Assert.AreEqual(ShieldState.Recharging, state);
            Assert.AreEqual(Cooldown, timeLeft, Delta);
        }

        /// <summary>A small step during Recharging simply counts the cooldown down.</summary>
        [Test]
        public void Tick_DuringRecharging_CountsDown()
        {
            (ShieldState state, float timeLeft) = PlayerShieldController.Tick(ShieldState.Recharging, Cooldown, 12f, Cooldown);

            Assert.AreEqual(ShieldState.Recharging, state);
            Assert.AreEqual(8f, timeLeft, Delta);
        }

        /// <summary>At the exact cooldown boundary (spec: recharge on [5,25[, i.e. 20 s after protection ends), the shield is Ready.</summary>
        [Test]
        public void Tick_AtCooldownBoundary_BecomesReady()
        {
            (ShieldState state, float timeLeft) = PlayerShieldController.Tick(ShieldState.Recharging, Cooldown, Cooldown, Cooldown);

            Assert.AreEqual(ShieldState.Ready, state);
            Assert.AreEqual(0f, timeLeft, Delta);
        }

        /// <summary>
        /// One frame long enough to cover the whole cycle (protection + cooldown) lands exactly on Ready, with no extra
        /// protection or shortened recharge, matching the worked example of the spec (activation at t=0, Ready again
        /// only at t=25).
        /// </summary>
        [Test]
        public void Tick_OneHugeFrame_CoveringWholeCycle_LandsOnReady()
        {
            (ShieldState state, float timeLeft) = PlayerShieldController.Tick(ShieldState.Active, ProtectionTime, ProtectionTime + Cooldown, Cooldown);

            Assert.AreEqual(ShieldState.Ready, state);
            Assert.AreEqual(0f, timeLeft, Delta);
        }

        /// <summary>
        /// A frame that overshoots the whole cycle by a few seconds still lands on Ready: the overshoot is dropped, not
        /// carried into a second activation (Ready does not restart the cycle on its own).
        /// </summary>
        [Test]
        public void Tick_FrameOvershootingWholeCycle_StaysReadyWithoutCarryingOverTime()
        {
            (ShieldState state, float timeLeft) = PlayerShieldController.Tick(ShieldState.Active, ProtectionTime, ProtectionTime + Cooldown + 10f, Cooldown);

            Assert.AreEqual(ShieldState.Ready, state);
            Assert.AreEqual(0f, timeLeft, Delta);
        }

        /// <summary>Many short frames covering the same total time as one long frame reach the exact same final state (spec).</summary>
        [Test]
        public void Tick_ManySmallFrames_MatchesOneBigFrameOverSameTotalTime()
        {
            const float totalTime = ProtectionTime + 8f; // Into Recharging, partway through it.
            const float smallStep = 0.1f;

            ShieldState smallStepsState = ShieldState.Active;
            float smallStepsTimeLeft = ProtectionTime;
            int steps = 0;
            while (steps * smallStep < totalTime)
            {
                (smallStepsState, smallStepsTimeLeft) = PlayerShieldController.Tick(smallStepsState, smallStepsTimeLeft, smallStep, Cooldown);
                steps++;
            }

            (ShieldState bigStepState, float bigStepTimeLeft) = PlayerShieldController.Tick(ShieldState.Active, ProtectionTime, steps * smallStep, Cooldown);

            Assert.AreEqual(bigStepState, smallStepsState);
            Assert.AreEqual(bigStepTimeLeft, smallStepsTimeLeft, Delta);
        }

        /// <summary>A Shield press may only start a new protection while Ready.</summary>
        [Test]
        public void CanActivate_OnlyWhileReady()
        {
            Assert.IsTrue(PlayerShieldController.CanActivate(ShieldState.Ready));
            Assert.IsFalse(PlayerShieldController.CanActivate(ShieldState.Active));
            Assert.IsFalse(PlayerShieldController.CanActivate(ShieldState.Recharging));
        }

        /// <summary>The HUD shows the plain Ready label with no timer.</summary>
        [Test]
        public void ComputeStatusText_Ready_ShowsPlainLabel()
        {
            string text = ShieldHudController.ComputeStatusText(true, false, 0f, "PRET", "ACTIF", "RECHARGE");

            Assert.AreEqual("PRET", text);
        }

        /// <summary>The HUD shows the Active label with the seconds left, rounded up.</summary>
        [Test]
        public void ComputeStatusText_Active_ShowsLabelAndRoundedUpTime()
        {
            string text = ShieldHudController.ComputeStatusText(false, true, 3.2f, "PRET", "ACTIF", "RECHARGE");

            Assert.AreEqual("ACTIF 4s", text);
        }

        /// <summary>The HUD shows the Recharging label with the seconds left, rounded up.</summary>
        [Test]
        public void ComputeStatusText_Recharging_ShowsLabelAndRoundedUpTime()
        {
            string text = ShieldHudController.ComputeStatusText(false, false, 12.01f, "PRET", "ACTIF", "RECHARGE");

            Assert.AreEqual("RECHARGE 13s", text);
        }
    }
}
