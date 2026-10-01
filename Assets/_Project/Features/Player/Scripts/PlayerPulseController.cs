using System.Collections.Generic;
using Ashar.Combat;
using Ashar.Core;
using Ashar.Enemies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ashar.Player
{
    /// <summary>Whether the Pulse may be activated by a press (spec E5-06).</summary>
    public enum PulseState
    {
        /// <summary>Charging (the mandatory first 45 s) or recharging after a use: not usable yet.</summary>
        Recharging,

        /// <summary>May be activated by a Pulse press.</summary>
        Ready,
    }

    /// <summary>
    /// The player's Pulse: one press destroys small enemies and stuns heavy units/bosses in a radius around the ship,
    /// and erases marked enemy bullets in range (spec E5-06; the design calls this the same power as the "hacking
    /// impulse").
    /// RESPONSIBILITIES: read the Pulse action, run the Recharging/Ready state machine (45 s before the first use and
    /// after every use), apply the wave to enemies and their bullets in range, and give a small graze discount on the
    /// recharge, never on the mandatory initial charge.
    /// HOW IT WORKS: unlike the shield, a Pulse has no held "Active" state: a press while Ready fires the wave once
    /// and goes straight to Recharging. Time.deltaTime drives the countdown, so the pause menu (E3-06) and the
    /// dialogue controller (E3-02) freeze it for free by disabling this component, exactly like the dash and the
    /// shield. GameOver and MissionEnd are the two states nothing else disables this component for, so they are
    /// checked explicitly. A life lost never resets or shortens the recharge: unlike the shield, a Pulse has nothing
    /// "in progress" to cut short, so there is no forced-recharge path to write; only a real activation restarts the
    /// clock, so death never grants a free recharge.
    /// WHY: one small state machine instead of a generic capability/cooldown framework, matching the shield's pattern
    /// (E5-05).
    /// </summary>
    public class PlayerPulseController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField, Tooltip("Tuning values of the ship: Pulse radius, cooldown, stun duration and graze discount.")]
        private PlayerShipData _shipData;

        [Header("References")]
        [SerializeField, Tooltip("Pulse action (Button) of the AsharControls input asset (Left/Right Ctrl, gamepad North/Y).")]
        private InputActionReference _pulseAction;

        [SerializeField, Tooltip("Optional. One-shot effect created at the ship's position every time the Pulse is emitted, hit or not (electric burst from the project's VFX pack).")]
        private GameObject _effectPrefab;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: current state of the Pulse.")]
        private PulseState _state = PulseState.Recharging;

        [SerializeField, Tooltip("Read-only: seconds left before Ready (0 once Ready).")]
        private float _timeLeft;

        [SerializeField, Tooltip("Read-only: true while the current recharge is the mandatory initial one, which the graze discount never shortens.")]
        private bool _isInitialCharge = true;

        /// <summary>True while the Pulse may be activated by a press.</summary>
        public bool IsReady => _state == PulseState.Ready;

        /// <summary>Seconds left before Ready (0 once Ready).</summary>
        public float TimeLeft => _timeLeft;

        /// <summary>Checks the required references and starts the mandatory initial charge.</summary>
        private void Awake()
        {
            if (_shipData == null || _pulseAction == null)
            {
                Debug.LogError($"{nameof(PlayerPulseController)} on '{name}' is missing its ship data or Pulse action. Pulse disabled.", this);
                enabled = false;
                return;
            }

            // A fresh scene always starts charging, regardless of whatever the Debug fields show in the saved scene.
            _state = PulseState.Recharging;
            _timeLeft = _shipData.PulseCooldown;
            _isInitialCharge = true;
        }

        /// <summary>Enables the action and starts listening to grazes for the recharge discount.</summary>
        private void OnEnable()
        {
            if (_pulseAction != null)
            {
                _pulseAction.action.Enable();
            }

            GameEvents.OnPlayerGrazed += HandlePlayerGrazed;
        }

        /// <summary>Stops listening.</summary>
        private void OnDisable()
        {
            GameEvents.OnPlayerGrazed -= HandlePlayerGrazed;
        }

        /// <summary>
        /// Advances the recharge and reads the Pulse press. GameOver and MissionEnd are the only states that do not
        /// already disable this component elsewhere (pause and dialogue do), so they are skipped explicitly here.
        /// </summary>
        private void Update()
        {
            GameState state = GameSession.Current != null ? GameSession.Current.State : GameState.Gameplay;
            if (state == GameState.GameOver || state == GameState.MissionEnd)
            {
                return;
            }

            (_state, _timeLeft) = Tick(_state, _timeLeft, Time.deltaTime);

            if (CanActivate(_state) && _pulseAction != null && _pulseAction.action.WasPressedThisFrame())
            {
                Activate();
            }
        }

        /// <summary>Emits the wave, then starts the post-use recharge. A press consumes the charge even if nothing is in range.</summary>
        private void Activate()
        {
            EmitWave();
            _state = PulseState.Recharging;
            _timeLeft = _shipData.PulseCooldown;
            _isInitialCharge = false;

            if (_effectPrefab != null)
            {
                Instantiate(_effectPrefab, transform.position, Quaternion.identity);
            }

            GameEvents.RaisePlayerPulsed();
        }

        /// <summary>
        /// Applies the Pulse to every enemy and erasable enemy bullet in range: small units are destroyed, heavy
        /// units and bosses are stunned, and marked bullets disappear. Each target is affected at most once even if
        /// several of its colliders overlap the wave (spec: deduplicate multi-collider targets). Allies, captives and
        /// player/ally bullets are never touched: they simply are not on the Enemy or EnemyBullet layers this reads.
        /// </summary>
        private void EmitWave()
        {
            Vector2 origin = transform.position;
            float radius = _shipData.PulseRadius;

            var handledEnemies = new HashSet<EnemyController>();
            foreach (Collider2D hit in Physics2D.OverlapCircleAll(origin, radius, 1 << Layers.Enemy))
            {
                EnemyController enemy = hit.GetComponentInParent<EnemyController>();
                if (enemy == null || !handledEnemies.Add(enemy))
                {
                    continue;
                }

                if (enemy.Data.PulseResponse == EnemyPulseResponse.Destroy)
                {
                    enemy.Kill();
                }
                else
                {
                    enemy.Stun(_shipData.PulseStunDuration);
                }
            }

            var handledBullets = new HashSet<ProjectileController>();
            foreach (Collider2D hit in Physics2D.OverlapCircleAll(origin, radius, 1 << Layers.EnemyBullet))
            {
                ProjectileController bullet = hit.GetComponentInParent<ProjectileController>();
                if (bullet == null || !bullet.PulseErasable || !handledBullets.Add(bullet))
                {
                    continue;
                }

                Destroy(bullet.gameObject);
            }
        }

        /// <summary>An admissible graze shortens the current recharge, floored at zero; the initial charge is never touched.</summary>
        private void HandlePlayerGrazed(int points)
        {
            if (_isInitialCharge)
            {
                return;
            }

            (_state, _timeLeft) = ApplyGrazeDiscount(_state, _timeLeft, _shipData.PulseGrazeDiscount);
        }

        /// <summary>True when a Pulse press may fire the wave: only while Ready.</summary>
        public static bool CanActivate(PulseState state)
        {
            return state == PulseState.Ready;
        }

        /// <summary>
        /// Advances the recharge by deltaTime, becoming Ready once it reaches zero and staying Ready afterwards.
        /// Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static (PulseState state, float timeLeft) Tick(PulseState state, float timeLeft, float deltaTime)
        {
            if (state == PulseState.Ready)
            {
                return (PulseState.Ready, 0f);
            }

            timeLeft -= deltaTime;
            return timeLeft <= 0f ? (PulseState.Ready, 0f) : (PulseState.Recharging, timeLeft);
        }

        /// <summary>
        /// Returns the state and time left after one admissible graze shortens the recharge, floored at zero (never
        /// applied while already Ready). Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static (PulseState state, float timeLeft) ApplyGrazeDiscount(PulseState state, float timeLeft, float discount)
        {
            if (state != PulseState.Recharging)
            {
                return (state, timeLeft);
            }

            float discounted = Mathf.Max(0f, timeLeft - discount);
            return discounted <= 0f ? (PulseState.Ready, 0f) : (PulseState.Recharging, discounted);
        }
    }
}
