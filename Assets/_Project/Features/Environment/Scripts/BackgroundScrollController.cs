using System;
using UnityEngine;

namespace Ashar.Environment
{
    /// <summary>
    /// Scrolls a stack of background layers downwards at different speeds to fake depth (parallax).
    /// RESPONSIBILITIES: move each layer, and wrap it around so the scroll never ends and shows no seam.
    /// HOW IT WORKS: each layer is a root object holding two identical tiles, tile B exactly one tile height
    /// above tile A. The root slides down; once it has moved one tile height, it jumps back to its start.
    /// Because both tiles look the same, that jump is invisible. Each tile must be taller than the screen, so
    /// the two of them always cover it. A layer's speed is the base speed times its
    /// speed factor: distant layers get a small factor so they move slower than the near ones.
    /// WHY: the scenery scrolls, not the camera (spec §4), and the base speed is a plain Inspector value so
    /// the wave timeline can change it later.
    /// </summary>
    public class BackgroundScrollController : MonoBehaviour
    {
        /// <summary>Settings of one scrolling layer: what moves, what it is made of, and how fast.</summary>
        [Serializable]
        public class ScrollLayer
        {
            [SerializeField, Tooltip("Object that is moved by the scroll. Holds the two tiles as children.")]
            private Transform _root;

            [SerializeField, Tooltip("Renderer of one tile. Its drawn height (also for a Tiled sprite) is the distance after which the layer wraps around, so it must be a whole number of pattern repeats.")]
            private SpriteRenderer _tile;

            [SerializeField, Min(0f), Tooltip("Speed of this layer as a fraction of the base speed. Smaller for distant layers.")]
            private float _speedFactor = 1f;

            /// <summary>Object moved by the scroll.</summary>
            public Transform Root => _root;

            /// <summary>Renderer of one tile of the layer.</summary>
            public SpriteRenderer Tile => _tile;

            /// <summary>Speed of the layer as a fraction of the base speed.</summary>
            public float SpeedFactor => _speedFactor;
        }

        [Header("Scroll")]
        [SerializeField, Min(0f), Tooltip("Scroll speed of the nearest layer, in world units per second. Change it while playing to see the effect.")]
        private float _baseSpeed = 4f;

        [SerializeField, Tooltip("Layers from the farthest to the nearest. Each one needs a root, a tile renderer and a speed factor.")]
        private ScrollLayer[] _layers;

        [Header("Debug")]
        [SerializeField, Tooltip("Read-only: distance scrolled so far by the first layer, in world units (wraps at one tile height).")]
        private float _firstLayerOffset;

        private float[] _offsets;            // Distance already scrolled by each layer, kept between 0 and its tile height.
        private float[] _tileHeights;        // World height of one tile of each layer, read once at start.
        private Vector3[] _startPositions;   // Local position of each layer root when the scene starts.

        /// <summary>Reads the tile heights and start positions once, so Update() needs no lookups.</summary>
        private void Awake()
        {
            int count = _layers != null ? _layers.Length : 0;
            _offsets = new float[count];
            _tileHeights = new float[count];
            _startPositions = new Vector3[count];

            for (int i = 0; i < count; i++)
            {
                ScrollLayer layer = _layers[i];
                if (layer == null || layer.Root == null || layer.Tile == null || layer.Tile.sprite == null)
                {
                    Debug.LogError($"{nameof(BackgroundScrollController)}: layer {i} is missing its root, tile or sprite. Scroll disabled.", this);
                    enabled = false;
                    return;
                }

                // The renderer bounds give the height actually drawn in the world. Unlike the sprite size, they are
                // right for a Tiled sprite too, where one renderer repeats a small pattern several times.
                _tileHeights[i] = layer.Tile.bounds.size.y;
                _startPositions[i] = layer.Root.localPosition;
            }
        }

        /// <summary>Moves every layer down by its own speed and wraps it when it has travelled one tile.</summary>
        private void Update()
        {
            for (int i = 0; i < _layers.Length; i++)
            {
                float speed = _baseSpeed * _layers[i].SpeedFactor;
                _offsets[i] = AdvanceOffset(_offsets[i], speed, Time.deltaTime, _tileHeights[i]);

                Vector3 position = _startPositions[i];
                position.y -= _offsets[i];
                _layers[i].Root.localPosition = position;
            }

            if (_offsets.Length > 0)
            {
                _firstLayerOffset = _offsets[0];
            }
        }

        /// <summary>
        /// Adds the distance travelled this frame to a layer offset and wraps it into [0, tileHeight).
        /// Static and free of Unity state so it can be unit-tested. A tile height of 0 or less gives 0.
        /// </summary>
        public static float AdvanceOffset(float offset, float speed, float deltaTime, float tileHeight)
        {
            if (tileHeight <= 0f)
            {
                return 0f;
            }

            // Mathf.Repeat is a modulo that also works for negative values, and it keeps the leftover distance
            // when a frame is long, so the scroll speed stays exact whatever the frame rate.
            return Mathf.Repeat(offset + speed * deltaTime, tileHeight);
        }
    }
}
