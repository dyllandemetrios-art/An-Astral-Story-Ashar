using UnityEngine;

namespace Ashar.Player
{
    /// <summary>
    /// Tuning values of the visual feedback when the player ship is hit.
    /// RESPONSIBILITIES: hold the durations, colours and strength of the hit flash, the tint that follows it and
    /// the shake, so they can be adjusted in the Inspector without touching code.
    /// HOW IT WORKS: a hit plays three effects at once. First a short flash in one colour (white); then the ship
    /// is tinted (red) and fades back to normal; during the same time the ship image shakes and settles.
    /// PATTERN: data-driven design, like PlayerShipData. Separate asset because it is about looks, not gameplay.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerFeedbackData_Default", menuName = "Ashar/Player Feedback Data")]
    public class PlayerFeedbackData : ScriptableObject
    {
        [Header("Flash")]
        [SerializeField, Min(0f), Tooltip("How long the ship is drawn entirely in the flash colour, in seconds (spec E1-06: 0.1).")]
        private float _flashDuration = 0.1f;

        [SerializeField, Tooltip("Colour of the flash.")]
        private Color _flashColor = Color.white;

        [Header("Tint")]
        [SerializeField, Min(0f), Tooltip("How long the tint lasts after the flash, fading out, in seconds.")]
        private float _tintDuration = 0.35f;

        [SerializeField, Tooltip("Colour the ship is tinted with after the flash. A light red, different from the orange of enemy bullets.")]
        private Color _tintColor = new Color(1f, 0.25f, 0.25f, 1f);

        [SerializeField, Range(0f, 1f), Tooltip("How strongly the ship is tinted at the start of the tint (0 = not at all, 1 = fully the tint colour).")]
        private float _tintStrength = 0.6f;

        [Header("Shake")]
        [SerializeField, Min(0f), Tooltip("How long the ship image shakes, in seconds.")]
        private float _shakeDuration = 0.3f;

        [SerializeField, Min(0f), Tooltip("Largest shake distance at the start, in world units. It fades to 0. Only the image shakes, never the hitbox.")]
        private float _shakeAmplitude = 0.12f;

        [SerializeField, Min(0.005f), Tooltip("Time between two random shake positions, in seconds. Larger values look like a slower tremble.")]
        private float _shakeInterval = 0.04f;

        /// <summary>How long the ship is drawn entirely in the flash colour, in seconds.</summary>
        public float FlashDuration => _flashDuration;

        /// <summary>Colour of the flash.</summary>
        public Color FlashColor => _flashColor;

        /// <summary>How long the tint lasts after the flash, in seconds.</summary>
        public float TintDuration => _tintDuration;

        /// <summary>Colour the ship is tinted with after the flash.</summary>
        public Color TintColor => _tintColor;

        /// <summary>Strength of the tint at its start, from 0 to 1.</summary>
        public float TintStrength => _tintStrength;

        /// <summary>How long the ship image shakes, in seconds.</summary>
        public float ShakeDuration => _shakeDuration;

        /// <summary>Largest shake distance at the start, in world units.</summary>
        public float ShakeAmplitude => _shakeAmplitude;

        /// <summary>Time between two random shake positions, in seconds.</summary>
        public float ShakeInterval => _shakeInterval;

        /// <summary>Total time of the hit feedback: the longest of the flash + tint and the shake.</summary>
        public float TotalDuration => Mathf.Max(_flashDuration + _tintDuration, _shakeDuration);
    }
}
