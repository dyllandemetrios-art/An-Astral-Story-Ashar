using Ashar.Player;
using NUnit.Framework;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the ship banking calculation: which frame set the horizontal input asks for, and which frame of it plays.
    /// WHY: "leans left when moving left" is easy to get backwards (a sign mistake), and the deadzone and looping must
    /// behave exactly, so both are cheap to pin down here without playing.
    /// </summary>
    public class PlayerPolishTests
    {
        /// <summary>A clearly negative input asks for the left-banking frames.</summary>
        [Test]
        public void ComputeBankState_NegativeInput_IsLeft()
        {
            Assert.AreEqual(ShipBankState.Left, PlayerBankController.ComputeBankState(-1f, 0.15f));
        }

        /// <summary>A clearly positive input asks for the right-banking frames.</summary>
        [Test]
        public void ComputeBankState_PositiveInput_IsRight()
        {
            Assert.AreEqual(ShipBankState.Right, PlayerBankController.ComputeBankState(1f, 0.15f));
        }

        /// <summary>No input, or a light touch inside the deadzone, keeps the level flight loop.</summary>
        [Test]
        public void ComputeBankState_InsideDeadzone_IsBase()
        {
            Assert.AreEqual(ShipBankState.Base, PlayerBankController.ComputeBankState(0f, 0.15f));
            Assert.AreEqual(ShipBankState.Base, PlayerBankController.ComputeBankState(0.1f, 0.15f));
            Assert.AreEqual(ShipBankState.Base, PlayerBankController.ComputeBankState(-0.1f, 0.15f));
        }

        /// <summary>An input right at the edge of the deadzone already counts as a bank (the deadzone is exclusive).</summary>
        [Test]
        public void ComputeBankState_AtDeadzoneEdge_Banks()
        {
            Assert.AreEqual(ShipBankState.Right, PlayerBankController.ComputeBankState(0.15f, 0.15f));
            Assert.AreEqual(ShipBankState.Left, PlayerBankController.ComputeBankState(-0.15f, 0.15f));
        }

        /// <summary>The first frame of a loop is shown at time 0.</summary>
        [Test]
        public void LoopFrameIndex_AtStart_IsFrameZero()
        {
            Assert.AreEqual(0, PlayerBankController.LoopFrameIndex(0f, 12f, 8));
        }

        /// <summary>Partway through, the frame matches elapsed time x frame rate.</summary>
        [Test]
        public void LoopFrameIndex_Partway_MatchesElapsedTimes12Fps()
        {
            Assert.AreEqual(3, PlayerBankController.LoopFrameIndex(0.29f, 12f, 8)); // 0.29 x 12 = 3.48 -> 3.
        }

        /// <summary>Past the end of the strip, the animation wraps back to the start instead of running off the array.</summary>
        [Test]
        public void LoopFrameIndex_PastTheEnd_WrapsAround()
        {
            Assert.AreEqual(1, PlayerBankController.LoopFrameIndex(0.75f, 12f, 8)); // 0.75 x 12 = 9 -> 9 % 8 = 1.
        }

        /// <summary>A frame count of 0 never indexes out of range.</summary>
        [Test]
        public void LoopFrameIndex_NoFrames_IsZero()
        {
            Assert.AreEqual(0, PlayerBankController.LoopFrameIndex(1f, 12f, 0));
        }
    }
}
