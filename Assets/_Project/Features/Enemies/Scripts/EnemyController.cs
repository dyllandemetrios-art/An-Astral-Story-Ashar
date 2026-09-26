using Ashar.Combat;
using Ashar.Core;
using UnityEngine;

namespace Ashar.Enemies
{
    /// <summary>
    /// The behaviour shared by every enemy: it moves as its EnemyData says, takes damage, flashes when hit and dies.
    /// RESPONSIBILITIES: follow the movement of its data, lose hit points when a player bullet touches it, announce its
    /// death with the score it is worth, show an explosion, and remove itself when it leaves the play space.
    /// HOW IT WORKS: whoever creates the enemy calls Initialize() with the data and the scene context. From then on the
    /// enemy computes its position from the time since it appeared (see EnemyMovement). Player bullets are triggers on
    /// the PlayerBullet layer; the collision matrix lets them meet only enemies, and the enemy's Rigidbody2D makes Unity
    /// deliver the contact to this script.
    /// WHY: one script for all enemies, with the differences in data, so a new enemy is a new asset and a new sprite.
    /// The flash on a hit uses the SpriteFlash shader through a MaterialPropertyBlock, like the player ship.
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        private static readonly int FlashColorId = Shader.PropertyToID("_FlashColor");   // Shader property ids are faster than names.
        private static readonly int FlashAmountId = Shader.PropertyToID("_FlashAmount");

        [Header("References")]
        [SerializeField, Tooltip("Renderer of the enemy image (the Visual child). It must use the SpriteFlash shader for the hit flash to show.")]
        private SpriteRenderer _visual;

        [SerializeField, Tooltip("Optional explosion created where the enemy dies.")]
        private GameObject _deathEffect;

        [Header("Hit flash")]
        [SerializeField, Min(0f), Tooltip("How long the enemy flashes when hit, in seconds.")]
        private float _flashDuration = 0.06f;

        [SerializeField, Tooltip("Colour of the hit flash.")]
        private Color _flashColor = Color.white;

        [SerializeField, Range(0f, 1f), Tooltip("How strongly the enemy is pushed to the flash colour at the start of the flash.")]
        private float _flashStrength = 0.9f;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: hit points left.")]
        private float _hp;

        [SerializeField, Tooltip("Read-only: kind of enemy (its data id).")]
        private string _dataId;

        private EnemyData _data;             // What this enemy is.
        private EnemySpawnContext _context;  // What it needs to know about the scene.
        private Vector2 _start;              // Where it appeared.
        private Vector2 _direction;          // Direction of travel, chosen once when it appeared.
        private float _time;                 // Seconds since it appeared.
        private float _flashTimer = -1f;     // Seconds since the last hit, or negative when no flash is running.
        private bool _initialized;           // False until Initialize() has been called.
        private bool _dead;                  // True once dead, so it can die only once.
        private MaterialPropertyBlock _block; // Per-renderer shader values, so the shared material is left alone.

        /// <summary>Hit points left.</summary>
        public float Hp => _hp;

        /// <summary>The data this enemy was created from.</summary>
        public EnemyData Data => _data;

        /// <summary>Sets what kind of enemy this is. Must be called once, right after the enemy is created.</summary>
        public void Initialize(EnemyData data, EnemySpawnContext context)
        {
            _data = data;
            _context = context;
            _hp = data.MaxHp;
            _dataId = data.Id;
            _start = transform.position;
            bool hasTarget = context.Target != null;
            Vector2 target = hasTarget ? (Vector2)context.Target.position : Vector2.zero;
            _direction = EnemyMovement.ChooseDirection(data.Movement, _start, target, hasTarget);
            _time = 0f;
            _block = new MaterialPropertyBlock();
            ApplyFlash(0f);
            _initialized = true;
        }

        /// <summary>Moves the enemy along its path, removes it when it has left the play space, and runs the hit flash.</summary>
        private void Update()
        {
            if (!_initialized)
            {
                return;
            }

            _time += Time.deltaTime;
            Vector2 position = EnemyMovement.ComputePosition(_data.Movement, _start, _direction, _data.Speed, _data.Amplitude, _data.Frequency, _time);
            transform.position = new Vector3(position.x, position.y, transform.position.z);

            if (!_context.LifeBounds.Contains(position))
            {
                Destroy(gameObject); // Left the screen without being killed: no score, no explosion.
                return;
            }

            UpdateFlash();
        }

        /// <summary>Called by Unity when another trigger enters this enemy: a player bullet costs it hit points.</summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_dead || other.gameObject.layer != Layers.PlayerBullet)
            {
                return;
            }

            ProjectileController bullet = other.GetComponentInParent<ProjectileController>();
            if (bullet == null)
            {
                return;
            }

            TakeDamage(bullet.Damage);
            Destroy(bullet.gameObject); // The bullet is spent.
        }

        /// <summary>Removes hit points, starts the hit flash, and kills the enemy when none are left.</summary>
        public void TakeDamage(float damage)
        {
            if (_dead)
            {
                return;
            }

            _hp = ApplyDamage(_hp, damage);
            _flashTimer = 0f;

            if (_hp <= 0f)
            {
                Die();
            }
        }

        /// <summary>Announces the death, shows the explosion and removes the enemy.</summary>
        private void Die()
        {
            _dead = true;
            GameEvents.RaiseEnemyKilled(_data.Score);

            if (_deathEffect != null)
            {
                Instantiate(_deathEffect, transform.position, Quaternion.identity, _context.FxParent);
            }

            Destroy(gameObject);
        }

        /// <summary>Runs the hit flash: full strength at the hit, fading to nothing.</summary>
        private void UpdateFlash()
        {
            if (_flashTimer < 0f)
            {
                return;
            }

            _flashTimer += Time.deltaTime;
            if (_flashTimer >= _flashDuration)
            {
                _flashTimer = -1f;
                ApplyFlash(0f);
                return;
            }

            ApplyFlash(ComputeHitFlash(_flashTimer, _flashDuration, _flashStrength));
        }

        /// <summary>Sends the flash colour and amount to the shader of the enemy image.</summary>
        private void ApplyFlash(float amount)
        {
            _visual.GetPropertyBlock(_block);
            _block.SetColor(FlashColorId, _flashColor);
            _block.SetFloat(FlashAmountId, amount);
            _visual.SetPropertyBlock(_block);
        }

        /// <summary>
        /// Returns the hit points left after taking damage, never below zero. Static and free of Unity state so it can be
        /// unit-tested.
        /// </summary>
        public static float ApplyDamage(float hp, float damage)
        {
            return Mathf.Max(0f, hp - Mathf.Max(0f, damage));
        }

        /// <summary>
        /// Returns how far the enemy is pushed to the flash colour at a given time after a hit: the full strength at the
        /// hit, fading linearly to zero. Static and free of Unity state so it can be unit-tested.
        /// </summary>
        public static float ComputeHitFlash(float elapsed, float duration, float strength)
        {
            if (duration <= 0f || elapsed >= duration)
            {
                return 0f;
            }

            return strength * (1f - elapsed / duration);
        }
    }
}
