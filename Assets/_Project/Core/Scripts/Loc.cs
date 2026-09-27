using System.Collections.Generic;
using UnityEngine;

namespace Ashar.Core
{
    /// <summary>
    /// Loads the interface string table and returns each key's text in the current language (cahier des charges §7.13).
    /// RESPONSIBILITIES: parse Localization/strings.csv (key;fr;en) once, and return the right column for a key.
    /// HOW IT WORKS: the table is loaded lazily on first use from Resources, so any script can call Get() without
    /// worrying about load order. A key missing from the table shows as "#key#" and logs a warning, instead of
    /// throwing or silently showing nothing, so a typo is visible on screen during development.
    /// WHY: this is interface text (button labels, options), not narrative content: cahier §7.13 asks for a plain
    /// FR/EN table here, distinct from DialogueData's own FR/EN fields used for spoken lines (spec E3-02).
    /// </summary>
    public static class Loc
    {
        private const string ResourcePath = "Localization/strings";

        private static Dictionary<string, string> _french;
        private static Dictionary<string, string> _english;
        private static bool _loaded;

        /// <summary>Returns the text for a key in the current language (PlayerPreferences.Language). "#key#" if missing.</summary>
        public static string Get(string key)
        {
            EnsureLoaded();
            Dictionary<string, string> table = PlayerPreferences.Language == Language.English ? _english : _french;
            if (table.TryGetValue(key, out string value))
            {
                return value;
            }

            Debug.LogWarning($"Loc: missing key '{key}' for {PlayerPreferences.Language}.");
            return $"#{key}#";
        }

        /// <summary>Loads the table from Resources if it has not been loaded yet this session.</summary>
        private static void EnsureLoaded()
        {
            if (_loaded)
            {
                return;
            }

            TextAsset csv = Resources.Load<TextAsset>(ResourcePath);
            (_french, _english) = ParseCsv(csv != null ? csv.text : "");
            if (csv == null)
            {
                Debug.LogWarning($"Loc: '{ResourcePath}' not found in Resources; every key will show as \"#key#\".");
            }

            _loaded = true;
        }

        /// <summary>
        /// Parses a "key;fr;en" table, one entry per line, skipping a header line and blank lines. Malformed lines
        /// (not exactly 3 fields) are skipped rather than throwing. Static and free of Unity state so it can be
        /// unit-tested.
        /// </summary>
        public static (Dictionary<string, string> French, Dictionary<string, string> English) ParseCsv(string text)
        {
            var french = new Dictionary<string, string>();
            var english = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(text))
            {
                return (french, english);
            }

            string[] lines = text.Replace("\r\n", "\n").Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.Length == 0 || line.StartsWith("key;"))
                {
                    continue; // Blank line or the header row.
                }

                string[] fields = line.Split(';');
                if (fields.Length != 3)
                {
                    continue;
                }

                french[fields[0]] = fields[1];
                english[fields[0]] = fields[2];
            }

            return (french, english);
        }

        /// <summary>Test-only hook: forces the next Get() to reload the table (the real game never needs this).</summary>
        internal static void ResetForTests()
        {
            _loaded = false;
        }
    }
}
