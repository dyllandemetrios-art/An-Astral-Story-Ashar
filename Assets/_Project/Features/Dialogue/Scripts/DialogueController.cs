using System;
using Ashar.Core;
using Ashar.Player;
using Ashar.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ashar.Dialogue
{
    /// <summary>
    /// Plays a linear dialogue sequence between combats: the world keeps flying, only the player's controls and the
    /// enemies wait (spec E3-02).
    /// RESPONSIBILITIES: show one line at a time on UI/Submit, put the player ship on a calm presentation flight while
    /// its controls are off, and tell the caller (MissionRunnerController) when the exchange is over so combat resumes.
    /// HOW IT WORKS: Play() is called directly by whoever wants a sequence shown (today, only MissionRunnerController
    /// on a Dialogue wave event) with a callback to invoke once done. While active, this is the only script driving
    /// the UI/Submit action: DebugRadioDisplayDriver is switched off in the same setup. Never touches
    /// Time.timeScale; the pause menu (not built yet) is expected to call SetInputSuspended(true) so its own
    /// Submit presses do not leak into the dialogue (spec E3-02, boundary with E3-06).
    /// WHY: a single small controller keeps the state machine (open, advance, close) in one place, reusing
    /// GameSession and GameEvents instead of a generic cutscene system.
    /// </summary>
    public class DialogueController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("View that shows the current line (E3-01). Reused as-is: this story never changes it.")]
        private UIRadioDisplayView _view;

        [SerializeField, Tooltip("Submit action (Button) of the UI action map. Each press shows the next line.")]
        private InputActionReference _advanceAction;

        [SerializeField, Tooltip("Player ship transform, moved to the cruise position while a dialogue plays.")]
        private Transform _player;

        [SerializeField, Tooltip("Scene marker for the presentation flight position. Move it in the Scene view to tune where the ship waits.")]
        private Transform _cruisePosition;

        [SerializeField, Tooltip("Player's movement script, switched off while a dialogue plays.")]
        private PlayerMovementController _movement;

        [SerializeField, Tooltip("Player's fire script, switched off while a dialogue plays and kept off until the button is released, so the final Submit press (shared with Fire on gamepad) cannot fire a shot.")]
        private PlayerShootController _shoot;

        [SerializeField, Tooltip("Player's dash script, switched off while a dialogue plays.")]
        private PlayerDashController _dash;

        [SerializeField, Tooltip("Optional. Player's shield script (spec E5-05), switched off while a dialogue plays so its timers freeze exactly like the dash's.")]
        private PlayerShieldController _shield;

        [Header("Settings")]
        [SerializeField, Min(0f), Tooltip("Speed at which the ship travels to the cruise position, in world units per second.")]
        private float _cruiseApproachSpeed = 3f;

        [SerializeField, Tooltip("Language read at the start of each exchange (spec: applied to the next exchange, not mid-line). No language menu exists yet (E3-06): change this in the Inspector, or call SetLanguage, to test the other language.")]
        private DialogueLanguage _language = DialogueLanguage.French;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: true while a sequence is showing.")]
        private bool _active;

        [SerializeField, Tooltip("Read-only: index of the line currently shown.")]
        private int _lineIndex;

        private DialogueData _data;             // Sequence currently playing, or null.
        private Action _onFinished;              // Callback given to Play(), invoked once when the sequence closes normally.
        private bool _skipInputThisFrame;        // True for the one frame a sequence opens: an already-held Submit must not skip line 1.
        private bool _inputSuspended;            // Set by SetInputSuspended(true): a future pause menu keeps its own Submit from advancing the line.

        /// <summary>True while a sequence is showing.</summary>
        public bool IsActive => _active;

        /// <summary>Makes sure the Submit action is enabled.</summary>
        private void OnEnable()
        {
            if (_advanceAction != null)
            {
                _advanceAction.action.Enable();
            }
        }

        /// <summary>
        /// If this component is disabled or destroyed while a sequence is active (mission end or abandon), the view is
        /// hidden and the state is cleared, but the finished callback is never invoked: the mission is going away, not
        /// resuming combat.
        /// </summary>
        private void OnDisable()
        {
            if (!_active)
            {
                return;
            }

            if (_view != null)
            {
                _view.Hide();
            }

            _active = false;
            _onFinished = null;
            _data = null;
        }

        /// <summary>
        /// Starts a sequence: on = puts the player on the presentation flight, off = puts the controls back. A request
        /// while a sequence is already active is refused and logged, never replacing the one showing; onFinished is
        /// still called once so the caller is never left waiting forever. A missing or empty sequence is diagnosed the
        /// same way, before any state changes.
        /// </summary>
        public void Play(DialogueData data, Action onFinished)
        {
            if (_active)
            {
                Debug.LogError($"{nameof(DialogueController)}: a dialogue is already playing; the request for '{(data != null ? data.Id : "null")}' is ignored.", this);
                onFinished?.Invoke();
                return;
            }

            if (data == null || data.Lines.Count == 0)
            {
                Debug.LogError($"{nameof(DialogueController)}: missing or empty DialogueData; skipped without changing state.", this);
                onFinished?.Invoke();
                return;
            }

            _data = data;
            _onFinished = onFinished;
            _lineIndex = 0;
            _active = true;
            _skipInputThisFrame = true;

            GameSession.Current?.EnterDialogue();
            GameEvents.RaiseGameStateChanged(GameState.Dialogue);
            SetPlayerControlsEnabled(false);
            ShowCurrentLine();
        }

        /// <summary>A future pause menu calls this so its own Submit presses do not advance the dialogue underneath it.</summary>
        public void SetInputSuspended(bool suspended)
        {
            _inputSuspended = suspended;
        }

        /// <summary>Sets the language used from the next Play() call onward; the line already showing is not reformatted.</summary>
        public void SetLanguage(DialogueLanguage language)
        {
            _language = language;
        }

        /// <summary>Flies the ship toward the cruise position, and advances the line on Submit.</summary>
        private void Update()
        {
            if (!_active)
            {
                return;
            }

            FlyTowardCruisePosition();

            if (_skipInputThisFrame)
            {
                _skipInputThisFrame = false; // The frame a sequence opens never reads input, even if Submit is already held.
                return;
            }

            if (_inputSuspended || _advanceAction == null || !_advanceAction.action.WasPressedThisFrame())
            {
                return;
            }

            Advance();
        }

        /// <summary>Moves the player toward the cruise marker at a constant speed, if both are set.</summary>
        private void FlyTowardCruisePosition()
        {
            if (_player == null || _cruisePosition == null)
            {
                return;
            }

            _player.position = Vector3.MoveTowards(_player.position, _cruisePosition.position, _cruiseApproachSpeed * Time.deltaTime);
        }

        /// <summary>Shows the next line, or closes the sequence once the last one has been validated.</summary>
        private void Advance()
        {
            _lineIndex++;
            if (_lineIndex >= _data.Lines.Count)
            {
                Finish();
            }
            else
            {
                ShowCurrentLine();
            }
        }

        /// <summary>Displays the line at _lineIndex in the current language.</summary>
        private void ShowCurrentLine()
        {
            DialogueLine line = _data.Lines[_lineIndex];
            _view.Show(line.SpeakerName, line.GetText(_language));
        }

        /// <summary>Hides the view, restores the player's controls and returns to normal play, then tells the caller.</summary>
        private void Finish()
        {
            _view.Hide();
            _active = false;
            Action onFinished = _onFinished;
            _onFinished = null;
            _data = null;

            GameSession.Current?.ExitDialogue();
            GameEvents.RaiseGameStateChanged(GameState.Gameplay);
            SetPlayerControlsEnabled(true);
            if (_shoot != null)
            {
                _shoot.SuppressUntilReleased();
            }

            onFinished?.Invoke();
        }

        /// <summary>Switches the player's flight and combat scripts on or off together.</summary>
        private void SetPlayerControlsEnabled(bool enabledState)
        {
            if (_movement != null)
            {
                _movement.enabled = enabledState;
            }

            if (_shoot != null)
            {
                _shoot.enabled = enabledState;
            }

            if (_dash != null)
            {
                _dash.enabled = enabledState;
            }

            if (_shield != null)
            {
                _shield.enabled = enabledState;
            }
        }
    }
}
