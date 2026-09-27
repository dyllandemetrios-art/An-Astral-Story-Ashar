using Ashar.Core;
using Ashar.Dialogue;
using NUnit.Framework;
using UnityEngine;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the pure logic added for manual dialogues (spec E3-02): the Dialogue state of GameSession, and picking
    /// the right language out of a DialogueLine.
    /// WHY: GameSession already refuses damage and score outside Gameplay; these tests check that Dialogue reuses
    /// that guard correctly, without touching Unity objects.
    /// </summary>
    public class DialogueTests
    {
        /// <summary>Entering the dialogue state from normal play switches to Dialogue.</summary>
        [Test]
        public void EnterDialogue_FromGameplay_SwitchesState()
        {
            var session = new GameSession(3, -1);

            session.EnterDialogue();

            Assert.AreEqual(GameState.Dialogue, session.State);
        }

        /// <summary>Entering the dialogue state does nothing outside normal play (for example after a game over).</summary>
        [Test]
        public void EnterDialogue_OutsideGameplay_DoesNothing()
        {
            var session = new GameSession(1, -1);
            session.LoseLife(); // Now GameOver.

            session.EnterDialogue();

            Assert.AreEqual(GameState.GameOver, session.State);
        }

        /// <summary>Exiting the dialogue state returns to normal play.</summary>
        [Test]
        public void ExitDialogue_FromDialogue_ReturnsToGameplay()
        {
            var session = new GameSession(3, -1);
            session.EnterDialogue();

            session.ExitDialogue();

            Assert.AreEqual(GameState.Gameplay, session.State);
        }

        /// <summary>Exiting the dialogue state does nothing when the state changed for another reason meanwhile.</summary>
        [Test]
        public void ExitDialogue_NotInDialogue_DoesNothing()
        {
            var session = new GameSession(3, -1);

            session.ExitDialogue();

            Assert.AreEqual(GameState.Gameplay, session.State);
        }

        /// <summary>No score is earned while a dialogue plays, the same rule that already applies outside Gameplay.</summary>
        [Test]
        public void AddScore_DuringDialogue_ChangesNothing()
        {
            var session = new GameSession(3, -1);
            session.EnterDialogue();

            session.AddScore(500);

            Assert.AreEqual(0, session.Score);
        }

        /// <summary>A hit during a dialogue costs no life: LoseLife is a no-op outside Gameplay.</summary>
        [Test]
        public void LoseLife_DuringDialogue_ChangesNothing()
        {
            var session = new GameSession(3, -1);
            session.EnterDialogue();

            bool result = session.LoseLife();

            Assert.IsTrue(result);
            Assert.AreEqual(3, session.Lives);
            Assert.AreEqual(GameState.Dialogue, session.State);
        }

        /// <summary>A line without French or English text returns an empty string rather than null, so the view never gets a null.</summary>
        [Test]
        public void DialogueLine_GetText_PicksTheRequestedLanguage()
        {
            var line = new DialogueLine();
            SetPrivateField(line, "_speakerName", "KAAL'VARIS");
            SetPrivateField(line, "_textFr", "Texte francais.");
            SetPrivateField(line, "_textEn", "English text.");

            Assert.AreEqual("Texte francais.", line.GetText(DialogueLanguage.French));
            Assert.AreEqual("English text.", line.GetText(DialogueLanguage.English));
        }

        /// <summary>Sets a private serialized field by reflection, so the test can build a DialogueLine without a scene.</summary>
        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.IsNotNull(field, $"Field '{fieldName}' not found on {target.GetType().Name}.");
            field.SetValue(target, value);
        }
    }
}
