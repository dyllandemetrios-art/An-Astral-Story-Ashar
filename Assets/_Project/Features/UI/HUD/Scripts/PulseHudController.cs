using Ashar.Core;
using Ashar.Player;
using TMPro;
using UnityEngine;

namespace Ashar.UI
{
    /// <summary>
    /// Minimal HUD text for the Pulse's state (spec E5-06): the charge/recharge counting down from 45, then Ready.
    /// RESPONSIBILITIES: read PlayerPulseController every frame and show a single localized status line.
    /// HOW IT WORKS: ComputeStatusText is pure and unit-tested; this component only wires it to Loc (E3-06's string
    /// table) and the label, the same shape as ShieldHudController (E5-05).
    /// WHY: the spec explicitly asks for "45 to 0, then Ready or the recharge left", not the full HUD (cooldown
    /// bubbles, icon...) a later story may add; no generic capability-UI framework.
    /// </summary>
    public class PulseHudController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Player's Pulse script, read every frame.")]
        private PlayerPulseController _pulse;

        [SerializeField, Tooltip("Label that shows the Pulse's status.")]
        private TMP_Text _label;

        /// <summary>Refreshes the status line from the Pulse's current state and the active language.</summary>
        private void Update()
        {
            if (_pulse == null || _label == null)
            {
                return;
            }

            _label.text = ComputeStatusText(_pulse.IsReady, _pulse.TimeLeft, Loc.Get("pulse.ready"), Loc.Get("pulse.charging"));
        }

        /// <summary>
        /// Builds the one-line status text: Ready shows the plain label, otherwise the charging/recharging label with
        /// the seconds left, rounded up so the display never briefly shows "0s" the instant before Ready.
        /// Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static string ComputeStatusText(bool isReady, float timeLeft, string readyLabel, string chargingLabel)
        {
            return isReady ? readyLabel : $"{chargingLabel} {Mathf.CeilToInt(timeLeft)}s";
        }
    }
}
