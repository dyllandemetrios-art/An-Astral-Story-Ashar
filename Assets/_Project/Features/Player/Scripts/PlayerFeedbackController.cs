using Ashar.Core;
using UnityEngine;

namespace Ashar.Player
{
    /// <summary>
    /// Gives the player ship its sounds (fire, dash, hit) and its visual reaction to a hit (flash, tint, shake).
    /// RESPONSIBILITIES: listen to the game events of the player and react with sound and picture.
    /// HOW IT WORKS: this script subscribes to GameEvents in OnEnable. Fire and dash play one sound each. A hit plays
    /// its sound and starts a timer; while it runs, the sprite is drawn in the flash colour, then tinted red and fading,
    /// and the ship image (the Visual child, never the hitbox) shakes with a distance that shrinks to zero.
    /// WHY: the flash is done by the SpriteFlash shader, driven through a MaterialPropertyBlock, so no material is
    /// copied and the asset is never changed. The calculations are static methods with no Unity state, so they can be
    /// unit-tested. Shake positions are snapped to whole pixels so the pixel art stays crisp.
    /// </summary>
    public class PlayerFeedbackController : MonoBehaviour
    {
        private const float PixelsPerUnit = 48f; // Engine scale (spec §4), used to snap the shake to whole pixels.

        private static readonly int FlashColorId = Shader.PropertyToID("_FlashColor");   // Shader property ids are faster than names.
        private static readonly int FlashAmountId = Shader.PropertyToID("_FlashAmount");

        [Header("Data")]
        [SerializeField, Tooltip("Durations, colours and strengths of the hit feedback.")]
        private PlayerFeedbackData _feedbackData;

        [Header("References")]
        [SerializeField, Tooltip("Renderer of the ship image. It must use the SpriteFlash shader for the flash to show.")]
        private SpriteRenderer _visual;

        [SerializeField, Tooltip("Plays the sounds. Its volume sets the overall level of the ship sounds.")]
        private AudioSource _audioSource;

        [Header("Sounds")]
        [SerializeField, Tooltip("Played each time the ship fires a bullet.")]
        private AudioClip _fireClip;

        [SerializeField, Tooltip("Played when the ship starts a dash.")]
        private AudioClip _dashClip;

        [SerializeField, Tooltip("Played when the ship is hit.")]
        private AudioClip _hitClip;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: seconds since the last hit, or a negative value when no feedback is running.")]
        private float _hitElapsed = -1f;

        private MaterialPropertyBlock _block;   // Per-renderer shader values, so the shared material is left alone.
        private Vector3 _visualStartPosition;   // Local position of the Visual at rest, restored after the shake.
        private Vector2 _shakeRandom;           // Current random direction of the shake, in [-1, 1] on both axes.
        private float _shakeTimer;              // Seconds left before a new random shake position is chosen.

        /// <summary>Checks the references, remembers the rest position and prepares the shader values.</summary>
        private void Awake()
        {
            if (_feedbackData == null || _visual == null || _audioSource == null)
            {
                Debug.LogError($"{nameof(PlayerFeedbackController)} on '{name}' is missing its feedback data, visual or audio source. Feedback disabled.", this);
                enabled = false;
                return;
            }

            _block = new MaterialPropertyBlock();
            _visualStartPosition = _visual.transform.localPosition;
            ApplyFlash(Color.white, 0f);
        }

        /// <summary>Starts listening to the player events.</summary>
        private void OnEnable()
        {
            GameEvents.OnPlayerFired += HandleFired;
            GameEvents.OnPlayerDashed += HandleDashed;
            GameEvents.OnPlayerHit += HandleHit;
        }

        /// <summary>Stops listening, and puts the image back at rest, so a disabled ship is never left flashing.</summary>
        private void OnDisable()
        {
            GameEvents.OnPlayerFired -= HandleFired;
            GameEvents.OnPlayerDashed -= HandleDashed;
            GameEvents.OnPlayerHit -= HandleHit;

            if (_visual != null)
            {
                Rest();
            }
        }

        /// <summary>Runs the flash, tint and shake while a hit feedback is playing.</summary>
        private void Update()
        {
            if (_hitElapsed < 0f)
            {
                return;
            }

            _hitElapsed += Time.deltaTime;
            if (_hitElapsed >= _feedbackData.TotalDuration)
            {
                Rest();
                return;
            }

            float amount = ComputeFlash(_hitElapsed, _feedbackData.FlashDuration, _feedbackData.FlashColor,
                _feedbackData.TintDuration, _feedbackData.TintColor, _feedbackData.TintStrength, out Color color);
            ApplyFlash(color, amount);

            _shakeTimer -= Time.deltaTime;
            if (_shakeTimer <= 0f)
            {
                _shakeTimer = _feedbackData.ShakeInterval;
                _shakeRandom = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }

            Vector2 offset = ComputeShakeOffset(_hitElapsed, _feedbackData.ShakeDuration, _feedbackData.ShakeAmplitude, _shakeRandom, PixelsPerUnit);
            _visual.transform.localPosition = _visualStartPosition + (Vector3)offset;
        }

        /// <summary>Plays the fire sound.</summary>
        private void HandleFired()
        {
            Play(_fireClip);
        }

        /// <summary>Plays the dash sound.</summary>
        private void HandleDashed()
        {
            Play(_dashClip);
        }

        /// <summary>Plays the hit sound and starts the flash, tint and shake.</summary>
        private void HandleHit()
        {
            Play(_hitClip);
            _hitElapsed = 0f;
            _shakeTimer = 0f; // Pick a first random position at once.
        }

        /// <summary>Plays a clip once, if there is one. A missing clip is never an error: the game works without sound.</summary>
        private void Play(AudioClip clip)
        {
            if (clip != null)
            {
                _audioSource.PlayOneShot(clip);
            }
        }

        /// <summary>Puts the image back to its normal colour and position and ends the feedback.</summary>
        private void Rest()
        {
            _hitElapsed = -1f;
            ApplyFlash(Color.white, 0f);
            _visual.transform.localPosition = _visualStartPosition;
        }

        /// <summary>Sends the flash colour and amount to the shader of the ship image.</summary>
        private void ApplyFlash(Color color, float amount)
        {
            _visual.GetPropertyBlock(_block);
            _block.SetColor(FlashColorId, color);
            _block.SetFloat(FlashAmountId, amount);
            _visual.SetPropertyBlock(_block);
        }

        /// <summary>
        /// Returns how far the sprite is pushed towards a colour, and that colour, at a given time after a hit.
        /// First the flash colour at full strength, then the tint colour fading out, then nothing.
        /// Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static float ComputeFlash(float elapsed, float flashDuration, Color flashColor,
            float tintDuration, Color tintColor, float tintStrength, out Color color)
        {
            if (elapsed < flashDuration)
            {
                color = flashColor;
                return 1f;
            }

            color = tintColor;
            float end = flashDuration + tintDuration;
            if (tintDuration <= 0f || elapsed >= end)
            {
                return 0f;
            }

            float progress = (elapsed - flashDuration) / tintDuration; // 0 at the end of the flash, 1 at the end of the tint.
            return tintStrength * (1f - progress);
        }

        /// <summary>
        /// Returns the sideways and vertical shake distance at a given time: a random direction scaled by an amplitude
        /// that shrinks linearly to zero, snapped to whole pixels. Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static Vector2 ComputeShakeOffset(float elapsed, float duration, float amplitude, Vector2 randomDirection, float pixelsPerUnit)
        {
            if (duration <= 0f || elapsed >= duration)
            {
                return Vector2.zero;
            }

            float fade = 1f - elapsed / duration;
            Vector2 raw = randomDirection * (amplitude * fade);
            return new Vector2(Mathf.Round(raw.x * pixelsPerUnit) / pixelsPerUnit, Mathf.Round(raw.y * pixelsPerUnit) / pixelsPerUnit);
        }
    }
}
