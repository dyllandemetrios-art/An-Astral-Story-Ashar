using UnityEngine;

namespace Ashar.Combat
{
    /// <summary>
    /// Plays a short animation made of separate sprites (an explosion, an impact) once, then removes itself.
    /// RESPONSIBILITIES: show each sprite of a list in turn at a set speed, then destroy its object.
    /// HOW IT WORKS: each frame it works out which sprite is due from the time elapsed and the frame rate, and gives it
    /// to the SpriteRenderer. When the time is past the last sprite, the object is destroyed.
    /// WHY: a simple way to play the animation frames of the effects pack without building Animator assets for each
    /// explosion. The frame choice is a static method with no Unity state, so it can be unit-tested.
    /// </summary>
    public class FrameAnimationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Renderer that shows the sprites.")]
        private SpriteRenderer _renderer;

        [Header("Animation")]
        [SerializeField, Tooltip("The sprites to show, in order.")]
        private Sprite[] _frames;

        [SerializeField, Min(1f), Tooltip("How many sprites are shown per second.")]
        private float _framesPerSecond = 24f;

        private float _elapsed; // Seconds since the animation started.

        /// <summary>Shows the first sprite at once, so the object never appears empty for a frame.</summary>
        private void Start()
        {
            ShowFrame(0);
        }

        /// <summary>Advances the animation and destroys the object once the last sprite has been shown.</summary>
        private void Update()
        {
            _elapsed += Time.deltaTime;
            int index = ComputeFrameIndex(_elapsed, _framesPerSecond, _frames != null ? _frames.Length : 0);
            if (index < 0)
            {
                Destroy(gameObject);
                return;
            }

            ShowFrame(index);
        }

        /// <summary>Gives one sprite of the list to the renderer.</summary>
        private void ShowFrame(int index)
        {
            if (_renderer != null && _frames != null && index >= 0 && index < _frames.Length)
            {
                _renderer.sprite = _frames[index];
            }
        }

        /// <summary>
        /// Returns which sprite is due at a given time, or -1 when the animation is over (or has nothing to show).
        /// Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static int ComputeFrameIndex(float elapsed, float framesPerSecond, int frameCount)
        {
            if (frameCount <= 0 || framesPerSecond <= 0f)
            {
                return -1;
            }

            int index = (int)(elapsed * framesPerSecond);
            return index >= frameCount ? -1 : index;
        }
    }
}
