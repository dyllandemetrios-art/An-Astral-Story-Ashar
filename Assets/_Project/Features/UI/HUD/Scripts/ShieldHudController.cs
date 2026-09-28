using Ashar.Core;
using Ashar.Player;
using TMPro;
using UnityEngine;

namespace Ashar.UI
{
    /// <summary>
    /// Minimal HUD text for the shield's state (spec E5-05): Ready, Active or Recharging, with the time left.
    /// RESPONSIBILITIES: read PlayerShieldController every frame and show a single localized status line.
    /// HOW IT WORKS: ComputeStatusText is pure and unit-tested; this component only wires it to Loc (E3-06's string
    /// table) and the label. Reading Loc every frame, rather than once on enable like LocalizedText, means a language
    /// change in Options applies to this line immediately without any extra refresh call.
    /// WHY: the spec explicitly asks for a minimal display here, not the full HUD (cooldown bubbles, boss bar...)
    /// built in a later story; no generic capability-UI framework.
    /// </summary>
    public class ShieldHudController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Player's shield script, read every frame.")]
        private PlayerShieldController _shield;

        [SerializeField, Tooltip("Label that shows the shield's status.")]
        private TMP_Text _label;

        /// <summary>Refreshes the status line from the shield's current state and the active language.</summary>
        private void Update()
        {
            if (_shield == null || _label == null)
            {
                return;
            }

            _label.text = ComputeStatusText(
                _shield.IsReady,
                _shield.IsActive,
                _shield.TimeLeft,
                Loc.Get("shield.ready"),
                Loc.Get("shield.active"),
                Loc.Get("shield.recharging"));
        }

        /// <summary>
        /// Builds the one-line status text: Active and Recharging append the seconds left, rounded up so the display
        /// never briefly shows "0s" the instant before the state actually changes.
        /// Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static string ComputeStatusText(bool isReady, bool isActive, float timeLeft, string readyLabel, string activeLabel, string rechargingLabel)
        {
            if (isActive)
            {
                return $"{activeLabel} {Mathf.CeilToInt(timeLeft)}s";
            }

            return isReady ? readyLabel : $"{rechargingLabel} {Mathf.CeilToInt(timeLeft)}s";
        }
    }
}
