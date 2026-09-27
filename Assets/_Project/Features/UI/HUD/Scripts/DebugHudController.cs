using Ashar.Core;
using UnityEngine;

namespace Ashar.UI
{
    /// <summary>
    /// PROVISIONAL text display of lives and game state, so the arcade loop can be tested before the real HUD exists.
    /// RESPONSIBILITIES: show the lives left in the corner of the screen, and a message when the game is over or the
    /// mission is complete.
    /// HOW IT WORKS: it reads GameSession.Current every frame and draws text with the old immediate-mode GUI (OnGUI).
    /// Score is never shown (spec E3-09: the demo is about surviving, not scoring); GameSession still tracks it
    /// internally so the existing events, fields and tests stay untouched.
    /// WHY: the real HUD (lives, cooldown bubbles, boss bar) is built later with uGUI and the interface kit. This
    /// script is a throw-away test tool: it lives in the TestBed scene only, and will be deleted when the HUD replaces it.
    /// </summary>
    public class DebugHudController : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField, Min(8), Tooltip("Font size of the text, in pixels.")]
        private int _fontSize = 24;

        /// <summary>Draws the lives, and the game over or mission complete message. No score: the demo is about surviving (spec E3-09).</summary>
        private void OnGUI()
        {
            GameSession session = GameSession.Current;
            if (session == null)
            {
                return;
            }

            var style = new GUIStyle(GUI.skin.label) { fontSize = _fontSize, fontStyle = FontStyle.Bold };
            style.normal.textColor = Color.white;

            style.alignment = TextAnchor.UpperRight;
            GUI.Label(new Rect(0f, 8f, Screen.width - 16f, _fontSize * 2.5f), $"LIVES {session.Lives}", style);

            string message = session.State == GameState.GameOver
                ? (session.CanContinue ? "GAME OVER\npress the Fire button to continue" : "GAME OVER")
                : session.State == GameState.MissionEnd ? "MISSION COMPLETE" : null;
            if (message != null)
            {
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = _fontSize * 2;
                GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), message, style);
            }
        }
    }
}
