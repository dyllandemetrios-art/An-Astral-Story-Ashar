using System.Collections.Generic;
using Ashar.Waves;
using NUnit.Framework;
using UnityEngine;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Tests the pure calculations of the wave system: where enemies of a group appear, the order of a wave table, and the
    /// smooth change of scroll speed.
    /// WHY: a level is written as a table, so the table must always mean what it says: an enemy that appears in the wrong
    /// place or a timeline in the wrong order would ruin a level without any error message.
    /// </summary>
    public class WaveTests
    {
        private const float Delta = 0.0001f;                              // Tolerance for float comparisons.
        private static readonly Rect Screen = new Rect(-10f, -5.625f, 20f, 11.25f); // The visible screen, in world units.

        /// <summary>A single enemy entering from the top appears just above the top edge, in the middle of the screen.</summary>
        [Test]
        public void ComputeSpawnPosition_SingleFromTop_IsAboveTopEdge()
        {
            Vector2 position = WaveMath.ComputeSpawnPosition(WaveEntry.Top, WaveFormation.Single, 0f, 0f, 0, 1, 1.8f, Screen, 1f);

            Assert.AreEqual(0f, position.x, Delta);
            Assert.AreEqual(6.625f, position.y, Delta); // 5.625 + 1.
        }

        /// <summary>The entry offset shifts the entry point sideways for the top, and up for the sides.</summary>
        [Test]
        public void ComputeSpawnPosition_Offset_ShiftsTheEntryPoint()
        {
            Vector2 top = WaveMath.ComputeSpawnPosition(WaveEntry.Top, WaveFormation.Single, 3f, 0f, 0, 1, 1.8f, Screen, 1f);
            Vector2 left = WaveMath.ComputeSpawnPosition(WaveEntry.Left, WaveFormation.Single, 2f, 0f, 0, 1, 1.8f, Screen, 1f);

            Assert.AreEqual(3f, top.x, Delta);
            Assert.AreEqual(-11f, left.x, Delta);
            Assert.AreEqual(2f, left.y, Delta);
        }

        /// <summary>The right entry mirrors the left one.</summary>
        [Test]
        public void ComputeSpawnPosition_FromRight_IsBesideRightEdge()
        {
            Vector2 position = WaveMath.ComputeSpawnPosition(WaveEntry.Right, WaveFormation.Single, 0f, 0f, 0, 1, 1.8f, Screen, 1f);

            Assert.AreEqual(11f, position.x, Delta);
            Assert.AreEqual(0f, position.y, Delta);
        }

        /// <summary>The Screen entry puts the enemy on the screen, the given depth below the top edge.</summary>
        [Test]
        public void ComputeSpawnPosition_ScreenEntry_IsOnScreenAtDepth()
        {
            Vector2 position = WaveMath.ComputeSpawnPosition(WaveEntry.Screen, WaveFormation.Single, -4f, 2.5f, 0, 1, 1.8f, Screen, 1f);

            Assert.AreEqual(-4f, position.x, Delta);
            Assert.AreEqual(3.125f, position.y, Delta); // 5.625 - 2.5.
        }

        /// <summary>A line of three from the top is spread evenly around the entry point, all at the same height.</summary>
        [Test]
        public void ComputeSpawnPosition_Line_SpreadsEvenly()
        {
            var xs = new float[3];
            for (int i = 0; i < 3; i++)
            {
                Vector2 position = WaveMath.ComputeSpawnPosition(WaveEntry.Top, WaveFormation.Line, 0f, 0f, i, 3, 2f, Screen, 1f);
                xs[i] = position.x;
                Assert.AreEqual(6.625f, position.y, Delta);
            }

            Assert.AreEqual(-2f, xs[0], Delta);
            Assert.AreEqual(0f, xs[1], Delta);
            Assert.AreEqual(2f, xs[2], Delta);
        }

        /// <summary>In a V the middle enemy leads (closest to the screen) and the outer ones are further back.</summary>
        [Test]
        public void ComputeSpawnPosition_V_MiddleLeads()
        {
            Vector2 left = WaveMath.ComputeSpawnPosition(WaveEntry.Top, WaveFormation.V, 0f, 0f, 0, 3, 2f, Screen, 1f);
            Vector2 middle = WaveMath.ComputeSpawnPosition(WaveEntry.Top, WaveFormation.V, 0f, 0f, 1, 3, 2f, Screen, 1f);
            Vector2 right = WaveMath.ComputeSpawnPosition(WaveEntry.Top, WaveFormation.V, 0f, 0f, 2, 3, 2f, Screen, 1f);

            Assert.AreEqual(6.625f, middle.y, Delta);
            Assert.AreEqual(6.625f + 1.2f, left.y, Delta);  // 2 x 0.6 further back.
            Assert.AreEqual(left.y, right.y, Delta);
            Assert.AreEqual(-left.x, right.x, Delta);
        }

        /// <summary>A column keeps every enemy at the entry point: the delay between units separates them.</summary>
        [Test]
        public void ComputeSpawnPosition_Column_SharesTheEntryPoint()
        {
            Vector2 first = WaveMath.ComputeSpawnPosition(WaveEntry.Top, WaveFormation.Column, 1f, 0f, 0, 4, 2f, Screen, 1f);
            Vector2 last = WaveMath.ComputeSpawnPosition(WaveEntry.Top, WaveFormation.Column, 1f, 0f, 3, 4, 2f, Screen, 1f);

            Assert.AreEqual(first, last);
        }

        /// <summary>A table whose times only go forward has no problem.</summary>
        [Test]
        public void FindTimeOrderProblem_IncreasingTimes_ReturnsMinusOne()
        {
            Assert.AreEqual(-1, WaveData.FindTimeOrderProblem(Events(0f, 2f, 2f, 5f)));
        }

        /// <summary>A time that goes back inside a block is found, with the index of the offending event.</summary>
        [Test]
        public void FindTimeOrderProblem_TimeGoesBack_ReturnsItsIndex()
        {
            Assert.AreEqual(2, WaveData.FindTimeOrderProblem(Events(0f, 5f, 3f)));
        }

        /// <summary>After a WaitForClear, times start again from 0, so a smaller time is not a problem.</summary>
        [Test]
        public void FindTimeOrderProblem_TimesRestartAfterWaitForClear()
        {
            var events = new List<WaveEvent> { Event(0f, WaveEventType.Spawn), Event(6f, WaveEventType.WaitForClear), Event(1f, WaveEventType.Spawn) };

            Assert.AreEqual(-1, WaveData.FindTimeOrderProblem(events));
        }

        /// <summary>After a Dialogue, times also start again from 0 (spec E3-02): MissionRunnerController resets its block clock the same way.</summary>
        [Test]
        public void FindTimeOrderProblem_TimesRestartAfterDialogue()
        {
            var events = new List<WaveEvent> { Event(0f, WaveEventType.Spawn), Event(4f, WaveEventType.Dialogue), Event(1f, WaveEventType.Spawn) };

            Assert.AreEqual(-1, WaveData.FindTimeOrderProblem(events));
        }

        /// <summary>The scroll speed goes from the start value to the target in a straight line, and ends exactly on the target.</summary>
        [Test]
        public void ComputeScrollSpeed_GoesFromStartToTarget()
        {
            Assert.AreEqual(4f, WaveMath.ComputeScrollSpeed(4f, 8f, 0f, 2f), Delta);
            Assert.AreEqual(6f, WaveMath.ComputeScrollSpeed(4f, 8f, 1f, 2f), Delta);
            Assert.AreEqual(8f, WaveMath.ComputeScrollSpeed(4f, 8f, 2f, 2f), Delta);
            Assert.AreEqual(8f, WaveMath.ComputeScrollSpeed(4f, 8f, 5f, 2f), Delta);
        }

        /// <summary>A transition of duration 0 is instant.</summary>
        [Test]
        public void ComputeScrollSpeed_ZeroDuration_IsTargetAtOnce()
        {
            Assert.AreEqual(0f, WaveMath.ComputeScrollSpeed(4f, 0f, 0f, 0f), Delta);
        }

        /// <summary>Builds a list of Spawn events with the given times.</summary>
        private static List<WaveEvent> Events(params float[] times)
        {
            var list = new List<WaveEvent>();
            foreach (float time in times)
            {
                list.Add(Event(time, WaveEventType.Spawn));
            }

            return list;
        }

        /// <summary>Builds one wave event with a time and a type, through its serialized fields.</summary>
        private static WaveEvent Event(float time, WaveEventType type)
        {
            var waveEvent = new WaveEvent();
            typeof(WaveEvent).GetField("_time", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(waveEvent, time);
            typeof(WaveEvent).GetField("_type", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(waveEvent, type);
            return waveEvent;
        }
    }
}
