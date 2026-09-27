namespace Ashar.Core
{
    /// <summary>
    /// The states the game can be in during a mission (spec §5.3). Dialogue and Paused are added by their own stories.
    /// </summary>
    public enum GameState
    {
        /// <summary>Normal play: the player flies, fights and scores.</summary>
        Gameplay,

        /// <summary>The wave table has ended: the mission is over and the score is final.</summary>
        MissionEnd,

        /// <summary>The player has lost their last life and has not continued yet.</summary>
        GameOver,
    }
}
