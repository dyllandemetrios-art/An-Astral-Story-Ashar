using System;
using UnityEngine;

namespace Ashar.Core
{
    /// <summary>The language the interface and the dialogues are shown in (spec E3-06, cahier des charges §7.13).</summary>
    public enum Language
    {
        /// <summary>French, the default language.</summary>
        French,

        /// <summary>English.</summary>
        English,
    }

    /// <summary>
    /// The player's own settings (spec E3-06): language, volumes, hitbox visibility and fullscreen.
    /// RESPONSIBILITIES: hold the current preferences, validate what is loaded, and persist them with PlayerPrefs.
    /// HOW IT WORKS: Load() reads PlayerPrefs once; a missing or out-of-range value falls back to the defaults below
    /// instead of being applied as-is. Save() writes the current values, meant to be called when the Options panel
    /// closes, not every frame. This never touches a design ScriptableObject on disk (PlayerShipData, GameBalanceData...):
    /// PlayerHealthController combines ShowHitboxOverride with PlayerShipData.ShowHitbox itself, so Dyllan's own
    /// design value is preserved and only overridden once the player actually picks one in Options.
    /// WHY: a handful of player-facing toggles do not need a save file or a ScriptableObject; PlayerPrefs is enough.
    /// </summary>
    public static class PlayerPreferences
    {
        private const string LanguageKey = "Ashar.Language";
        private const string MusicVolumeKey = "Ashar.MusicVolume";
        private const string EffectsVolumeKey = "Ashar.EffectsVolume";
        private const string DialogueVolumeKey = "Ashar.DialogueVolume";
        private const string ShowHitboxKey = "Ashar.ShowHitbox";
        private const string FullscreenKey = "Ashar.Fullscreen";

        /// <summary>Interface and dialogue language. French by default.</summary>
        public static Language Language { get; set; } = Language.French;

        /// <summary>Music volume, 0 (silent) to 1 (full).</summary>
        public static float MusicVolume { get; set; } = 1f;

        /// <summary>Sound effects volume, 0 to 1.</summary>
        public static float EffectsVolume { get; set; } = 1f;

        /// <summary>Dialogue volume (radio blips today, voice later), 0 to 1.</summary>
        public static float DialogueVolume { get; set; } = 1f;

        /// <summary>
        /// Whether the player hitbox dot is shown. Null until the player picks a value in Options: PlayerHealthController
        /// then falls back to PlayerShipData's own design default.
        /// </summary>
        public static bool? ShowHitboxOverride { get; set; }

        /// <summary>Whether the build window runs fullscreen.</summary>
        public static bool Fullscreen { get; set; } = true;

        /// <summary>Reads every preference from PlayerPrefs, correcting missing or out-of-range values to the defaults above.</summary>
        public static void Load()
        {
            Language = ParseLanguage(PlayerPrefs.GetString(LanguageKey, Language.French.ToString()));
            MusicVolume = ClampVolume(PlayerPrefs.GetFloat(MusicVolumeKey, 1f));
            EffectsVolume = ClampVolume(PlayerPrefs.GetFloat(EffectsVolumeKey, 1f));
            DialogueVolume = ClampVolume(PlayerPrefs.GetFloat(DialogueVolumeKey, 1f));
            Fullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) != 0;
            ShowHitboxOverride = PlayerPrefs.HasKey(ShowHitboxKey) ? PlayerPrefs.GetInt(ShowHitboxKey) != 0 : (bool?)null;
        }

        /// <summary>Writes every preference to PlayerPrefs. Called when the Options panel closes, not every frame.</summary>
        public static void Save()
        {
            PlayerPrefs.SetString(LanguageKey, Language.ToString());
            PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
            PlayerPrefs.SetFloat(EffectsVolumeKey, EffectsVolume);
            PlayerPrefs.SetFloat(DialogueVolumeKey, DialogueVolume);
            PlayerPrefs.SetInt(FullscreenKey, Fullscreen ? 1 : 0);
            if (ShowHitboxOverride.HasValue)
            {
                PlayerPrefs.SetInt(ShowHitboxKey, ShowHitboxOverride.Value ? 1 : 0);
            }

            PlayerPrefs.Save();
        }

        /// <summary>Clamps a volume to [0, 1]. Static and free of Unity state so it can be unit-tested.</summary>
        public static float ClampVolume(float volume)
        {
            return volume < 0f ? 0f : volume > 1f ? 1f : volume;
        }

        /// <summary>
        /// Parses a stored language name, falling back to French for anything missing or unrecognised.
        /// Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static Language ParseLanguage(string storedName)
        {
            return Enum.TryParse(storedName, out Language language) ? language : Language.French;
        }
    }
}
