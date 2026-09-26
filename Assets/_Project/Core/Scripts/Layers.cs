using UnityEngine;

namespace Ashar.Core
{
    /// <summary>
    /// The physics layers of the game, as constants, so no script needs a magic number or a string.
    /// RESPONSIBILITIES: give a name to each layer index defined in the project's Tags and Layers settings.
    /// HOW IT WORKS: the indexes below are the ones written in ProjectSettings/TagManager.asset. The collision
    /// matrix (who collides with whom, spec §5.4) is stored in the Physics 2D settings. An EditMode test checks
    /// that both still match this file, so a change made by hand in the Editor cannot go unnoticed.
    /// WHY: layers keep collision tests cheap (a bullet never even looks at another bullet) and make the rules
    /// readable in one table.
    /// </summary>
    public static class Layers
    {
        public const int PlayerHitbox = 8; // Tiny circle at the centre of the ship: the only part that gets hit.
        public const int PlayerGraze = 9;  // Larger circle around the ship, to detect near misses.
        public const int PlayerBullet = 10; // Bullets fired by the player.
        public const int Enemy = 11;        // Enemy bodies.
        public const int EnemyBullet = 12;  // Bullets fired by enemies.
        public const int PowerUp = 13;      // Collectable power-ups.

        /// <summary>The names of the game layers, in index order starting at <see cref="PlayerHitbox"/>.</summary>
        public static readonly string[] Names =
        {
            "PlayerHitbox", "PlayerGraze", "PlayerBullet", "Enemy", "EnemyBullet", "PowerUp",
        };

        /// <summary>Returns the layer name for a game layer index (8 to 13).</summary>
        public static string NameOf(int layer)
        {
            return Names[layer - PlayerHitbox];
        }
    }
}
