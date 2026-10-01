using Ashar.Core;
using Ashar.Dialogue;
using Ashar.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Ashar.Menu
{
    /// <summary>
    /// The in-game pause menu (spec E3-06): a voluntary pause distinct from the Dialogue state, with a root panel,
    /// an Options panel and an abandon confirmation.
    /// RESPONSIBILITIES: freeze the game on Pause/Cancel, show the right panel, and restore everything (time, player
    /// controls, dialogue input) in the right order when resuming or abandoning to the main menu.
    /// HOW IT WORKS: pausing calls GameSession.EnterPause() (remembers Gameplay or Dialogue), sets Time.timeScale to 0
    /// and, if we came from Gameplay, disables the player's flight/fire/dash scripts the same way DialogueController
    /// does; if we came from Dialogue, it instead calls DialogueController.SetInputSuspended(true) so the dialogue's
    /// own Submit reading does not leak through pause navigation. Time.timeScale alone would not be enough: Update()
    /// still runs every real frame regardless of timeScale, so a script reading a fresh button press could still act.
    /// WHY: a single small state machine (which panel is showing) instead of a generic UI/state framework, per spec.
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        /// <summary>Which panel of the pause menu is currently shown.</summary>
        private enum Panel
        {
            /// <summary>Not paused: no panel is shown.</summary>
            None,

            /// <summary>Reprendre / Options / Retour au menu.</summary>
            Root,

            /// <summary>Language, volumes, hitbox, fullscreen.</summary>
            Options,

            /// <summary>Abandon confirmation.</summary>
            Confirm,
        }

        [Header("Input")]
        [SerializeField, Tooltip("Pause action (Button) of the Player action map (Escape, gamepad Start). Opens the pause menu, and resumes when pressed again from the root panel.")]
        private InputActionReference _pauseAction;

        [SerializeField, Tooltip("UI Cancel action. From Options or the confirmation, closes only that panel back to the root.")]
        private InputActionReference _cancelAction;

        [Header("Player (disabled while paused from Gameplay; untouched while paused from Dialogue)")]
        [SerializeField, Tooltip("Optional. Suspended (not disabled) instead of the player scripts when pausing during a dialogue.")]
        private DialogueController _dialogueController;

        [SerializeField, Tooltip("Player's movement script, switched off while paused from Gameplay.")]
        private PlayerMovementController _movement;

        [SerializeField, Tooltip("Player's fire script, switched off while paused from Gameplay and kept off until the shared button is released, so resuming cannot fire a shot.")]
        private PlayerShootController _shoot;

        [SerializeField, Tooltip("Player's dash script, switched off while paused from Gameplay.")]
        private PlayerDashController _dash;

        [SerializeField, Tooltip("Optional. Player's shield script (spec E5-05), switched off while paused from Gameplay so its timers freeze exactly like the dash's.")]
        private PlayerShieldController _shield;

        [SerializeField, Tooltip("Optional. Player's Pulse script (spec E5-06), switched off while paused from Gameplay so its recharge freezes exactly like the shield's.")]
        private PlayerPulseController _pulse;

        [Header("Panels")]
        [SerializeField, Tooltip("Reprendre / Options / Retour au menu.")]
        private GameObject _rootPanel;

        [SerializeField, Tooltip("Language, volumes, hitbox, fullscreen.")]
        private GameObject _optionsPanel;

        [SerializeField, Tooltip("Abandon confirmation (Annuler / Retour au menu).")]
        private GameObject _confirmPanel;

        [SerializeField, Tooltip("Button selected by the EventSystem whenever the root panel opens (keyboard/gamepad focus).")]
        private GameObject _firstSelectedOnRoot;

        [SerializeField, Tooltip("Button selected by the EventSystem whenever the Options panel opens.")]
        private GameObject _firstSelectedInOptions;

        [SerializeField, Tooltip("Button selected by the EventSystem whenever the abandon confirmation opens. Spec: Annuler selected by default.")]
        private GameObject _firstSelectedInConfirm;

        [Header("Settings")]
        [SerializeField, Tooltip("Scene loaded by the abandon confirmation.")]
        private string _mainMenuSceneName = "MainMenu";

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: which panel is currently shown.")]
        private Panel _current = Panel.None;

        private float _previousTimeScale = 1f; // Time.timeScale at the moment of pausing, restored on resume.

        /// <summary>True while any pause panel is shown.</summary>
        public bool IsPaused => _current != Panel.None;

        /// <summary>
        /// Hides every pause panel at scene start, regardless of what was left active in the editor: a panel's active
        /// state in the saved scene is not a reliable source of truth (an author can leave it visible while working
        /// on its layout), so the state machine's own None state is enforced here instead of assumed.
        /// </summary>
        private void Awake()
        {
            ShowPanel(Panel.None);
        }

        /// <summary>Makes sure the actions used to open, navigate and close the menu are enabled.</summary>
        private void OnEnable()
        {
            if (_pauseAction != null)
            {
                _pauseAction.action.Enable();
            }

            if (_cancelAction != null)
            {
                _cancelAction.action.Enable();
            }
        }

        /// <summary>Reads Pause and Cancel each frame; what they do depends on which panel (if any) is showing.</summary>
        private void Update()
        {
            if (GameSession.Current == null)
            {
                return;
            }

            bool pausePressed = _pauseAction != null && _pauseAction.action.WasPressedThisFrame();
            bool cancelPressed = _cancelAction != null && _cancelAction.action.WasPressedThisFrame();

            switch (_current)
            {
                case Panel.None:
                    if (pausePressed && CanPause())
                    {
                        OpenPause();
                    }

                    break;

                case Panel.Root:
                    if (pausePressed || cancelPressed)
                    {
                        Resume();
                    }

                    break;

                default: // Options or Confirm: only Cancel closes the panel, back to the root (spec: never straight to Gameplay).
                    if (cancelPressed)
                    {
                        BackToRoot();
                    }

                    break;
            }
        }

        /// <summary>Pause is offered only during normal play or a dialogue, never from Game Over or the mission end.</summary>
        private bool CanPause()
        {
            GameState state = GameSession.Current.State;
            return state == GameState.Gameplay || state == GameState.Dialogue;
        }

        /// <summary>Freezes the game and shows the root pause panel. Safe to call when already paused or not allowed: does nothing.</summary>
        public void OpenPause()
        {
            if (_current != Panel.None || !CanPause())
            {
                return;
            }

            GameState previous = GameSession.Current.State;
            GameSession.Current.EnterPause();
            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            if (previous == GameState.Gameplay)
            {
                SetPlayerControlsEnabled(false);
            }
            else if (_dialogueController != null)
            {
                _dialogueController.SetInputSuspended(true);
            }

            GameEvents.RaiseGameStateChanged(GameState.Paused);
            ShowPanel(Panel.Root);
        }

        /// <summary>Shows the Options panel (called by the root panel's Options button).</summary>
        public void OpenOptions()
        {
            if (_current == Panel.Root)
            {
                ShowPanel(Panel.Options);
            }
        }

        /// <summary>Shows the abandon confirmation (called by the root panel's Retour au menu button).</summary>
        public void OpenConfirm()
        {
            if (_current == Panel.Root)
            {
                ShowPanel(Panel.Confirm);
            }
        }

        /// <summary>Closes Options or the confirmation, back to the root panel. Never resumes Gameplay by itself.</summary>
        public void BackToRoot()
        {
            if (_current == Panel.Options || _current == Panel.Confirm)
            {
                ShowPanel(Panel.Root);
            }
        }

        /// <summary>Restores time and the player's controls, resuming Gameplay or Dialogue, whichever was suspended.</summary>
        public void Resume()
        {
            if (_current == Panel.None)
            {
                return;
            }

            GameSession.Current.ExitPause();
            GameState resumed = GameSession.Current.State;
            Time.timeScale = _previousTimeScale;

            if (resumed == GameState.Gameplay)
            {
                SetPlayerControlsEnabled(true);
                if (_shoot != null)
                {
                    _shoot.SuppressUntilReleased();
                }
            }
            else if (_dialogueController != null)
            {
                _dialogueController.SetInputSuspended(false);
            }

            GameEvents.RaiseGameStateChanged(resumed);
            ShowPanel(Panel.None);
        }

        /// <summary>
        /// Called by the confirmation's Retour au menu button: restores time and controls (so nothing stays frozen or
        /// disabled across the scene load) and loads the main menu. No dialogue callback is invoked, the same way
        /// DialogueController.OnDisable never resumes combat when it is torn down instead of finishing normally.
        /// </summary>
        public void ConfirmReturnToMenu()
        {
            Time.timeScale = 1f;
            SetPlayerControlsEnabled(true);
            SceneManager.LoadScene(_mainMenuSceneName);
        }

        /// <summary>Shows exactly one panel (or none) and restores keyboard/gamepad focus when the root panel opens.</summary>
        private void ShowPanel(Panel panel)
        {
            _current = panel;
            if (_rootPanel != null)
            {
                _rootPanel.SetActive(panel == Panel.Root);
            }

            if (_optionsPanel != null)
            {
                _optionsPanel.SetActive(panel == Panel.Options);
            }

            if (_confirmPanel != null)
            {
                _confirmPanel.SetActive(panel == Panel.Confirm);
            }

            GameObject firstSelected = panel switch
            {
                Panel.Root => _firstSelectedOnRoot,
                Panel.Options => _firstSelectedInOptions,
                Panel.Confirm => _firstSelectedInConfirm,
                _ => null,
            };

            Select(firstSelected);
        }

        /// <summary>
        /// Selects a button, clearing the current selection first: EventSystem.SetSelectedGameObject is a no-op when
        /// the target is already the current selection, which would silently skip the highlight's color transition.
        /// </summary>
        private static void Select(GameObject target)
        {
            if (target == null || EventSystem.current == null)
            {
                return;
            }

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(target);
        }

        /// <summary>Switches the player's flight and combat scripts on or off together (mirrors DialogueController).</summary>
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

            if (_pulse != null)
            {
                _pulse.enabled = enabledState;
            }
        }
    }
}
