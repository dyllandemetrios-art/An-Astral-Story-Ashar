using UnityEngine;

namespace Ashar.Core
{
    /// <summary>
    /// Applies the frame-rate target required by the spec (§3) before the first scene loads.
    /// HOW IT WORKS: [RuntimeInitializeOnLoadMethod] makes Unity call Apply() automatically at
    /// startup, so every scene (Boot, TestBed...) gets the setting without a component in its hierarchy.
    /// WHY: VSync lives in the Quality Settings so it stays editable in the Editor. While VSync is on,
    /// Unity ignores targetFrameRate; this value is the safety net if VSync is ever turned off.
    /// </summary>
    public static class FrameRateSettings
    {
        private const int TargetFrameRate = 60; // Engine setting from the spec, not a balancing value.

        /// <summary>Sets the target frame rate once, before the first scene is loaded.</summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Apply()
        {
            Application.targetFrameRate = TargetFrameRate;
        }
    }
}
