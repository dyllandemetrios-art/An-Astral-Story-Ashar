using Ashar.Core;
using Ashar.Environment;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ashar.Player
{
    /// <summary>
    /// Short burst of movement that makes the ship briefly invulnerable.
    /// RESPONSIBILITIES: start a dash on the Dash button, move the ship during it, keep the cooldown and the
    /// invulnerability time, and show a provisional trail.
    /// HOW IT WORKS: when Dash is pressed and the cooldown is over, the direction is fixed (the Move input, or up
    /// if there is none). For dashDuration seconds the ship then travels dashDistance in that direction, kept
    /// inside the play area. The cooldown and the invulnerability both count from the start of the dash.
    /// While dashing, PlayerMovementController stands aside, so only one script moves the ship.
    /// WHY: [DefaultExecutionOrder(-10)] makes this script run before PlayerMovementController each frame, so
    /// the movement script always sees an up-to-date IsDashing. Values come from PlayerShipData.
    /// </summary>
    [DefaultExecutionOrder(-10)]
    public class PlayerDashController : MonoBehaviour
    {
        private const float NoInputThreshold = 0.1f; // Below this length the Move input counts as "no direction".

        [Header("Data")]
        [SerializeField, Tooltip("Tuning values of the ship: dash distance, duration, invulnerability and cooldown.")]
        private PlayerShipData _shipData;

        [Header("References")]
        [SerializeField, Tooltip("Play area that limits the dash. Set it on the scene instance: a prefab cannot point to a scene object.")]
        private PlayAreaController _playArea;

        [SerializeField, Tooltip("Dash action (Button) of the AsharControls input asset.")]
        private InputActionReference _dashAction;

        [SerializeField, Tooltip("Move action (Vector2) of the AsharControls input asset, read to choose the dash direction.")]
        private InputActionReference _moveAction;

        [SerializeField, Tooltip("Optional provisional trail, switched on only while dashing.")]
        private TrailRenderer _trail;

        [Header("Debug")]
        [SerializeField, Tooltip("Dash again as soon as the cooldown allows it, without pressing the button. To check the cooldown; leave off otherwise.")]
        private bool _debugAlwaysDash;

        [SerializeField, Tooltip("Read-only: true while the ship is travelling in a dash.")]
        private bool _isDashing;

        [SerializeField, Tooltip("Read-only: seconds of invulnerability left.")]
        private float _invulnerabilityLeft;

        [SerializeField, Tooltip("Read-only: seconds left before the next dash is allowed.")]
        private float _cooldownLeft;

        private Vector2 _direction = Vector2.up; // Direction fixed at the start of the current dash.
        private float _dashTimeLeft;             // Seconds of travel left in the current dash.

        /// <summary>True while the ship is travelling in a dash.</summary>
        public bool IsDashing => _isDashing;

        /// <summary>True while the ship cannot be hit (during the dash and a short time after it).</summary>
        public bool IsInvulnerable => _invulnerabilityLeft > 0f;

        /// <summary>Checks that every reference is set, and disables the component if one is missing.</summary>
        private void Awake()
        {
            if (_shipData == null || _playArea == null || _dashAction == null || _moveAction == null)
            {
                Debug.LogError($"{nameof(PlayerDashController)} on '{name}' is missing its ship data, play area, dash action or move action. Dash disabled.", this);
                enabled = false;
            }
        }

        /// <summary>Makes sure the actions are enabled and the trail is off.</summary>
        private void OnEnable()
        {
            // The actions are shared with other player components, so they are deliberately never disabled here.
            if (_dashAction != null)
            {
                _dashAction.action.Enable();
            }

            if (_moveAction != null)
            {
                _moveAction.action.Enable();
            }

            SetTrail(false);
        }

        /// <summary>Runs the timers, starts a dash when asked and moves the ship during it.</summary>
        private void Update()
        {
            float deltaTime = Time.deltaTime;
            _cooldownLeft = Mathf.Max(0f, _cooldownLeft - deltaTime);
            _invulnerabilityLeft = Mathf.Max(0f, _invulnerabilityLeft - deltaTime);

            bool wanted = _debugAlwaysDash || _dashAction.action.WasPressedThisFrame();
            if (wanted && CanStartDash(_cooldownLeft, _isDashing))
            {
                StartDash(_moveAction.action.ReadValue<Vector2>());
            }

            if (_isDashing)
            {
                MoveDuringDash(deltaTime);
            }
        }

        /// <summary>Fixes the direction and starts the timers of a new dash.</summary>
        private void StartDash(Vector2 moveInput)
        {
            _direction = ComputeDashDirection(moveInput);
            _dashTimeLeft = _shipData.DashDuration;
            _invulnerabilityLeft = _shipData.DashInvulnTime;
            _cooldownLeft = _shipData.DashCooldown;
            _isDashing = true;
            SetTrail(true);
            GameEvents.RaisePlayerDashed();
        }

        /// <summary>Moves the ship along the dash direction for this frame, and ends the dash when its time is up.</summary>
        private void MoveDuringDash(float deltaTime)
        {
            float step = Mathf.Min(deltaTime, _dashTimeLeft); // Never travel more than the time left in the dash.
            float speed = _shipData.DashDistance / _shipData.DashDuration;

            Vector2 next = ComputeDashPosition(transform.position, _direction, speed, step, _playArea.Bounds);
            transform.position = new Vector3(next.x, next.y, transform.position.z);

            _dashTimeLeft -= step;
            if (_dashTimeLeft <= 0f)
            {
                _isDashing = false;
                SetTrail(false);
            }
        }

        /// <summary>Switches the provisional trail on or off, if there is one.</summary>
        private void SetTrail(bool on)
        {
            if (_trail != null)
            {
                _trail.emitting = on;
            }
        }

        /// <summary>
        /// Chooses the dash direction: the Move input, normalised, or straight up when there is no input.
        /// Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static Vector2 ComputeDashDirection(Vector2 moveInput)
        {
            return moveInput.magnitude < NoInputThreshold ? Vector2.up : moveInput.normalized;
        }

        /// <summary>True when a new dash may start: the cooldown is over and no dash is running.</summary>
        public static bool CanStartDash(float cooldownLeft, bool isDashing)
        {
            return cooldownLeft <= 0f && !isDashing;
        }

        /// <summary>Returns where the ship is after travelling for a given time at the dash speed, kept inside the bounds.</summary>
        public static Vector2 ComputeDashPosition(Vector2 position, Vector2 direction, float speed, float travelTime, Rect bounds)
        {
            return PlayAreaController.ClampToRect(bounds, position + direction * (speed * travelTime));
        }
    }
}
