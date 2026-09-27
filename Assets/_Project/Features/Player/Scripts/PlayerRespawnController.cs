using Ashar.Core;
using Ashar.Environment;
using UnityEngine;

namespace Ashar.Player
{
    /// <summary>
    /// Handles the life of the ship on the screen: the explosion when it is hit, the reappearance with a short blinking
    /// protection, and the disappearance at game over.
    /// RESPONSIBILITIES: put the ship back at the bottom centre when a life is lost, make it untouchable for a few seconds
    /// (blinking so the player sees it), hide and freeze it when the game is over, and bring it back after a continue.
    /// HOW IT WORKS: it listens to GameEvents. OnPlayerRespawnRequested makes the explosion where the ship is, then moves it
    /// and starts the protection timer; OnGameStateChanged to GameOver makes the explosion and turns the ship off. While the ship is "off", its flying
    /// scripts are disabled and its picture and colliders are hidden, but this script keeps running so it can bring the ship back.
    /// WHY: separating this from PlayerHealthController keeps hit detection simple. The rules of lives and game over stay in
    /// GameSession; this script only shows them on the ship.
    /// </summary>
    public class PlayerRespawnController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Play area: the ship reappears at its bottom centre.")]
        private PlayAreaController _playArea;

        [SerializeField, Tooltip("Global tuning values: how long the ship is protected after it reappears.")]
        private GameBalanceData _balance;

        [SerializeField, Tooltip("Renderer of the ship picture (the Visual child), made to blink during the protection.")]
        private SpriteRenderer _visual;

        [SerializeField, Tooltip("Optional. Scene object that holds the effects (Runtime/FX). Set it on the scene instance.")]
        private Transform _fxParent;

        [SerializeField, Tooltip("Optional. Explosion created where the ship is hit.")]
        private GameObject _deathEffect;

        [SerializeField, Tooltip("Scripts that make the ship fly and fire. They are switched off while the ship is hidden (game over).")]
        private Behaviour[] _flightScripts;

        [SerializeField, Tooltip("Objects hidden while the ship is off: the picture, the hitbox, the graze zone, the trail.")]
        private GameObject[] _hiddenWhileOff;

        [Header("Respawn")]
        [SerializeField, Min(0f), Tooltip("Distance between the bottom edge of the screen and the reappearing ship, in world units.")]
        private float _respawnHeight = 1.8f;

        [SerializeField, Min(0.02f), Tooltip("Length of one blink cycle during the protection, in seconds.")]
        private float _blinkPeriod = 0.14f;

        [SerializeField, Range(0f, 1f), Tooltip("Opacity of the ship at the faint moment of a blink (1 = no blinking).")]
        private float _blinkFaintAlpha = 0.3f;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: seconds of protection left.")]
        private float _protectionLeft;

        [SerializeField, Tooltip("Read-only: true while the ship is hidden (game over).")]
        private bool _isOff;

        private float _protectionTime; // Seconds since the protection started, for the blink.

        /// <summary>True while the ship is protected after reappearing: it cannot be hit.</summary>
        public bool IsInvulnerable => _protectionLeft > 0f;

        /// <summary>Checks the references, and disables the component if one is missing.</summary>
        private void Awake()
        {
            if (_playArea == null || _balance == null || _visual == null)
            {
                Debug.LogError($"{nameof(PlayerRespawnController)} on '{name}' is missing its play area, balance data or visual. Respawn disabled.", this);
                enabled = false;
            }
        }

        /// <summary>Starts listening to the game events.</summary>
        private void OnEnable()
        {
            GameEvents.OnPlayerRespawnRequested += HandleRespawnRequested;
            GameEvents.OnGameStateChanged += HandleStateChanged;
        }

        /// <summary>Stops listening and makes the ship fully visible again.</summary>
        private void OnDisable()
        {
            GameEvents.OnPlayerRespawnRequested -= HandleRespawnRequested;
            GameEvents.OnGameStateChanged -= HandleStateChanged;
            SetAlpha(1f);
        }

        /// <summary>Counts the protection down and blinks the ship while it lasts.</summary>
        private void Update()
        {
            if (_protectionLeft <= 0f)
            {
                return;
            }

            _protectionLeft -= Time.deltaTime;
            _protectionTime += Time.deltaTime;
            SetAlpha(_protectionLeft > 0f ? ComputeBlinkAlpha(_protectionTime, _blinkPeriod, _blinkFaintAlpha) : 1f);
        }

        /// <summary>
        /// Creates the explosion where the ship is. It is done here and not on the hit event, because the ship is moved by the
        /// respawn request that follows the hit at once: the explosion must be made before that move.
        /// </summary>
        private void Explode()
        {
            if (_deathEffect != null)
            {
                Instantiate(_deathEffect, transform.position, Quaternion.identity, _fxParent);
            }
        }

        /// <summary>Puts the ship back at the bottom centre, shows it and starts the protection.</summary>
        private void HandleRespawnRequested()
        {
            if (!_isOff)
            {
                Explode(); // A ship that was hidden (after a game over) has nothing to explode.
            }

            SetOff(false);

            Rect screen = _playArea.ScreenBounds;
            transform.position = new Vector3(screen.center.x, screen.yMin + _respawnHeight, transform.position.z);

            _protectionLeft = _balance.RespawnInvulnTime;
            _protectionTime = 0f;
        }

        /// <summary>Turns the ship off when the game is over. (A continue brings it back through the respawn request.)</summary>
        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
            {
                if (!_isOff)
                {
                    Explode();
                }

                _protectionLeft = 0f;
                SetAlpha(1f);
                SetOff(true);
            }
        }

        /// <summary>Hides or shows the ship, and switches its flying scripts off or on.</summary>
        private void SetOff(bool off)
        {
            _isOff = off;
            foreach (Behaviour script in _flightScripts)
            {
                if (script != null)
                {
                    script.enabled = !off;
                }
            }

            foreach (GameObject part in _hiddenWhileOff)
            {
                if (part != null)
                {
                    part.SetActive(!off);
                }
            }
        }

        /// <summary>Sets the opacity of the ship picture.</summary>
        private void SetAlpha(float alpha)
        {
            Color color = _visual.color;
            color.a = alpha;
            _visual.color = color;
        }

        /// <summary>
        /// Returns the opacity of the ship at a given time of its protection: faint during the first half of each blink
        /// cycle, fully visible during the second half. Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static float ComputeBlinkAlpha(float elapsed, float period, float faintAlpha)
        {
            if (period <= 0f)
            {
                return 1f;
            }

            return Mathf.Repeat(elapsed, period) < period * 0.5f ? faintAlpha : 1f;
        }
    }
}
