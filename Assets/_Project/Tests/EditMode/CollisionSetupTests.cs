using Ashar.Core;
using Ashar.Player;
using NUnit.Framework;
using UnityEngine;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Checks that the project's physics layers and collision matrix still match the spec (§5.4) and the Layers class.
    /// WHY: layers and the matrix live in project settings, where a change made by hand in the Editor would
    /// silently break the hit rules. These tests turn such a change into a red test.
    /// </summary>
    public class CollisionSetupTests
    {
        /// <summary>Each game layer index has the name that Layers declares.</summary>
        [Test]
        public void Layers_MatchProjectSettings()
        {
            for (int i = 0; i < Layers.Names.Length; i++)
            {
                Assert.AreEqual(Layers.Names[i], LayerMask.LayerToName(Layers.PlayerHitbox + i), $"Layer {Layers.PlayerHitbox + i}");
            }
        }

        /// <summary>The collision matrix allows exactly the five pairs of the spec table, and nothing else.</summary>
        [Test]
        public void CollisionMatrix_MatchesSpecTable()
        {
            (int, int)[] allowed =
            {
                (Layers.PlayerHitbox, Layers.EnemyBullet),
                (Layers.PlayerHitbox, Layers.Enemy),
                (Layers.PlayerGraze, Layers.EnemyBullet),
                (Layers.PlayerBullet, Layers.Enemy),
                (Layers.PowerUp, Layers.PlayerHitbox),
            };

            for (int a = Layers.PlayerHitbox; a <= Layers.PowerUp; a++)
            {
                for (int b = a; b <= Layers.PowerUp; b++)
                {
                    bool shouldCollide = System.Array.Exists(allowed, pair => (pair.Item1 == a && pair.Item2 == b) || (pair.Item1 == b && pair.Item2 == a));
                    Assert.AreEqual(shouldCollide, !Physics2D.GetIgnoreLayerCollision(a, b), $"{Layers.NameOf(a)} x {Layers.NameOf(b)}");
                }
            }
        }

        /// <summary>The game layers never collide with the Default layer, so scenery can never trigger a hit.</summary>
        [Test]
        public void GameLayers_IgnoreDefaultLayer()
        {
            for (int layer = Layers.PlayerHitbox; layer <= Layers.PowerUp; layer++)
            {
                Assert.IsTrue(Physics2D.GetIgnoreLayerCollision(layer, 0), Layers.NameOf(layer));
            }
        }

        /// <summary>A hit counts unless the ship is invulnerable.</summary>
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void ShouldRegisterHit_DependsOnInvulnerability(bool invulnerable, bool expected)
        {
            Assert.AreEqual(expected, PlayerHealthController.ShouldRegisterHit(invulnerable));
        }
    }
}
