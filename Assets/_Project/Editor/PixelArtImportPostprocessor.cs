using UnityEditor;
using UnityEngine;

namespace Ashar.Editor
{
    /// <summary>
    /// Gives every new texture in the project's own folders the pixel-art import settings (§3):
    /// Sprite (Single mode), Point filtering, no compression, no mip maps, 48 pixels per unit.
    /// HOW IT WORKS: Unity calls OnPreprocessTexture() on every AssetPostprocessor before a texture
    /// is imported; we change the importer settings there, so the texture is imported correctly first time.
    /// WHY: a forgotten manual setting (bilinear filtering, compression) is the usual cause of blurry pixel art.
    /// </summary>
    public class PixelArtImportPostprocessor : AssetPostprocessor
    {
        public const float PixelsPerUnit = 48f; // Engine scale from the spec (§4): 1 unit = 48 reference pixels.

        private static readonly string[] PixelArtRoots = // Pixel-art folders; packages and plugins are left untouched.
        {
            "Assets/_Project/",
            "Assets/ThirdParty/",
        };

        /// <summary>
        /// True when the asset lives in a folder that should receive the pixel-art settings.
        /// Kept public and pure so it can be unit-tested without importing a real texture.
        /// </summary>
        public static bool IsPixelArtPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return false;
            }

            // Unity paths use forward slashes, but normalize in case a Windows path slips in.
            string normalizedPath = assetPath.Replace('\\', '/');
            foreach (string root in PixelArtRoots)
            {
                if (normalizedPath.StartsWith(root, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Applies the pixel-art settings to a texture imported for the first time in a pixel-art folder.</summary>
        private void OnPreprocessTexture()
        {
            if (!IsPixelArtPath(assetPath))
            {
                return;
            }

            // Only on the very first import (no .meta yet): settings tweaked by hand
            // in the Inspector afterwards must never be overwritten by a reimport.
            if (!assetImporter.importSettingsMissing)
            {
                return;
            }

            var textureImporter = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.Sprite;

            // Unity 6.6 slices a new sprite texture into several trimmed sprites by default. Single keeps one sprite
            // per image with its whole cell, so the pivot stays at the cell centre and animation frames never jitter.
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            textureImporter.spritePixelsPerUnit = PixelsPerUnit;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.mipmapEnabled = false;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        }
    }
}
