using Ashar.Player;
using NUnit.Framework;
using UnityEngine;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the timing of the hit feedback: the flash, the tint that follows and the shake.
    /// WHY: "flash for 0.1 s, then a fading tint, then nothing" and "the shake dies out and stays on the pixel grid"
    /// are easy to get subtly wrong, and cheap to check without playing.
    /// </summary>
    public class FeedbackMathTests
    {
        private const float Delta = 0.0001f;                     // Tolerance for float comparisons.
        private static readonly Color Red = new Color(1f, 0.25f, 0.25f);

        /// <summary>During the flash, the sprite is fully pushed to the flash colour.</summary>
        [Test]
        public void ComputeFlash_DuringFlash_IsFullFlashColour()
        {
            float amount = PlayerFeedbackController.ComputeFlash(0.05f, 0.1f, Color.white, 0.35f, Red, 0.6f, out Color color);

            Assert.AreEqual(1f, amount, Delta);
            Assert.AreEqual(Color.white, color);
        }

        /// <summary>Right after the flash, the tint starts at its full strength, in the tint colour.</summary>
        [Test]
        public void ComputeFlash_StartOfTint_IsTintStrength()
        {
            float amount = PlayerFeedbackController.ComputeFlash(0.1f, 0.1f, Color.white, 0.35f, Red, 0.6f, out Color color);

            Assert.AreEqual(0.6f, amount, Delta);
            Assert.AreEqual(Red, color);
        }

        /// <summary>Halfway through the tint, half of its strength is left.</summary>
        [Test]
        public void ComputeFlash_HalfwayThroughTint_IsHalfStrength()
        {
            float amount = PlayerFeedbackController.ComputeFlash(0.1f + 0.175f, 0.1f, Color.white, 0.35f, Red, 0.6f, out _);

            Assert.AreEqual(0.3f, amount, Delta);
        }

        /// <summary>After flash and tint, the sprite is back to normal.</summary>
        [Test]
        public void ComputeFlash_AfterEverything_IsZero()
        {
            float amount = PlayerFeedbackController.ComputeFlash(0.5f, 0.1f, Color.white, 0.35f, Red, 0.6f, out _);

            Assert.AreEqual(0f, amount, Delta);
        }

        /// <summary>A tint duration of 0 gives no tint instead of dividing by zero.</summary>
        [Test]
        public void ComputeFlash_ZeroTintDuration_IsZeroAfterFlash()
        {
            float amount = PlayerFeedbackController.ComputeFlash(0.2f, 0.1f, Color.white, 0f, Red, 0.6f, out _);

            Assert.AreEqual(0f, amount, Delta);
        }

        /// <summary>At the start of the shake, the offset is the full amplitude in the random direction (snapped to pixels).</summary>
        [Test]
        public void ComputeShakeOffset_AtStart_IsFullAmplitude()
        {
            Vector2 offset = PlayerFeedbackController.ComputeShakeOffset(0f, 0.3f, 0.12f, new Vector2(1f, 0f), 48f);

            Assert.AreEqual(0.125f, offset.x, Delta); // 0.12 u = 5.76 px, rounded to 6 px = 0.125 u.
            Assert.AreEqual(0f, offset.y, Delta);
        }

        /// <summary>The shake shrinks over time and is gone at the end.</summary>
        [Test]
        public void ComputeShakeOffset_GrowsSmallerAndEnds()
        {
            Vector2 early = PlayerFeedbackController.ComputeShakeOffset(0.03f, 0.3f, 0.12f, Vector2.right, 48f);
            Vector2 late = PlayerFeedbackController.ComputeShakeOffset(0.27f, 0.3f, 0.12f, Vector2.right, 48f);
            Vector2 over = PlayerFeedbackController.ComputeShakeOffset(0.3f, 0.3f, 0.12f, Vector2.right, 48f);

            Assert.Greater(early.x, late.x);
            Assert.AreEqual(0f, over.x, Delta);
        }

        /// <summary>Every shake position lies on the pixel grid, so the pixel art is never blurred.</summary>
        [Test]
        public void ComputeShakeOffset_IsSnappedToWholePixels()
        {
            Vector2 offset = PlayerFeedbackController.ComputeShakeOffset(0.05f, 0.3f, 0.12f, new Vector2(0.7f, -0.4f), 48f);

            Assert.AreEqual(0f, offset.x * 48f - Mathf.Round(offset.x * 48f), Delta);
            Assert.AreEqual(0f, offset.y * 48f - Mathf.Round(offset.y * 48f), Delta);
        }

        /// <summary>A shake duration of 0 gives no shake instead of dividing by zero.</summary>
        [Test]
        public void ComputeShakeOffset_ZeroDuration_IsZero()
        {
            Vector2 offset = PlayerFeedbackController.ComputeShakeOffset(0f, 0f, 0.12f, Vector2.one, 48f);

            Assert.AreEqual(Vector2.zero, offset);
        }
    }
}
