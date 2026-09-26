using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ashar.Combat
{
    /// <summary>
    /// A deliberately minimal object pool, used only by the performance test bench (story E2-01).
    /// RESPONSIBILITIES: hand out bullets, reusing released ones instead of creating new ones.
    /// HOW IT WORKS: released bullets are switched off and kept in a stack. Get() takes the last one and switches it on;
    /// only when the stack is empty does it create a new bullet, so the pool grows to the highest number of bullets
    /// alive at once and then stops creating anything.
    /// WHY: it exists to measure what pooling would save. It is NOT the game's pooling: that is decided by the
    /// measure and, if wanted, built properly in its own story.
    /// </summary>
    public class StressTestPool
    {
        private readonly Stack<StressBulletController> _free = new Stack<StressBulletController>(); // Switched-off bullets ready to reuse.
        private readonly Func<StressBulletController> _create;                                       // Creates a brand new bullet when none is free.

        /// <summary>Creates an empty pool that builds new bullets with the given function.</summary>
        public StressTestPool(Func<StressBulletController> create)
        {
            _create = create;
        }

        /// <summary>Returns a switched-on bullet at the given position, reused if one is free, new otherwise.</summary>
        public StressBulletController Get(Vector3 position)
        {
            StressBulletController bullet = _free.Count > 0 ? _free.Pop() : _create();
            bullet.transform.position = position;
            bullet.gameObject.SetActive(true);
            return bullet;
        }

        /// <summary>Switches a bullet off and keeps it for reuse.</summary>
        public void Release(StressBulletController bullet)
        {
            bullet.gameObject.SetActive(false);
            _free.Push(bullet);
        }
    }
}
