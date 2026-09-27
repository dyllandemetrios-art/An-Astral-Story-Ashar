using UnityEngine;
using UnityEngine.InputSystem;

namespace Ashar.Player
{
    /// <summary>
    /// Tilts the ship picture sideways while it moves left or right, like a plane banking into a turn.
    /// RESPONSIBILITIES: turn the horizontal part of the Move input into a Z rotation of the ship's Visual child, eased
    /// in and out instead of snapping.
    /// HOW IT WORKS: each frame it reads the horizontal Move input, works out the angle it should lean towards (full tilt
    /// at full stick, no tilt with no horizontal input), and turns the current angle towards it at a fixed speed.
    /// WHY: the purchased sprite (Plane 07) has no hand-drawn banking frames, only Planes 01, 02, 03 and 09 do (folders
    /// with a "_spin" animation). Rotating the sprite is the common stand-in when there is no dedicated art; it costs no
    /// new sprite. Caveat: a Point-filtered sprite rotated to an in-between angle is no longer perfectly axis-aligned, so
    /// its edges are not as crisp as the rest of the pixel art (spec §8 asks for none of that) — worth judging on screen,
    /// and easy to drop or replace with real frames (switch to Plane 01/02/03/09) if it looks wrong.
    /// </summary>
    public class PlayerBankController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("The ship's picture, tilted by this script. Never the root: the hitbox and graze zone stay axis-aligned.")]
        private Transform _visual;

        [SerializeField, Tooltip("Move action (Vector2) of the AsharControls input asset.")]
        private InputActionReference _moveAction;

        [Header("Settings")]
        [SerializeField, Range(0f, 45f), Tooltip("Tilt angle at full left or right input, in degrees.")]
        private float _maxBankAngle = 14f;

        [SerializeField, Min(1f), Tooltip("How fast the tilt reaches its target angle, in degrees per second.")]
        private float _turnSpeed = 480f;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: current tilt angle, in degrees.")]
        private float _currentAngle;

        /// <summary>Checks that every reference is set, and disables the component if one is missing.</summary>
        private void Awake()
        {
            if (_visual == null || _moveAction == null)
            {
                Debug.LogError($"{nameof(PlayerBankController)} on '{name}' is missing its visual or move action. Banking disabled.", this);
                enabled = false;
            }
        }

        /// <summary>Makes sure the Move action is enabled.</summary>
        private void OnEnable()
        {
            // The action is shared with other player components, so it is deliberately never disabled here.
            if (_moveAction != null)
            {
                _moveAction.action.Enable();
            }
        }

        /// <summary>Puts the picture back level when this script stops running, so it is never left tilted.</summary>
        private void OnDisable()
        {
            if (_visual != null)
            {
                _visual.localRotation = Quaternion.identity;
            }
        }

        /// <summary>Eases the tilt towards the angle the current horizontal input asks for.</summary>
        private void Update()
        {
            float horizontalInput = _moveAction.action.ReadValue<Vector2>().x;
            float targetAngle = ComputeTargetBankAngle(horizontalInput, _maxBankAngle);
            _currentAngle = ComputeNextAngle(_currentAngle, targetAngle, _turnSpeed * Time.deltaTime);
            _visual.localRotation = Quaternion.Euler(0f, 0f, _currentAngle);
        }

        /// <summary>
        /// Returns the tilt angle a given horizontal input asks for: full tilt at full input, none with no input. Moving
        /// right (positive input) leans the nose to the right, hence the negative sign (a positive Z rotation turns
        /// anticlockwise). Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static float ComputeTargetBankAngle(float horizontalInput, float maxBankAngle)
        {
            return -Mathf.Clamp(horizontalInput, -1f, 1f) * maxBankAngle;
        }

        /// <summary>Moves an angle towards a target by at most maxDelta degrees, without overshooting it.</summary>
        public static float ComputeNextAngle(float currentAngle, float targetAngle, float maxDeltaDegrees)
        {
            return Mathf.MoveTowards(currentAngle, targetAngle, maxDeltaDegrees);
        }
    }
}
