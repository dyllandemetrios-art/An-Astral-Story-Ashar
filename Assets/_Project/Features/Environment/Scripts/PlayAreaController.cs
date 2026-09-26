using UnityEngine;

namespace Ashar.Environment
{
    /// <summary>
    /// Defines the rectangle in which the player ship may move, and draws it as a Gizmo.
    /// RESPONSIBILITIES: expose the playable limits (Bounds) to gameplay scripts and show them in the Scene view.
    /// HOW IT WORKS: the camera is fixed and always shows referenceResolution / pixelsPerUnit world units
    /// (Pixel Perfect Camera + Windowbox), so the visible area never changes with the window size.
    /// The playable area is that visible area shrunk by a margin on every side (spec §4).
    /// WHY: one shared source of truth for the limits, instead of each script guessing the screen size.
    /// </summary>
    public class PlayAreaController : MonoBehaviour
    {
        [Header("Screen")]
        [SerializeField, Tooltip("Reference resolution of the Pixel Perfect Camera, in pixels. Must match the camera component.")]
        private Vector2Int _referenceResolution = new Vector2Int(960, 540);

        [SerializeField, Min(1f), Tooltip("Pixels per world unit of the game sprites. Must match the Pixel Perfect Camera (Assets PPU).")]
        private float _pixelsPerUnit = 48f;

        [Header("Play area")]
        [SerializeField, Min(0f), Tooltip("Distance kept between the play area and each screen edge, in world units (spec §4: 0.7).")]
        private float _margin = 0.7f;

        [Header("Gizmos")]
        [SerializeField, Tooltip("Colour of the playable rectangle in the Scene view.")]
        private Color _playAreaColor = new Color(0.2f, 1f, 0.4f, 1f);

        [SerializeField, Tooltip("Colour of the full visible screen rectangle in the Scene view.")]
        private Color _screenColor = new Color(0.6f, 0.6f, 0.6f, 1f);

        /// <summary>The rectangle the player ship must stay inside, in world units.</summary>
        public Rect Bounds => ComputeBounds(transform.position, _referenceResolution, _pixelsPerUnit, _margin);

        /// <summary>The whole visible screen, in world units (the play area without its margin).</summary>
        public Rect ScreenBounds => ComputeBounds(transform.position, _referenceResolution, _pixelsPerUnit, 0f);

        /// <summary>Returns the point of the play area closest to the given position.</summary>
        public Vector2 Clamp(Vector2 position)
        {
            return ClampToRect(Bounds, position);
        }

        /// <summary>
        /// Computes a rectangle centred on a point: the reference screen size in world units, minus a margin on each side.
        /// Static and free of Unity state so it can be unit-tested. A margin bigger than the screen gives an empty rectangle.
        /// </summary>
        public static Rect ComputeBounds(Vector2 center, Vector2Int referenceResolution, float pixelsPerUnit, float margin)
        {
            float width = Mathf.Max(0f, referenceResolution.x / pixelsPerUnit - 2f * margin);
            float height = Mathf.Max(0f, referenceResolution.y / pixelsPerUnit - 2f * margin);
            var size = new Vector2(width, height);
            return new Rect(center - size * 0.5f, size);
        }

        /// <summary>Returns the point of the rectangle closest to the given position (the position itself if inside).</summary>
        public static Vector2 ClampToRect(Rect rect, Vector2 position)
        {
            return new Vector2(
                Mathf.Clamp(position.x, rect.xMin, rect.xMax),
                Mathf.Clamp(position.y, rect.yMin, rect.yMax));
        }

        /// <summary>Returns the rectangle grown by the margin on every side (used to give bullets room to leave the screen).</summary>
        public static Rect Inflate(Rect rect, float margin)
        {
            return new Rect(rect.xMin - margin, rect.yMin - margin, rect.width + 2f * margin, rect.height + 2f * margin);
        }

        /// <summary>Draws the screen and the play area in the Scene view, even when the object is not selected.</summary>
        private void OnDrawGizmos()
        {
            DrawRect(ScreenBounds, _screenColor);
            DrawRect(Bounds, _playAreaColor);
        }

        /// <summary>Draws the outline of a rectangle on the z = 0 plane.</summary>
        private static void DrawRect(Rect rect, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(rect.center, rect.size);
        }
    }
}
