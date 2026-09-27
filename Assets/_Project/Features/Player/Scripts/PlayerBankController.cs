using UnityEngine;
using UnityEngine.InputSystem;

namespace Ashar.Player
{
    /// <summary>The three sets of hand-drawn frames the ship can show, chosen by the horizontal input.</summary>
    public enum ShipBankState
    {
        /// <summary>No horizontal input: the plain flight loop.</summary>
        Base,

        /// <summary>Moving left: the left-leaning flight loop.</summary>
        Left,

        /// <summary>Moving right: the right-leaning flight loop.</summary>
        Right,
    }

    /// <summary>
    /// Shows the ship leaning into a turn while it moves left or right, using the pack's own hand-drawn frames.
    /// RESPONSIBILITIES: pick which of the three animated frame sets (level flight, banking left, banking right) to
    /// show from the horizontal Move input, and play that set as a looping animation.
    /// HOW IT WORKS: the purchased sheet for this ship (Plane 07A) is one 20-frame strip: frames 1-4 are the level
    /// flight loop, 5-12 lean left, 13-20 lean right. Each frame is looped at its own speed; switching state restarts
    /// the new loop from its first frame, so a bank never starts mid-pose.
    /// WHY: earlier this was a script-driven rotation of the sprite, because Plane 07 looked like it had no banking
    /// frames; Dyllan pointed out the pack already draws them, which reads far better than a rotated, slightly
    /// aliased Point-filtered sprite. Kept as sprite-swap only: no rotation, no interpolation.
    /// </summary>
    public class PlayerBankController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Renderer of the ship's picture (the Visual child), whose sprite is swapped each frame.")]
        private SpriteRenderer _visual;

        [SerializeField, Tooltip("Move action (Vector2) of the AsharControls input asset.")]
        private InputActionReference _moveAction;

        [Header("Frames")]
        [SerializeField, Tooltip("Level flight loop (Plane 07A frames 1 to 4).")]
        private Sprite[] _baseFrames;

        [SerializeField, Tooltip("Banking-left loop (Plane 07A frames 5 to 12).")]
        private Sprite[] _leftFrames;

        [SerializeField, Tooltip("Banking-right loop (Plane 07A frames 13 to 20).")]
        private Sprite[] _rightFrames;

        [SerializeField, Min(0.01f), Tooltip("Horizontal input below this size counts as no input: keeps a light touch on the stick from flickering between level flight and a bank.")]
        private float _deadzone = 0.15f;

        [SerializeField, Min(1f), Tooltip("How many frames of the animation are shown per second.")]
        private float _framesPerSecond = 12f;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: which frame set is playing right now.")]
        private ShipBankState _currentState;

        private float _stateTime; // Seconds since the current state started, so its loop always starts at frame 0.

        /// <summary>Checks that every reference is set, and disables the component if one is missing.</summary>
        private void Awake()
        {
            if (_visual == null || _moveAction == null || IsEmpty(_baseFrames) || IsEmpty(_leftFrames) || IsEmpty(_rightFrames))
            {
                Debug.LogError($"{nameof(PlayerBankController)} on '{name}' is missing its visual, move action or one of its frame sets. Banking disabled.", this);
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

        /// <summary>Picks the frame set from the current horizontal input and plays it.</summary>
        private void Update()
        {
            float horizontalInput = _moveAction.action.ReadValue<Vector2>().x;
            ShipBankState wantedState = ComputeBankState(horizontalInput, _deadzone);

            if (wantedState != _currentState)
            {
                _currentState = wantedState;
                _stateTime = 0f;
            }
            else
            {
                _stateTime += Time.deltaTime;
            }

            Sprite[] frames = FramesFor(_currentState);
            int index = LoopFrameIndex(_stateTime, _framesPerSecond, frames.Length);
            _visual.sprite = frames[index];
        }

        /// <summary>Returns the frame set for a bank state.</summary>
        private Sprite[] FramesFor(ShipBankState state)
        {
            switch (state)
            {
                case ShipBankState.Left: return _leftFrames;
                case ShipBankState.Right: return _rightFrames;
                default: return _baseFrames;
            }
        }

        /// <summary>True when an array is missing or has no sprite in it.</summary>
        private static bool IsEmpty(Sprite[] frames)
        {
            return frames == null || frames.Length == 0;
        }

        /// <summary>
        /// Returns which way the ship should lean for a given horizontal input: left, right, or level flight inside the
        /// deadzone. Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static ShipBankState ComputeBankState(float horizontalInput, float deadzone)
        {
            if (horizontalInput <= -deadzone)
            {
                return ShipBankState.Left;
            }

            return horizontalInput >= deadzone ? ShipBankState.Right : ShipBankState.Base;
        }

        /// <summary>
        /// Returns which frame of a looping animation is due at a given time. Static and free of Unity state so it can
        /// be unit-tested. A frame count of 0 or less always returns frame 0, so a caller never indexes out of range.
        /// </summary>
        public static int LoopFrameIndex(float elapsed, float framesPerSecond, int frameCount)
        {
            if (frameCount <= 0)
            {
                return 0;
            }

            int index = (int)(elapsed * framesPerSecond);
            return index % frameCount;
        }
    }
}
