using UnityEngine;

namespace Ashar.Enemies
{
    /// <summary>
    /// The pure calculations of enemy movement: where an enemy is, at a given time after it appeared.
    /// RESPONSIBILITIES: turn an EnemyData's movement type and numbers into a position.
    /// HOW IT WORKS: two static methods. ChooseDirection picks the direction of travel once, when the enemy appears
    /// (down, towards the player, or across the screen). ComputePosition then gives the position at any time, from the
    /// starting point alone, with no stored state: the same time always gives the same position.
    /// WHY: with no Unity state these methods can be unit-tested, and a movement can never drift or accumulate errors.
    /// </summary>
    public static class EnemyMovement
    {
        /// <summary>
        /// Chooses the direction of travel at the moment the enemy appears: down for Straight and Sine, towards the target
        /// for Dive (down if there is no usable target), across the screen for LateralSweep (away from the side it came
        /// from), and none for Orbit and Static.
        /// </summary>
        public static Vector2 ChooseDirection(EnemyMovementType type, Vector2 start, Vector2 target, bool hasTarget)
        {
            switch (type)
            {
                case EnemyMovementType.Dive:
                    Vector2 toTarget = target - start;
                    return hasTarget && toTarget.sqrMagnitude > 0.0001f ? toTarget.normalized : Vector2.down;

                case EnemyMovementType.LateralSweep:
                    return start.x > 0f ? Vector2.left : Vector2.right;

                case EnemyMovementType.Orbit:
                case EnemyMovementType.Static:
                    return Vector2.zero;

                default:
                    return Vector2.down;
            }
        }

        /// <summary>
        /// Returns the position of the enemy at a given time after it appeared. For lines and sweeps it is the start plus
        /// the travel, plus a swing at right angles to the travel. For Orbit it is a point on a circle that passes through
        /// the start. For Static it is the start.
        /// </summary>
        public static Vector2 ComputePosition(EnemyMovementType type, Vector2 start, Vector2 direction, float speed, float amplitude, float frequency, float time)
        {
            switch (type)
            {
                case EnemyMovementType.Static:
                    return start;

                case EnemyMovementType.Orbit:
                    if (amplitude <= 0f)
                    {
                        return start;
                    }

                    float angle = speed / amplitude * time; // Angular speed = tangential speed / radius.
                    return start + new Vector2(amplitude * (Mathf.Cos(angle) - 1f), amplitude * Mathf.Sin(angle));

                default:
                    Vector2 travelled = start + direction * (speed * time);
                    Vector2 sideways = new Vector2(-direction.y, direction.x); // Right angle to the direction of travel.
                    float swing = amplitude * Mathf.Sin(2f * Mathf.PI * frequency * time);
                    return type == EnemyMovementType.Straight || type == EnemyMovementType.Dive ? travelled : travelled + sideways * swing;
            }
        }
    }
}
