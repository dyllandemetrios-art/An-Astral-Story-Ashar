using Ashar.Core;
using Ashar.Waves;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Ashar.Menu
{
    /// <summary>
    /// The main menu (spec E3-06): Jouer, Options, Quitter.
    /// RESPONSIBILITIES: load the player's saved preferences on startup, start a fresh run, show Options, and quit
    /// the build (never the Editor).
    /// HOW IT WORKS: Jouer loads the QA destination scene (TestBed, until a real mission scene chain exists) and
    /// starts its MissionRunnerController itself once the scene is loaded, so the player never needs the manual
    /// "Start mission" context-menu entry meant for testing. A loading flag stops a second press from loading the
    /// scene twice.
    /// WHY: no mission-select or save/continue here (out of scope, E3-04): every run starts fresh, matching the
    /// spec's "aucune sélection de mission" constraint for this story.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField, Tooltip("Jouer / Options / Quitter.")]
        private GameObject _rootPanel;

        [SerializeField, Tooltip("Language, volumes, hitbox, fullscreen.")]
        private GameObject _optionsPanel;

        [SerializeField, Tooltip("Button selected by the EventSystem whenever the root panel opens (keyboard/gamepad focus).")]
        private GameObject _firstSelectedOnRoot;

        [SerializeField, Tooltip("Button selected by the EventSystem whenever the Options panel opens.")]
        private GameObject _firstSelectedInOptions;

        [Header("Settings")]
        [SerializeField, Tooltip("Scene loaded by Jouer. QA destination (TestBed) until a real mission scene chain exists (spec E3-06); never claim the final tutorial is wired up.")]
        private string _playSceneName = "TestBed";

        private bool _isLoading; // True from the first Jouer press until the scene load completes, so a second press does nothing.

        /// <summary>Loads the player's saved preferences so the menu (and the game) reflect them immediately.</summary>
        private void Awake()
        {
            PlayerPreferences.Load();
        }

        /// <summary>Selects the first root button so keyboard/gamepad navigation has a starting point without a mouse click.</summary>
        private void Start()
        {
            Select(_firstSelectedOnRoot);
        }

        /// <summary>Starts a fresh run: loads the QA destination scene and starts its mission once loaded. Ignored while already loading.</summary>
        public void Play()
        {
            if (_isLoading)
            {
                return;
            }

            _isLoading = true;
            SceneManager.sceneLoaded += HandleSceneLoaded;
            SceneManager.LoadScene(_playSceneName);
        }

        /// <summary>Shows the Options panel.</summary>
        public void OpenOptions()
        {
            if (_rootPanel != null)
            {
                _rootPanel.SetActive(false);
            }

            if (_optionsPanel != null)
            {
                _optionsPanel.SetActive(true);
            }

            Select(_firstSelectedInOptions);
        }

        /// <summary>Closes Options, back to the root panel.</summary>
        public void CloseOptions()
        {
            if (_optionsPanel != null)
            {
                _optionsPanel.SetActive(false);
            }

            if (_rootPanel != null)
            {
                _rootPanel.SetActive(true);
            }

            Select(_firstSelectedOnRoot);
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

        /// <summary>Quits the build. In the Editor this only logs, so Play Mode is never closed by testing Quitter.</summary>
        public void Quit()
        {
#if UNITY_EDITOR
            Debug.Log($"{nameof(MainMenuController)}: Quitter pressed in the Editor; not closing it. Verify the real close in a Windows build.");
#else
            Application.Quit();
#endif
        }

        /// <summary>Starts the mission of the freshly loaded scene, once, then stops listening.</summary>
        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            var runner = FindAnyObjectByType<MissionRunnerController>();
            if (runner != null)
            {
                runner.StartMission();
            }
        }
    }
}
