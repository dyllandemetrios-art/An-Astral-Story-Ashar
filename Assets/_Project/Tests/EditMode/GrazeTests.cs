using Ashar.Combat;
using Ashar.Player;
using NUnit.Framework;
using UnityEngine;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the graze rules: each bullet counts once, never during a dash, and never when it is an impact.
    /// WHY: these are the three acceptance criteria of the story, and each one is a way to earn points that should
    /// not be earned. Checking them without running the game keeps the scoring honest.
    /// </summary>
    public class GrazeTests
    {
        private const float Delta = 0.0001f; // Tolerance for float comparisons.

        /// <summary>A fresh bullet, with the ship flying normally and away from the hitbox, counts as a graze.</summary>
        [Test]
        public void ShouldCountGraze_NormalNearMiss_Counts()
        {
            Assert.IsTrue(PlayerGrazeController.ShouldCountGraze(false, false, false));
        }

        /// <summary>A bullet already counted never counts again.</summary>
        [Test]
        public void ShouldCountGraze_AlreadyCounted_DoesNotCount()
        {
            Assert.IsFalse(PlayerGrazeController.ShouldCountGraze(true, false, false));
        }

        /// <summary>No graze while the ship is dashing.</summary>
        [Test]
        public void ShouldCountGraze_WhileDashing_DoesNotCount()
        {
            Assert.IsFalse(PlayerGrazeController.ShouldCountGraze(false, true, false));
        }

        /// <summary>A bullet already touching the hitbox is an impact: the impact wins, so no graze.</summary>
        [Test]
        public void ShouldCountGraze_TouchingHitbox_DoesNotCount()
        {
            Assert.IsFalse(PlayerGrazeController.ShouldCountGraze(false, false, true));
        }

        /// <summary>The same bullet entering the graze zone three times earns the points once.</summary>
        [Test]
        public void SameBulletEnteringSeveralTimes_IsCountedOnce()
        {
            var bulletObject = new GameObject("TestBullet");
            var bullet = bulletObject.AddComponent<ProjectileController>();
            int grazes = 0;

            for (int i = 0; i < 3; i++)
            {
                if (PlayerGrazeController.ShouldCountGraze(bullet.HasBeenGrazed, false, false))
                {
                    bullet.MarkGrazed();
                    grazes++;
                }
            }

            Object.DestroyImmediate(bulletObject);
            Assert.AreEqual(1, grazes);
        }

        /// <summary>A bullet that hit the ship is marked, so it can no longer be counted as a graze afterwards.</summary>
        [Test]
        public void BulletMarkedByAnImpact_CannotBeGrazed()
        {
            var bulletObject = new GameObject("TestBullet");
            var bullet = bulletObject.AddComponent<ProjectileController>();

            bullet.MarkGrazed(); // What PlayerHealthController does when the bullet hits.
            bool counts = PlayerGrazeController.ShouldCountGraze(bullet.HasBeenGrazed, false, false);

            Object.DestroyImmediate(bulletObject);
            Assert.IsFalse(counts);
        }

        /// <summary>The graze flash starts at its strength and fades linearly to nothing.</summary>
        [Test]
        public void ComputeGrazeFlash_StartsAtStrengthAndFades()
        {
            Assert.AreEqual(0.35f, PlayerFeedbackController.ComputeGrazeFlash(0f, 0.08f, 0.35f), Delta);
            Assert.AreEqual(0.175f, PlayerFeedbackController.ComputeGrazeFlash(0.04f, 0.08f, 0.35f), Delta);
            Assert.AreEqual(0f, PlayerFeedbackController.ComputeGrazeFlash(0.08f, 0.08f, 0.35f), Delta);
        }

        /// <summary>A graze flash duration of 0 gives no flash instead of dividing by zero.</summary>
        [Test]
        public void ComputeGrazeFlash_ZeroDuration_IsZero()
        {
            Assert.AreEqual(0f, PlayerFeedbackController.ComputeGrazeFlash(0f, 0f, 0.35f), Delta);
        }
    }
}
