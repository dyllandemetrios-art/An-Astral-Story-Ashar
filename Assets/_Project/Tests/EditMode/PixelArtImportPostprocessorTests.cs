using Ashar.Editor;
using NUnit.Framework;

namespace Ashar.Tests.EditMode
{
    /// <summary>
    /// Checks which folders receive the pixel-art import settings.
    /// WHY: the folder filter is the only pure logic of the postprocessor, so it is tested
    /// directly instead of importing real textures (slow and dependent on the project state).
    /// </summary>
    public class PixelArtImportPostprocessorTests
    {
        /// <summary>Textures in the project's own folders or in the purchased packs are pixel art.</summary>
        [TestCase("Assets/_Project/Features/Player/Art/Placeholder/Ship.png")]
        [TestCase("Assets/ThirdParty/DyLESTorm/Ships/Ship01.png")]
        [TestCase("Assets\\ThirdParty\\DyLESTorm\\Ship01.png")]
        public void IsPixelArtPath_ProjectOrThirdPartyTexture_ReturnsTrue(string assetPath)
        {
            Assert.IsTrue(PixelArtImportPostprocessor.IsPixelArtPath(assetPath));
        }

        /// <summary>Anything outside those folders (plugins, packages, look-alike names, empty paths) is left alone.</summary>
        [TestCase("Assets/Plugins/Some/Icon.png")]
        [TestCase("Assets/_ProjectBackup/Ship.png")]
        [TestCase("Packages/com.unity.2d.sprite/Icon.png")]
        [TestCase("")]
        [TestCase(null)]
        public void IsPixelArtPath_OtherLocation_ReturnsFalse(string assetPath)
        {
            Assert.IsFalse(PixelArtImportPostprocessor.IsPixelArtPath(assetPath));
        }
    }
}
