using UnityEngine;

namespace Ashar.Waves
{
    /// <summary>
    /// The pure calculations of the wave system: where each enemy of a group appears, and the smooth change of scroll speed.
    /// RESPONSIBILITIES: turn the layout settings of a Spawn event into positions.
    /// HOW IT WORKS: ComputeSpawnPosition gives the position of one enemy of a group from its number, the entry side and
    /// the formation. The entry point is just outside the screen (or on it for the Screen entry), so enemies come into view.
    /// WHY: no Unity state, so the layouts can be unit-tested and a wave table can be trusted to look as written.
    /// </summary>
    public static class WaveMath
    {
        /// <summary>
        /// Returns the world position where the enemy number `index` (from 0) of a group of `count` appears.
        /// Top: above the top edge, shifted sideways by the offset. Left and Right: beside that edge, shifted up by the
        /// offset. Screen: on the screen, the given depth below the top edge, shifted sideways by the offset.
        /// A Line spreads the group along the entry edge; a V does the same and also puts the outer enemies further
        /// back, so the middle one leads; Single and Column keep every enemy at the entry point.
        /// </summary>
        public static Vector2 ComputeSpawnPosition(WaveEntry entry, WaveFormation formation, float entryOffset, float entryDepth,
            int index, int count, float spacing, Rect screen, float margin)
        {
            float lateral = 0f;  // Distance along the entry edge from the entry point.
            float back = 0f;     // Distance further away from the screen than the entry point.
            if (count > 1 && (formation == WaveFormation.Line || formation == WaveFormation.V))
            {
                float fromMiddle = index - (count - 1) * 0.5f;
                lateral = fromMiddle * spacing;
                if (formation == WaveFormation.V)
                {
                    back = Mathf.Abs(fromMiddle) * spacing * 0.6f;
                }
            }

            switch (entry)
            {
                case WaveEntry.Left:
                    return new Vector2(screen.xMin - margin - back, screen.center.y + entryOffset + lateral);

                case WaveEntry.Right:
                    return new Vector2(screen.xMax + margin + back, screen.center.y + entryOffset + lateral);

                case WaveEntry.Screen:
                    return new Vector2(screen.center.x + entryOffset + lateral, screen.yMax - entryDepth + back);

                default: // Top
                    return new Vector2(screen.center.x + entryOffset + lateral, screen.yMax + margin + back);
            }
        }

        /// <summary>
        /// Returns the scroll speed at a given time of a transition: from the start speed to the target speed, in a straight
        /// line over the duration, and exactly the target speed at the end (or at once if the duration is 0).
        /// </summary>
        public static float ComputeScrollSpeed(float fromSpeed, float toSpeed, float elapsed, float duration)
        {
            if (duration <= 0f || elapsed >= duration)
            {
                return toSpeed;
            }

            return Mathf.Lerp(fromSpeed, toSpeed, Mathf.Max(0f, elapsed) / duration);
        }
    }
}
