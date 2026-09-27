namespace Ashar.Core
{
    /// <summary>
    /// The state of one game in progress: lives, score, continues left, and whether the game is on, over or won.
    /// RESPONSIBILITIES: apply the rules of the arcade loop (a hit costs a life, no life left means game over, a continue
    /// starts again with fresh lives and a score of 0) and hold the current numbers.
    /// HOW IT WORKS: a plain C# object with no Unity code. GameSessionController creates one, feeds it the game events
    /// (enemy killed, player hit...) and announces the changes. Static Current gives every script a single access point to
    /// the state of the running game (spec: "un seul point d'accès global").
    /// WHY: with no Unity dependency, every rule of the loop can be unit-tested exactly.
    /// </summary>
    public class GameSession
    {
        private readonly int _startLives;   // Lives given at the start and after a continue.
        private int _continuesLeft;         // Continues still available; -1 = unlimited.

        /// <summary>The session of the game that is running, or null when none is.</summary>
        public static GameSession Current { get; set; }

        /// <summary>Creates a new game with the given number of lives and continues (-1 = unlimited).</summary>
        public GameSession(int startLives, int continues)
        {
            _startLives = startLives < 1 ? 1 : startLives;
            _continuesLeft = continues;
            Lives = _startLives;
            Score = 0;
            State = GameState.Gameplay;
        }

        /// <summary>Multiplier of enemy hit points (1 = the values of the enemy data).</summary>
        public float EnemyHpMultiplier { get; private set; } = 1f;

        /// <summary>Fire density multiplier (1 = the intervals of the pattern data).</summary>
        public float FireDensityMultiplier { get; private set; } = 1f;

        /// <summary>Multiplier of enemy bullet speed (1 = the speeds of the pattern data).</summary>
        public float BulletSpeedMultiplier { get; private set; } = 1f;

        /// <summary>Most bullets alive at once (0 = no limit).</summary>
        public int MaxProjectiles { get; private set; }

        /// <summary>Sets the global balance multipliers. Called every frame by GameSessionController so the asset can be tuned live.</summary>
        public void SetBalance(float enemyHpMultiplier, float fireDensityMultiplier, float bulletSpeedMultiplier, int maxProjectiles)
        {
            EnemyHpMultiplier = enemyHpMultiplier;
            FireDensityMultiplier = fireDensityMultiplier;
            BulletSpeedMultiplier = bulletSpeedMultiplier;
            MaxProjectiles = maxProjectiles;
        }

        /// <summary>Lives left.</summary>
        public int Lives { get; private set; }

        /// <summary>Current score.</summary>
        public int Score { get; private set; }

        /// <summary>Current state of the game.</summary>
        public GameState State { get; private set; }

        /// <summary>True when the game is over and the player may still continue.</summary>
        public bool CanContinue => State == GameState.GameOver && _continuesLeft != 0;

        /// <summary>Adds points to the score. Points are only earned during normal play, never after the game is over.</summary>
        public void AddScore(int points)
        {
            if (State == GameState.Gameplay && points > 0)
            {
                Score += points;
            }
        }

        /// <summary>
        /// Takes one life. Returns true if the player still has a life (and will respawn), false if the game is now over.
        /// Does nothing (and returns true) when the game is not in normal play, so a late hit cannot cost a second life.
        /// </summary>
        public bool LoseLife()
        {
            if (State != GameState.Gameplay)
            {
                return true;
            }

            Lives--;
            if (Lives <= 0)
            {
                Lives = 0;
                State = GameState.GameOver;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Continues after a game over: lives back to their start value, score back to 0 (arcade convention, spec §7.1).
        /// Returns false if no continue is available. A limited number of continues goes down by one each time.
        /// </summary>
        public bool Continue()
        {
            if (!CanContinue)
            {
                return false;
            }

            if (_continuesLeft > 0)
            {
                _continuesLeft--;
            }

            Lives = _startLives;
            Score = 0;
            State = GameState.Gameplay;
            return true;
        }

        /// <summary>Ends the mission: the score is final. Only happens from normal play.</summary>
        public void EndMission()
        {
            if (State == GameState.Gameplay)
            {
                State = GameState.MissionEnd;
            }
        }

        /// <summary>Enters the Dialogue state from normal play (spec E3-02): no damage or score while it lasts, since both require Gameplay.</summary>
        public void EnterDialogue()
        {
            if (State == GameState.Gameplay)
            {
                State = GameState.Dialogue;
            }
        }

        /// <summary>Returns to normal play once a dialogue closes. Does nothing if the state changed for another reason meanwhile.</summary>
        public void ExitDialogue()
        {
            if (State == GameState.Dialogue)
            {
                State = GameState.Gameplay;
            }
        }
    }
}
