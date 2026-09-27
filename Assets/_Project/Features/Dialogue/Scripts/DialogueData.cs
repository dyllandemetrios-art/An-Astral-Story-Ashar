using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ashar.Dialogue
{
    /// <summary>The two languages a line can be shown in (spec §7.13). English is added by this story so the data is ready for it.</summary>
    public enum DialogueLanguage
    {
        /// <summary>French, the default language.</summary>
        French,

        /// <summary>English.</summary>
        English,
    }

    /// <summary>
    /// One line of a dialogue sequence: who says it, and its text in both languages.
    /// </summary>
    [Serializable]
    public class DialogueLine
    {
        [SerializeField, Tooltip("Name shown above the line (e.g. KAAL'VARIS, AMARA).")]
        private string _speakerName;

        [SerializeField, TextArea(2, 4), Tooltip("French text of this line, about two lines long.")]
        private string _textFr;

        [SerializeField, TextArea(2, 4), Tooltip("English text of this line, about two lines long.")]
        private string _textEn;

        [SerializeField, Tooltip("Optional voice clip for this line. Left empty: the line stays fully readable with no audio (spec E3-02).")]
        private AudioClip _voiceClip;

        /// <summary>Name shown above the line.</summary>
        public string SpeakerName => _speakerName;

        /// <summary>Optional voice clip; null when the line has none.</summary>
        public AudioClip VoiceClip => _voiceClip;

        /// <summary>Returns the text of this line in the given language.</summary>
        public string GetText(DialogueLanguage language)
        {
            return language == DialogueLanguage.English ? _textEn : _textFr;
        }
    }

    /// <summary>
    /// A linear sequence of dialogue lines played between combats (spec E3-02).
    /// RESPONSIBILITIES: hold the ordered lines of one exchange, with no branching.
    /// HOW IT WORKS: this is a ScriptableObject asset (DialogueData_...), read by DialogueController one line at a time.
    /// WHY: replaces the earlier Ink proposal (cahier des charges §7.7) with plain data, since the demo has no branching
    /// dialogue and needs no narrative scripting language.
    /// PATTERN: data-driven design, the same as WaveData for the wave tables.
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueData_New", menuName = "Ashar/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        [SerializeField, Tooltip("Identifier used in logs and diagnostics; never shown to the player.")]
        private string _id;

        [SerializeField, Tooltip("The lines, played in order, one per UI/Submit press.")]
        private List<DialogueLine> _lines = new List<DialogueLine>();

        /// <summary>Identifier used in logs and diagnostics.</summary>
        public string Id => _id;

        /// <summary>The lines of the sequence, in order.</summary>
        public IReadOnlyList<DialogueLine> Lines => _lines;
    }
}
