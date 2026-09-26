using UnityEngine;

namespace Ashar.Combat
{
    /// <summary>
    /// A bullet of the performance test bench (story E2-01): flies straight and tells its owner when it has left the screen.
    /// RESPONSIBILITIES: move at a fixed velocity and hand itself back to the bench when outside its life bounds.
    /// HOW IT WORKS: the bench calls Launch() right after creating or reusing the bullet. Every frame the bullet moves
    /// through its transform, like the real ProjectileController does, and asks the bench to release it once outside.
    /// WHY: the bench must be able to destroy a bullet OR send it back to a pool, and the game's ProjectileController
    /// always destroys itself. A separate class keeps the game code untouched by an experiment, while the prefab has the
    /// same physics parts (kinematic body, trigger collider, sprite) so the measured cost is realistic.
    /// </summary>
    public class StressBulletController : MonoBehaviour
    {
        private ProjectileStressTestController _owner; // The bench that created this bullet and takes it back.
        private Vector2 _velocity;                     // World units per second.
        private Rect _lifeBounds;                      // The bullet is released when it leaves this rectangle.
        private bool _flying;                          // True between Launch() and the release; stops a double release.

        /// <summary>Starts (or restarts) the flight of the bullet.</summary>
        public void Launch(Vector2 velocity, Rect lifeBounds, ProjectileStressTestController owner)
        {
            _velocity = velocity;
            _lifeBounds = lifeBounds;
            _owner = owner;
            _flying = true;
        }

        /// <summary>Moves the bullet and hands it back to the bench once it is outside its life bounds.</summary>
        private void Update()
        {
            if (!_flying)
            {
                return;
            }

            Vector3 position = transform.position;
            position.x += _velocity.x * Time.deltaTime;
            position.y += _velocity.y * Time.deltaTime;
            transform.position = position;

            if (!_lifeBounds.Contains(position))
            {
                _flying = false;
                _owner.ReleaseBullet(this);
            }
        }
    }
}
