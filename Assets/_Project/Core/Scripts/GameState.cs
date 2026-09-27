namespace Ashar.Core
{
    /// <summary>
    /// The states the game can be in during a mission (spec §5.3).
    /// </summary>
    public enum GameState
    {
        /// <summary>Normal play: the player flies, fights and scores.</summary>
        Gameplay,

        /// <summary>The wave table has ended: the mission is over and the score is final.</summary>
        MissionEnd,

        /// <summary>The player has lost their last life and has not continued yet.</summary>
        GameOver,

        /// <summary>
        /// A manual dialogue plays between combats (spec E3-02): the world keeps flying, but no spawn, attack, damage
        /// or score happens until it closes.
        /// </summary>
        Dialogue,

        /// <summary>
        /// A voluntary pause (spec E3-06): time is frozen (Time.timeScale = 0) and combat/dialogue inputs are
        /// explicitly suspended, since freezing time alone does not stop a script from reading a fresh button press.
        /// </summary>
        Paused,
    }
}
