using Ashar.Core;
using Ashar.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ashar.Menu
{
    /// <summary>
    /// The Options panel (spec E3-06): language, three volumes, hitbox visibility and fullscreen.
    /// RESPONSIBILITIES: show the current PlayerPreferences values when it opens, apply each change immediately, and
    /// save the preferences once when it closes.
    /// HOW IT WORKS: every control writes straight to the static PlayerPreferences; this panel does not decide when a
    /// value takes effect (a volume slider already applies live) or persist anything itself while it stays open, per
    /// spec ("écrire à la sortie du panneau, sans écriture chaque frame"). One instance of this panel exists per
    /// scene that needs it (MainMenu, TestBed's pause); they all read and write the same static preferences, so a
    /// change in either one is visible in the other the next time it opens.
    /// WHY: a plain MonoBehaviour is enough for a handful of controls; no generic settings framework.
    /// </summary>
    public class OptionsPanelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Optional. Told about a language change immediately, so a dialogue not yet shown starts in the new language (spec E3-02: applied at the next exchange).")]
        private DialogueController _dialogueController;

        [Header("Language")]
        [SerializeField, Tooltip("Shows the current language ('FR' or 'EN'). The button that calls ToggleLanguage flips it.")]
        private TMP_Text _languageValueLabel;

        [Header("Volumes")]
        [SerializeField, Tooltip("Music volume, 0 to 1.")]
        private Slider _musicSlider;

        [SerializeField, Tooltip("Sound effects volume, 0 to 1.")]
        private Slider _effectsSlider;

        [SerializeField, Tooltip("Dialogue volume (radio blips today, voice later), 0 to 1.")]
        private Slider _dialogueSlider;

        [Header("Display")]
        [SerializeField, Tooltip("On: the player hitbox dot is shown.")]
        private Toggle _hitboxToggle;

        [SerializeField, Tooltip("On: the build window runs fullscreen.")]
        private Toggle _fullscreenToggle;

        /// <summary>Shows the current preferences whenever the panel is opened.</summary>
        private void OnEnable()
        {
            if (_languageValueLabel != null)
            {
                UpdateLanguageLabel();
            }

            _musicSlider?.SetValueWithoutNotify(PlayerPreferences.MusicVolume);
            _effectsSlider?.SetValueWithoutNotify(PlayerPreferences.EffectsVolume);
            _dialogueSlider?.SetValueWithoutNotify(PlayerPreferences.DialogueVolume);
            _hitboxToggle?.SetIsOnWithoutNotify(PlayerPreferences.ShowHitboxOverride ?? true);
            _fullscreenToggle?.SetIsOnWithoutNotify(Screen.fullScreen);
        }

        /// <summary>Saves the preferences once, when the panel closes (not every frame).</summary>
        private void OnDisable()
        {
            PlayerPreferences.Save();
        }

        /// <summary>Flips French/English, tells the dialogue controller, and refreshes every visible label.</summary>
        public void ToggleLanguage()
        {
            PlayerPreferences.Language = PlayerPreferences.Language == Language.French ? Language.English : Language.French;
            if (_dialogueController != null)
            {
                _dialogueController.SetLanguage(PlayerPreferences.Language == Language.English ? DialogueLanguage.English : DialogueLanguage.French);
            }

            UpdateLanguageLabel();

            // Sibling panels (the root menu behind this one) also show localized text; a live toggle needs every
            // instance refreshed, not just this panel's own children, instead of waiting for panels to reopen.
            var labels = FindObjectsByType<LocalizedText>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (LocalizedText label in labels)
            {
                label.Refresh();
            }
        }

        /// <summary>Sets the music volume, clamped to [0, 1].</summary>
        public void SetMusicVolume(float volume)
        {
            PlayerPreferences.MusicVolume = PlayerPreferences.ClampVolume(volume);
        }

        /// <summary>Sets the effects volume, clamped to [0, 1].</summary>
        public void SetEffectsVolume(float volume)
        {
            PlayerPreferences.EffectsVolume = PlayerPreferences.ClampVolume(volume);
        }

        /// <summary>Sets the dialogue volume, clamped to [0, 1].</summary>
        public void SetDialogueVolume(float volume)
        {
            PlayerPreferences.DialogueVolume = PlayerPreferences.ClampVolume(volume);
        }

        /// <summary>Sets the hitbox dot visibility override.</summary>
        public void SetHitboxVisible(bool visible)
        {
            PlayerPreferences.ShowHitboxOverride = visible;
        }

        /// <summary>Applies fullscreen immediately, so the toggle's own state always matches what the player sees.</summary>
        public void SetFullscreen(bool fullscreen)
        {
            PlayerPreferences.Fullscreen = fullscreen;
            Screen.fullScreen = fullscreen;
        }

        /// <summary>Shows the language row's label together with "EN" or "FR" for the current language, e.g. "LANGUE : FR".</summary>
        private void UpdateLanguageLabel()
        {
            string code = PlayerPreferences.Language == Language.English ? "EN" : "FR";
            _languageValueLabel.text = $"{Loc.Get("options.language")} : {code}";
        }
    }
}
