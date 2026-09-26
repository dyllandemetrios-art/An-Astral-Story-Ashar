using Ashar.Environment;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ashar.Player
{
    /// <summary>
    /// Moves the player ship in 8 directions and keeps it inside the play area.
    /// RESPONSIBILITIES: read the Move action, turn it into a velocity, and clamp the new position to the play area.
    /// HOW IT WORKS: the Move action gives a direction between (-1,-1) and (1,1). The keyboard composite is already
    /// normalised, and ClampMagnitude(input, 1) also caps a gamepad stick pushed into a corner, so a diagonal is
    /// never faster than a straight line. Speed comes from PlayerShipData and the limits from PlayAreaController.
    /// WHY: the calculation is in static methods with no Unity state, so it can be unit-tested.
    /// </summary>
    public class PlayerMovementController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField, Tooltip("Tuning values of the ship. The speed is read from here every frame.")]
        private PlayerShipData _shipData;

        [Header("References")]
        [SerializeField, Tooltip("Play area that limits the movement. Set it on the scene instance: a prefab cannot point to a scene object.")]
        private PlayAreaController _playArea;

        [SerializeField, Tooltip("Move action (Vector2) of the AsharControls input asset.")]
        private InputActionReference _moveAction;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: velocity applied during the last frame, in world units per second.")]
        private Vector2 _currentVelocity;

        /// <summary>Checks that every reference is set, and disables the component if one is missing.</summary>
        private void Awake()
        {
            if (_shipData == null || _playArea == null || _moveAction == null)
            {
                Debug.LogError($"{nameof(PlayerMovementController)} on '{name}' is missing its ship data, play area or move action. Movement disabled.", this);
                enabled = false;
            }
        }

        /// <summary>Makes sure the Move action is enabled.</summary>
        private void OnEnable()
        {
            // The action is shared with other player components, so it is deliberately never disabled here:
            // turning it off in OnDisable would also cut the input of every other reader.
            if (_moveAction != null)
            {
                _moveAction.action.Enable();
            }
        }

        /// <summary>Reads the input and moves the ship, once per frame.</summary>
        private void Update()
        {
            Vector2 input = _moveAction.action.ReadValue<Vector2>();
            Vector2 position = transform.position;
            Vector2 next = ComputeNextPosition(position, input, _shipData.MoveSpeed, Time.deltaTime, _playArea.Bounds);

            _currentVelocity = Time.deltaTime > 0f ? (next - position) / Time.deltaTime : Vector2.zero;
            transform.position = new Vector3(next.x, next.y, transform.position.z);
        }

        /// <summary>
        /// Turns an input direction into a velocity. The direction is capped at length 1, so a diagonal never
        /// exceeds the ship speed, and a gamepad stick pushed halfway gives half the speed.
        /// </summary>
        public static Vector2 ComputeVelocity(Vector2 input, float speed)
        {
            return Vector2.ClampMagnitude(input, 1f) * speed;
        }

        /// <summary>Returns where the ship ends up after one frame of movement, kept inside the given bounds.</summary>
        public static Vector2 ComputeNextPosition(Vector2 position, Vector2 input, float speed, float deltaTime, Rect bounds)
        {
            Vector2 moved = position + ComputeVelocity(input, speed) * deltaTime;
            return PlayAreaController.ClampToRect(bounds, moved);
        }
    }
}
