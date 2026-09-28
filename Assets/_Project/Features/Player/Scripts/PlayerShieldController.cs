using Ashar.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ashar.Player
{
    /// <summary>
    /// The three states of the ephemeral shield (spec E5-05).
    /// </summary>
    public enum ShieldState
    {
        /// <summary>May be activated by a Shield press.</summary>
        Ready,

        /// <summary>Protecting the ship; ends on its own after the protection time.</summary>
        Active,

        /// <summary>Cannot protect; ends on its own after the cooldown, becoming Ready.</summary>
        Recharging,
    }

    /// <summary>
    /// An independent, always-available shield: one press blocks every impact for a few seconds, then needs to
    /// recharge before it can be used again (spec E5-05).
    /// RESPONSIBILITIES: read the Shield action, run the Ready/Active/Recharging state machine, show the halo only
    /// while Active, and expose IsReady/IsActive/TimeLeft for PlayerHealthController and the HUD/tutorial to read.
    /// HOW IT WORKS: unlike the dash or the generator, this capacity needs no charge or pickup: a new game starts
    /// Ready. Time.deltaTime drives the countdown, so the pause menu (E3-06) and the dialogue controller (E3-02)
    /// freeze it for free by disabling this component the same way they already disable movement/shoot/dash; no
    /// extra pause-awareness is written here. GameOver and MissionEnd are the two states nothing else disables this
    /// component for, so they are checked explicitly. A protection cut short by a forced respawn (a test kill, not a
    /// blocked hit) or by GameOver/MissionEnd jumps straight to a full Recharging instead of leaving a stale Active
    /// halo, per spec; an ordinary life lost while Recharging simply keeps counting down, so death never grants a
    /// free recharge.
    /// WHY: one small state machine instead of a generic capability/cooldown framework, matching the project's dash
    /// and respawn controllers.
    /// </summary>
    public class PlayerShieldController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField, Tooltip("Tuning values of the ship: shield protection time and cooldown.")]
        private PlayerShipData _shipData;

        [Header("References")]
        [SerializeField, Tooltip("Shield action (Button) of the AsharControls input asset (Left/Right Shift, gamepad West).")]
        private InputActionReference _shieldAction;

        [SerializeField, Tooltip("Halo shown only while the shield is Active. Provisional sprite from the ship pack (spec: precise the limit if Dyllan's final art is missing).")]
        private GameObject _halo;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: current state of the shield.")]
        private ShieldState _state = ShieldState.Ready;

        [SerializeField, Tooltip("Read-only: seconds left in the current state (0 while Ready).")]
        private float _timeLeft;

        /// <summary>True while the shield may be activated by a press.</summary>
        public bool IsReady => _state == ShieldState.Ready;

        /// <summary>True while the shield is protecting the ship.</summary>
        public bool IsActive => _state == ShieldState.Active;

        /// <summary>Seconds left in the current state: protection left while Active, recharge left while Recharging, 0 while Ready.</summary>
        public float TimeLeft => _timeLeft;

        /// <summary>Checks that the required references are set, and disables the component if one is missing.</summary>
        private void Awake()
        {
            if (_shipData == null || _shieldAction == null)
            {
                Debug.LogError($"{nameof(PlayerShieldController)} on '{name}' is missing its ship data or shield action. Shield disabled.", this);
                enabled = false;
            }
        }

        /// <summary>Enables the action, starts listening to forced transitions, and makes sure the halo matches the current state.</summary>
        private void OnEnable()
        {
            if (_shieldAction != null)
            {
                _shieldAction.action.Enable();
            }

            GameEvents.OnGameStateChanged += HandleGameStateChanged;
            GameEvents.OnPlayerRespawnRequested += HandleRespawnRequested;
            UpdateHalo();
        }

        /// <summary>
        /// Stops listening. The halo is deliberately left as it is: pause and dialogue disable this component to
        /// freeze the countdown (spec: "halo figé si actif"), so hiding it here would uncover an Active protection
        /// the moment the menu or the dialogue box appears, instead of showing it frozen underneath.
        /// </summary>
        private void OnDisable()
        {
            GameEvents.OnGameStateChanged -= HandleGameStateChanged;
            GameEvents.OnPlayerRespawnRequested -= HandleRespawnRequested;
        }

        /// <summary>
        /// Advances the state machine and reads the Shield press. GameOver and MissionEnd are the only states that do
        /// not already disable this component elsewhere (pause and dialogue do), so they are skipped explicitly here:
        /// no countdown, no activation.
        /// </summary>
        private void Update()
        {
            GameState state = GameSession.Current != null ? GameSession.Current.State : GameState.Gameplay;
            if (state == GameState.GameOver || state == GameState.MissionEnd)
            {
                return;
            }

            (_state, _timeLeft) = Tick(_state, _timeLeft, Time.deltaTime, _shipData.ShieldCooldown);
            UpdateHalo();

            if (CanActivate(_state) && _shieldAction != null && _shieldAction.action.WasPressedThisFrame())
            {
                Activate();
            }
        }

        /// <summary>Starts the protection: halo on, timer set, signal raised for whoever needs to know (e.g. the tutorial).</summary>
        private void Activate()
        {
            _state = ShieldState.Active;
            _timeLeft = _shipData.ShieldProtectionTime;
            UpdateHalo();
            GameEvents.RaisePlayerShielded();
        }

        /// <summary>
        /// A forced respawn (a kill that bypasses the shield's own invulnerability, e.g. a test kill or a hazard) cuts an
        /// Active protection short instead of leaving a stale halo: the shield jumps straight to a full Recharging.
        /// An ordinary respawn while Recharging or Ready changes nothing: the remaining recharge is never reset by dying.
        /// </summary>
        private void HandleRespawnRequested()
        {
            if (_state == ShieldState.Active)
            {
                EnterFullRecharge();
            }
        }

        /// <summary>GameOver or MissionEnd reached while Active: cut the protection short instead of leaving a stale halo.</summary>
        private void HandleGameStateChanged(GameState state)
        {
            if ((state == GameState.GameOver || state == GameState.MissionEnd) && _state == ShieldState.Active)
            {
                EnterFullRecharge();
            }
        }

        /// <summary>Jumps to Recharging at the full cooldown value and updates the halo.</summary>
        private void EnterFullRecharge()
        {
            _state = ShieldState.Recharging;
            _timeLeft = _shipData.ShieldCooldown;
            UpdateHalo();
        }

        /// <summary>Shows the halo only while Active.</summary>
        private void UpdateHalo()
        {
            if (_halo != null)
            {
                _halo.SetActive(_state == ShieldState.Active);
            }
        }

        /// <summary>True when a Shield press may start a new protection: only while Ready.</summary>
        public static bool CanActivate(ShieldState state)
        {
            return state == ShieldState.Ready;
        }

        /// <summary>
        /// Advances the state machine by deltaTime, draining it across as many phase changes as it takes (Active then
        /// Recharging then Ready), so one long frame lands on exactly the same state as many short frames covering the
        /// same total time: a boundary is never granted extra protection nor an extra-long recharge.
        /// Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static (ShieldState state, float timeLeft) Tick(ShieldState state, float timeLeft, float deltaTime, float cooldown)
        {
            float remaining = deltaTime;
            while (remaining > 0f && state != ShieldState.Ready)
            {
                if (timeLeft > remaining)
                {
                    timeLeft -= remaining;
                    remaining = 0f;
                }
                else
                {
                    remaining -= timeLeft;
                    state = state == ShieldState.Active ? ShieldState.Recharging : ShieldState.Ready;
                    timeLeft = state == ShieldState.Recharging ? cooldown : 0f;
                }
            }

            return (state, timeLeft);
        }
    }
}
