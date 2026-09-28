using UnityEngine;

namespace Ashar.Player
{
    /// <summary>
    /// Loops the shield halo through every sprite of the ship pack's Shield folder while it is shown (spec E5-05,
    /// refined at Dyllan's request: the halo animates instead of staying a single fixed sprite).
    /// RESPONSIBILITIES: show each sprite of _frames in turn, looping back to the first once the last has been shown,
    /// for as long as this object stays active.
    /// HOW IT WORKS: PlayerShieldController.UpdateHalo turns this GameObject on and off with the Active state; OnEnable
    /// resets the clock so a fresh protection always starts the loop on its first frame. Update reads Time.deltaTime,
    /// which Time.timeScale already drives to zero while the pause menu is open, so the loop freezes for free during
    /// pause exactly like the shield's own countdown, without any extra pause-aware code here. Dialogue does not zero
    /// Time.timeScale (spec E3-02), so the loop keeps playing then, matching every other continuous animation.
    /// WHY: same "no Animator asset" simplicity as FrameAnimationController's one-shot explosions, but wrapping with
    /// modulo instead of stopping, since a halo needs to loop for as long as the protection lasts.
    /// </summary>
    public class ShieldHaloAnimator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Renderer that shows the sprites.")]
        private SpriteRenderer _renderer;

        [Header("Animation")]
        [SerializeField, Tooltip("The halo sprites, shown in order and looped (e.g. the ship pack's Shield/shield-1..6).")]
        private Sprite[] _frames;

        [SerializeField, Min(1f), Tooltip("How many sprites are shown per second.")]
        private float _framesPerSecond = 10f;

        private float _elapsed; // Seconds since this loop last restarted.

        /// <summary>Restarts the loop on the first frame every time the halo turns on.</summary>
        private void OnEnable()
        {
            _elapsed = 0f;
            ShowFrame(0);
        }

        /// <summary>Advances the loop, wrapping back to the first frame after the last.</summary>
        private void Update()
        {
            _elapsed += Time.deltaTime;
            ShowFrame(ComputeFrameIndex(_elapsed, _framesPerSecond, _frames != null ? _frames.Length : 0));
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
        /// Returns which sprite is due at a given time, wrapping back to 0 once every frame has been shown; -1 when
        /// there is nothing to show (no frames, or a non-positive frame rate).
        /// Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static int ComputeFrameIndex(float elapsed, float framesPerSecond, int frameCount)
        {
            if (frameCount <= 0 || framesPerSecond <= 0f)
            {
                return -1;
            }

            int index = (int)(elapsed * framesPerSecond);
            return index % frameCount;
        }
    }
}
