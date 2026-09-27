namespace Ashar.Core
{
    /// <summary>
    /// The pure calculations that apply the global balance multipliers (spec §7.9) to the numbers of enemies and bullets.
    /// RESPONSIBILITIES: turn a base value from an asset and a global multiplier into the value used in play.
    /// HOW IT WORKS: each method takes the base value and the multiplier and returns the scaled value, guarding against
    /// values that would break the game (a density of 0 would mean an infinite firing interval).
    /// WHY: with no Unity code the rules can be unit-tested, and there is one place that says what "fire density" means.
    /// </summary>
    public static class BalanceMath
    {
        /// <summary>Returns the hit points of an enemy after the global multiplier, never below 1.</summary>
        public static float ScaledHp(float baseHp, float multiplier)
        {
            float scaled = baseHp * multiplier;
            return scaled < 1f ? 1f : scaled;
        }

        /// <summary>
        /// Returns the firing interval after the fire density multiplier. A density above 1 means more bullets, so a shorter
        /// interval; below 1 fewer bullets, a longer one (spec §6: the density divides the interval). A density of 0 or less
        /// leaves the interval unchanged instead of dividing by zero.
        /// </summary>
        public static float ScaledFireInterval(float baseInterval, float density)
        {
            return density > 0f ? baseInterval / density : baseInterval;
        }

        /// <summary>Returns the speed of a bullet after the global bullet speed multiplier.</summary>
        public static float ScaledBulletSpeed(float baseSpeed, float multiplier)
        {
            return baseSpeed * multiplier;
        }

        /// <summary>
        /// True when another bullet may be created: fewer than the maximum are alive. A maximum of 0 or less means no limit.
        /// The limit protects the frame rate: the measure of story E2-01 found the game far from its limit at 2000 bullets.
        /// </summary>
        public static bool CanCreateProjectile(int aliveCount, int maxProjectiles)
        {
            return maxProjectiles <= 0 || aliveCount < maxProjectiles;
        }
    }
}
