using UnityEngine;
using UnityEngine.InputSystem;

namespace Ashar.UI
{
    /// <summary>
    /// PROVISIONAL driver that feeds UIRadioDisplayView with Dyllan's actual lines from
    /// docs/Demo_Revolte_Ashar.md, so E3-01 can be play-tested before any dialogue system exists.
    /// RESPONSIBILITIES: show the next test line on UI/Submit, and hide the panel once the list of
    /// lines is exhausted.
    /// HOW IT WORKS: each press of UI/Submit shows the next line; pressing it again once the list is
    /// exhausted hides the panel and starts over. This script is a throw-away test tool: it lives in
    /// the TestBed scene only, and will be deleted once a real trigger (WaveData's Dialogue event,
    /// once Ink lands in E3-02) replaces it.
    /// </summary>
    public class DebugRadioDisplayDriver : MonoBehaviour
    {
        [System.Serializable]
        private struct TestLine
        {
            [Tooltip("Speaker name shown above the line.")]
            public string SpeakerName;

            [TextArea(2, 4), Tooltip("Line of dialogue, copied as written by Dyllan in Demo_Revolte_Ashar.md.")]
            public string Text;
        }

        [Header("References")]
        [SerializeField, Tooltip("View that shows the current test line.")]
        private UIRadioDisplayView _radioDisplay;

        [SerializeField, Tooltip("Submit action (Button) of the UI action map. Pressed, it shows the next test line.")]
        private InputActionReference _advanceAction;

        [Header("Test data")]
        [SerializeField, Tooltip("Lines shown in order, one per Submit press. Taken from docs/Demo_Revolte_Ashar.md so the display is tested with real dialogue, not invented text.")]
        private TestLine[] _testLines =
        {
            new TestLine { SpeakerName = "KAAL'VARIS", Text = "Générateur pléiadien connecté. Encore 45 secondes… et on va voir ce que ce tas de boue a dans le ventre." },
            new TestLine { SpeakerName = "AMARA", Text = "Kaal'varis, l'Assemblée vous a repérés. Des Horizon Scouts et des Guardian Dropships convergent vers vous." },
            new TestLine { SpeakerName = "KAAL'VARIS", Text = "Le Culte… Ils viennent finir le travail. Restez groupés. On s'est évadés ensemble, on sortira d'ici ensemble." },
        };

        private int _nextLineIndex; // Index of the next test line to show; wraps back to 0 after Hide().

        /// <summary>Enables the Submit action.</summary>
        private void OnEnable()
        {
            if (_advanceAction != null)
            {
                _advanceAction.action.Enable();
            }
        }

        /// <summary>Disables the Submit action.</summary>
        private void OnDisable()
        {
            if (_advanceAction != null)
            {
                _advanceAction.action.Disable();
            }
        }

        /// <summary>Shows the next test line on Submit, or hides the panel and loops once the list is exhausted.</summary>
        private void Update()
        {
            if (_radioDisplay == null || _advanceAction == null || !_advanceAction.action.WasPressedThisFrame())
            {
                return;
            }

            if (_nextLineIndex >= _testLines.Length)
            {
                _radioDisplay.Hide();
                _nextLineIndex = 0;
                return;
            }

            TestLine line = _testLines[_nextLineIndex];
            _radioDisplay.Show(line.SpeakerName, line.Text);
            _nextLineIndex++;
        }
    }
}
