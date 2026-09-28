using Ashar.Core;
using TMPro;
using UnityEngine;

namespace Ashar.Menu
{
    /// <summary>
    /// Shows one Loc key's text on a TMP label (spec E3-06: no interface text hard-coded in a scene).
    /// RESPONSIBILITIES: set the label's text from Loc whenever this object becomes active.
    /// HOW IT WORKS: OnEnable covers every case that matters here: the label's panel opening, and a language change
    /// applied by refreshing the panel (Options closes and reopens the button rows it does not own directly).
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField, Tooltip("Key looked up in Localization/strings.csv.")]
        private string _key;

        private TMP_Text _label;

        /// <summary>Applies the current language's text.</summary>
        private void OnEnable()
        {
            Refresh();
        }

        /// <summary>
        /// Re-applies the current language's text. Called by OnEnable, and by a panel that changes the language while
        /// its labels stay active (OnEnable only fires once when the panel itself opens).
        /// </summary>
        public void Refresh()
        {
            if (_label == null)
            {
                _label = GetComponent<TMP_Text>();
            }

            if (string.IsNullOrEmpty(_key))
            {
                Debug.LogWarning($"{nameof(LocalizedText)} on '{name}' has no key set.", this);
                return;
            }

            _label.text = Loc.Get(_key);
        }
    }
}
