using Ashar.Player;
using NUnit.Framework;
using UnityEngine;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the size of the hitbox capsule and of the graze capsule that wraps it.
    /// WHY: the ship is a long shape, so both zones are vertical capsules whose size is computed from a few tuning
    /// values. A wrong formula would make the ship touchable in the wrong places, and it is cheap to check here.
    /// </summary>
    public class HitboxShapeTests
    {
        private const float Delta = 0.0001f; // Tolerance for float comparisons.

        /// <summary>The hitbox capsule is twice the radius wide and as tall as asked.</summary>
        [Test]
        public void ComputeHitboxSize_NormalValues_IsWidthAndHeight()
        {
            Vector2 size = PlayerHealthController.ComputeHitboxSize(0.27f, 1.1f);

            Assert.AreEqual(0.54f, size.x, Delta);
            Assert.AreEqual(1.1f, size.y, Delta);
        }

        /// <summary>A height smaller than the width is raised to the width: the capsule becomes a circle, never a squashed shape.</summary>
        [Test]
        public void ComputeHitboxSize_HeightBelowWidth_BecomesCircle()
        {
            Vector2 size = PlayerHealthController.ComputeHitboxSize(0.27f, 0.2f);

            Assert.AreEqual(0.54f, size.x, Delta);
            Assert.AreEqual(0.54f, size.y, Delta);
        }

        /// <summary>The graze capsule is the hitbox grown by (graze radius - hitbox radius) on every side.</summary>
        [Test]
        public void ComputeGrazeSize_NormalValues_IsHitboxPlusMargin()
        {
            Vector2 size = PlayerGrazeController.ComputeGrazeSize(0.6f, 0.27f, 1.1f);

            Assert.AreEqual(1.2f, size.x, Delta);   // 0.54 + 2 x 0.33.
            Assert.AreEqual(1.76f, size.y, Delta);  // 1.10 + 2 x 0.33.
        }

        /// <summary>A graze radius smaller than the hitbox radius gives no margin: the zone never shrinks below the hitbox.</summary>
        [Test]
        public void ComputeGrazeSize_GrazeRadiusBelowHitbox_IsHitboxSize()
        {
            Vector2 size = PlayerGrazeController.ComputeGrazeSize(0.1f, 0.27f, 1.1f);

            Assert.AreEqual(0.54f, size.x, Delta);
            Assert.AreEqual(1.1f, size.y, Delta);
        }
    }
}
