using UnityEngine;

namespace Ashar.Enemies
{
    /// <summary>
    /// What an enemy needs to know about the scene when it is created: who to aim at, where it may live, where its effects go.
    /// HOW IT WORKS: the code that creates an enemy (the wave system, a debug spawner) fills one of these and passes it to
    /// EnemyController.Initialize().
    /// WHY: a prefab cannot point to objects of the scene, so the scene's objects are handed over at creation time instead of
    /// being searched for with Find calls.
    /// </summary>
    public readonly struct EnemySpawnContext
    {
        /// <summary>The player ship, aimed at by Dive enemies. May be null: the enemy then falls straight down.</summary>
        public readonly Transform Target;

        /// <summary>The area in which the enemy may live; it is destroyed when it leaves it.</summary>
        public readonly Rect LifeBounds;

        /// <summary>Scene object under which explosions and other effects are created (Runtime/FX). May be null.</summary>
        public readonly Transform FxParent;

        /// <summary>Creates a context from its three parts.</summary>
        public EnemySpawnContext(Transform target, Rect lifeBounds, Transform fxParent)
        {
            Target = target;
            LifeBounds = lifeBounds;
            FxParent = fxParent;
        }
    }
}
