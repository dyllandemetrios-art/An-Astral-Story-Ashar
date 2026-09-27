using TMPro;
using UnityEngine;

namespace Ashar.UI
{
    /// <summary>
    /// Pure display for the short "radio" line shown during gameplay: a speaker name and about
    /// two lines of text (spec E3-01, cahier des charges §7.7).
    /// RESPONSIBILITIES: hold the name and text labels and the panel that shows them, and expose
    /// Show/Hide so a caller (a tutorial step today, a future dialogue system later) can drive it.
    /// HOW IT WORKS: Show() always overwrites both labels from scratch, so a shorter new line never
    /// keeps a leftover character from a longer previous one. This view never advances itself, never
    /// subscribes to gameplay events (fire, waves...) and never touches Time.timeScale: the caller
    /// decides what line to show and when to Hide() it, so gameplay is never interrupted by it.
    /// </summary>
    public class UIRadioDisplayView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Root of the panel, toggled active/inactive to show or hide the display.")]
        private GameObject _panelRoot;

        [SerializeField, Tooltip("Label that shows the speaker's name (e.g. KAAL'VARIS, AMARA).")]
        private TMP_Text _speakerNameLabel;

        [SerializeField, Tooltip("Label that shows the line of dialogue, about two lines long.")]
        private TMP_Text _lineTextLabel;

        /// <summary>True while the panel is currently shown.</summary>
        public bool IsVisible => _panelRoot != null && _panelRoot.activeSelf;

        /// <summary>Shows the panel with the given speaker name and line, replacing any line already shown.</summary>
        public void Show(string speakerName, string line)
        {
            _speakerNameLabel.text = speakerName;
            _lineTextLabel.text = line;
            _panelRoot.SetActive(true);
        }

        /// <summary>Hides the panel. Safe to call when it is already hidden.</summary>
        public void Hide()
        {
            _panelRoot.SetActive(false);
        }
    }
}
