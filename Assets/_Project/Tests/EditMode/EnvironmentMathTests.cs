using Ashar.Environment;
using NUnit.Framework;
using UnityEngine;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the pure calculations of the Environment feature: play area limits and background wrap-around.
    /// WHY: these formulas are what every later feature relies on (player limits, seamless scroll), and they
    /// are cheap to check without opening a scene.
    /// </summary>
    public class EnvironmentMathTests
    {
        private const float Delta = 0.0001f; // Tolerance for float comparisons.

        /// <summary>960x540 at 48 pixels per unit is 20x11.25 units; a 0.7 margin on each side leaves 18.6x9.85.</summary>
        [Test]
        public void ComputeBounds_DefaultReference_ShrinksScreenByMarginOnEachSide()
        {
            Rect bounds = PlayAreaController.ComputeBounds(Vector2.zero, new Vector2Int(960, 540), 48f, 0.7f);

            Assert.AreEqual(18.6f, bounds.width, Delta);
            Assert.AreEqual(9.85f, bounds.height, Delta);
            Assert.AreEqual(Vector2.zero.x, bounds.center.x, Delta);
            Assert.AreEqual(Vector2.zero.y, bounds.center.y, Delta);
        }

        /// <summary>With no margin the rectangle is the whole 20x11.25 screen.</summary>
        [Test]
        public void ComputeBounds_ZeroMargin_ReturnsFullScreen()
        {
            Rect bounds = PlayAreaController.ComputeBounds(Vector2.zero, new Vector2Int(960, 540), 48f, 0f);

            Assert.AreEqual(20f, bounds.width, Delta);
            Assert.AreEqual(11.25f, bounds.height, Delta);
        }

        /// <summary>The rectangle follows the point it is centred on.</summary>
        [Test]
        public void ComputeBounds_OffsetCenter_MovesTheRectangle()
        {
            Rect bounds = PlayAreaController.ComputeBounds(new Vector2(2f, -1f), new Vector2Int(960, 540), 48f, 0.7f);

            Assert.AreEqual(2f, bounds.center.x, Delta);
            Assert.AreEqual(-1f, bounds.center.y, Delta);
        }

        /// <summary>A margin larger than half the screen gives an empty rectangle, never a negative size.</summary>
        [Test]
        public void ComputeBounds_MarginLargerThanScreen_ReturnsEmptyRect()
        {
            Rect bounds = PlayAreaController.ComputeBounds(Vector2.zero, new Vector2Int(960, 540), 48f, 50f);

            Assert.AreEqual(0f, bounds.width, Delta);
            Assert.AreEqual(0f, bounds.height, Delta);
        }

        /// <summary>A point outside the rectangle is brought back to the nearest edge; a point inside is unchanged.</summary>
        [Test]
        public void ClampToRect_PointOutside_ReturnsNearestEdgePoint()
        {
            var rect = new Rect(-2f, -1f, 4f, 2f);

            Vector2 outside = PlayAreaController.ClampToRect(rect, new Vector2(10f, -5f));
            Vector2 inside = PlayAreaController.ClampToRect(rect, new Vector2(0.5f, 0.5f));

            Assert.AreEqual(2f, outside.x, Delta);
            Assert.AreEqual(-1f, outside.y, Delta);
            Assert.AreEqual(0.5f, inside.x, Delta);
            Assert.AreEqual(0.5f, inside.y, Delta);
        }

        /// <summary>Below one tile height, the offset simply grows by speed x time.</summary>
        [Test]
        public void AdvanceOffset_BelowTileHeight_AddsDistance()
        {
            float offset = BackgroundScrollController.AdvanceOffset(1f, 4f, 0.5f, 11.25f);

            Assert.AreEqual(3f, offset, Delta);
        }

        /// <summary>Past one tile height the offset wraps around and keeps the leftover distance.</summary>
        [Test]
        public void AdvanceOffset_PastTileHeight_WrapsAndKeepsLeftover()
        {
            float offset = BackgroundScrollController.AdvanceOffset(11f, 4f, 0.5f, 11.25f);

            Assert.AreEqual(1.75f, offset, Delta); // 11 + 2 = 13, minus 11.25.
        }

        /// <summary>A tile with no height cannot scroll: the offset stays at 0 instead of dividing by zero.</summary>
        [Test]
        public void AdvanceOffset_ZeroTileHeight_ReturnsZero()
        {
            float offset = BackgroundScrollController.AdvanceOffset(3f, 4f, 0.5f, 0f);

            Assert.AreEqual(0f, offset, Delta);
        }
    }
}
