using Ashar.Core;
using NUnit.Framework;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the pure logic added for the pause and options menus (spec E3-06): the Paused state of GameSession, and
    /// the validation done by PlayerPreferences when it loads a stored value.
    /// WHY: these rules decide what state a pause restores and what happens to a corrupted or missing preference;
    /// both live in plain C# precisely so every case can be checked without a scene.
    /// </summary>
    public class MenuTests
    {
        private const float Delta = 0.0001f;

        /// <summary>Pausing from normal play switches to Paused and remembers Gameplay as the state to restore.</summary>
        [Test]
        public void EnterPause_FromGameplay_SwitchesStateAndRemembersIt()
        {
            var session = new GameSession(3, -1);

            session.EnterPause();

            Assert.AreEqual(GameState.Paused, session.State);

            session.ExitPause();
            Assert.AreEqual(GameState.Gameplay, session.State);
        }

        /// <summary>Pausing during a dialogue remembers Dialogue, not Gameplay, as the state to restore.</summary>
        [Test]
        public void EnterPause_FromDialogue_RestoresDialogueOnExit()
        {
            var session = new GameSession(3, -1);
            session.EnterDialogue();

            session.EnterPause();
            Assert.AreEqual(GameState.Paused, session.State);

            session.ExitPause();
            Assert.AreEqual(GameState.Dialogue, session.State);
        }

        /// <summary>Pause is refused from GameOver: the spec offers no pause on that screen.</summary>
        [Test]
        public void EnterPause_FromGameOver_DoesNothing()
        {
            var session = new GameSession(1, -1);
            session.LoseLife(); // Now GameOver.

            session.EnterPause();

            Assert.AreEqual(GameState.GameOver, session.State);
        }

        /// <summary>Pause is refused after the mission has ended.</summary>
        [Test]
        public void EnterPause_FromMissionEnd_DoesNothing()
        {
            var session = new GameSession(3, -1);
            session.EndMission();

            session.EnterPause();

            Assert.AreEqual(GameState.MissionEnd, session.State);
        }

        /// <summary>Opening the pause menu a second time while already paused changes nothing: the original return state survives.</summary>
        [Test]
        public void EnterPause_WhileAlreadyPaused_KeepsOriginalReturnState()
        {
            var session = new GameSession(3, -1);
            session.EnterDialogue();

            session.EnterPause();
            session.EnterPause(); // Double pause request: must not overwrite the remembered Dialogue with Paused.
            session.ExitPause();

            Assert.AreEqual(GameState.Dialogue, session.State);
        }

        /// <summary>Exiting pause when not paused does nothing (the state changed for another reason while paused was never entered).</summary>
        [Test]
        public void ExitPause_NotPaused_DoesNothing()
        {
            var session = new GameSession(3, -1);

            session.ExitPause();

            Assert.AreEqual(GameState.Gameplay, session.State);
        }

        /// <summary>No damage or score while paused, the same rule already used for Dialogue and non-Gameplay states.</summary>
        [Test]
        public void AddScoreAndLoseLife_DuringPause_ChangeNothing()
        {
            var session = new GameSession(3, -1);
            session.EnterPause();

            session.AddScore(500);
            bool result = session.LoseLife();

            Assert.AreEqual(0, session.Score);
            Assert.IsTrue(result);
            Assert.AreEqual(3, session.Lives);
        }

        /// <summary>A volume within range is kept exactly as given.</summary>
        [Test]
        public void ClampVolume_InRange_IsUnchanged()
        {
            Assert.AreEqual(0.5f, PlayerPreferences.ClampVolume(0.5f), Delta);
            Assert.AreEqual(0f, PlayerPreferences.ClampVolume(0f), Delta);
            Assert.AreEqual(1f, PlayerPreferences.ClampVolume(1f), Delta);
        }

        /// <summary>An out-of-range or corrupted volume is clamped to [0, 1] instead of being applied as-is.</summary>
        [Test]
        public void ClampVolume_OutOfRange_IsClamped()
        {
            Assert.AreEqual(0f, PlayerPreferences.ClampVolume(-3f), Delta);
            Assert.AreEqual(1f, PlayerPreferences.ClampVolume(4.2f), Delta);
        }

        /// <summary>A recognised language name parses back to that language.</summary>
        [Test]
        public void ParseLanguage_KnownName_ParsesIt()
        {
            Assert.AreEqual(Language.English, PlayerPreferences.ParseLanguage("English"));
            Assert.AreEqual(Language.French, PlayerPreferences.ParseLanguage("French"));
        }

        /// <summary>A missing or corrupted stored language falls back to French, never to an exception or an undefined value.</summary>
        [Test]
        public void ParseLanguage_MissingOrInvalid_FallsBackToFrench()
        {
            Assert.AreEqual(Language.French, PlayerPreferences.ParseLanguage(""));
            Assert.AreEqual(Language.French, PlayerPreferences.ParseLanguage(null));
            Assert.AreEqual(Language.French, PlayerPreferences.ParseLanguage("Klingon"));
        }
    }
}
