using Ashar.Core;
using Ashar.Player;
using NUnit.Framework;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the rules of the arcade loop: lives, score, game over, continue, and the blinking of the protected ship.
    /// WHY: these rules decide when a game ends and what a run is worth. They live in a class with no Unity code precisely so
    /// that every case (last life, late hit, continue, unlimited continues) can be checked exactly.
    /// </summary>
    public class ArcadeLoopTests
    {
        private const float Delta = 0.0001f; // Tolerance for float comparisons.

        /// <summary>A new game starts with the given lives, a score of 0 and normal play.</summary>
        [Test]
        public void NewSession_StartsWithLivesAndZeroScore()
        {
            var session = new GameSession(3, -1);

            Assert.AreEqual(3, session.Lives);
            Assert.AreEqual(0, session.Score);
            Assert.AreEqual(GameState.Gameplay, session.State);
        }

        /// <summary>Points add up during normal play; zero and negative points change nothing.</summary>
        [Test]
        public void AddScore_AddsPositivePointsOnly()
        {
            var session = new GameSession(3, -1);

            session.AddScore(100);
            session.AddScore(20);
            session.AddScore(0);
            session.AddScore(-50);

            Assert.AreEqual(120, session.Score);
        }

        /// <summary>A hit costs a life and the player respawns while lives remain.</summary>
        [Test]
        public void LoseLife_WithLivesLeft_RespawnsAndStaysInGameplay()
        {
            var session = new GameSession(3, -1);

            bool respawn = session.LoseLife();

            Assert.IsTrue(respawn);
            Assert.AreEqual(2, session.Lives);
            Assert.AreEqual(GameState.Gameplay, session.State);
        }

        /// <summary>Losing the last life is a game over, with 0 lives shown.</summary>
        [Test]
        public void LoseLife_LastLife_IsGameOver()
        {
            var session = new GameSession(1, -1);

            bool respawn = session.LoseLife();

            Assert.IsFalse(respawn);
            Assert.AreEqual(0, session.Lives);
            Assert.AreEqual(GameState.GameOver, session.State);
        }

        /// <summary>After a game over, a late hit does not take a second life and no points are earned.</summary>
        [Test]
        public void AfterGameOver_LateHitsAndPointsChangeNothing()
        {
            var session = new GameSession(1, -1);
            session.AddScore(300);
            session.LoseLife();

            session.LoseLife();
            session.AddScore(500);

            Assert.AreEqual(0, session.Lives);
            Assert.AreEqual(300, session.Score);
        }

        /// <summary>A continue restores the start lives and puts the score back to 0 (spec §7.1).</summary>
        [Test]
        public void Continue_RestoresLivesAndResetsScore()
        {
            var session = new GameSession(3, -1);
            session.AddScore(900);
            session.LoseLife();
            session.LoseLife();
            session.LoseLife();

            bool continued = session.Continue();

            Assert.IsTrue(continued);
            Assert.AreEqual(3, session.Lives);
            Assert.AreEqual(0, session.Score);
            Assert.AreEqual(GameState.Gameplay, session.State);
        }

        /// <summary>A continue is not possible while the game is still on.</summary>
        [Test]
        public void Continue_DuringGameplay_IsRefused()
        {
            var session = new GameSession(3, -1);

            Assert.IsFalse(session.CanContinue);
            Assert.IsFalse(session.Continue());
        }

        /// <summary>With a limited number of continues, each use costs one, and none left means no continue.</summary>
        [Test]
        public void Continue_LimitedNumber_RunsOut()
        {
            var session = new GameSession(1, 1);
            session.LoseLife();
            Assert.IsTrue(session.Continue());   // The only continue.

            session.LoseLife();

            Assert.IsFalse(session.CanContinue);
            Assert.IsFalse(session.Continue());
            Assert.AreEqual(GameState.GameOver, session.State);
        }

        /// <summary>Unlimited continues (-1) never run out.</summary>
        [Test]
        public void Continue_Unlimited_NeverRunsOut()
        {
            var session = new GameSession(1, -1);

            for (int i = 0; i < 5; i++)
            {
                session.LoseLife();
                Assert.IsTrue(session.Continue());
            }
        }

        /// <summary>Ending the mission freezes the score: no more points and no more lost lives.</summary>
        [Test]
        public void EndMission_FreezesTheGame()
        {
            var session = new GameSession(3, -1);
            session.AddScore(400);

            session.EndMission();
            session.AddScore(100);
            session.LoseLife();

            Assert.AreEqual(GameState.MissionEnd, session.State);
            Assert.AreEqual(400, session.Score);
            Assert.AreEqual(3, session.Lives);
        }

        /// <summary>A game over cannot be turned into a mission end.</summary>
        [Test]
        public void EndMission_AfterGameOver_KeepsGameOver()
        {
            var session = new GameSession(1, -1);
            session.LoseLife();

            session.EndMission();

            Assert.AreEqual(GameState.GameOver, session.State);
        }

        /// <summary>A start with fewer than one life is corrected to one life, so a game can always begin.</summary>
        [Test]
        public void NewSession_ZeroLives_IsCorrectedToOne()
        {
            Assert.AreEqual(1, new GameSession(0, -1).Lives);
        }

        /// <summary>The protected ship is faint in the first half of each blink cycle and solid in the second half.</summary>
        [Test]
        public void ComputeBlinkAlpha_AlternatesFaintAndSolid()
        {
            Assert.AreEqual(0.3f, PlayerRespawnController.ComputeBlinkAlpha(0.01f, 0.14f, 0.3f), Delta);
            Assert.AreEqual(1f, PlayerRespawnController.ComputeBlinkAlpha(0.10f, 0.14f, 0.3f), Delta);
            Assert.AreEqual(0.3f, PlayerRespawnController.ComputeBlinkAlpha(0.15f, 0.14f, 0.3f), Delta);
        }

        /// <summary>A blink period of 0 means no blinking instead of dividing by zero.</summary>
        [Test]
        public void ComputeBlinkAlpha_ZeroPeriod_IsSolid()
        {
            Assert.AreEqual(1f, PlayerRespawnController.ComputeBlinkAlpha(0.3f, 0f, 0.3f), Delta);
        }
    }
}
