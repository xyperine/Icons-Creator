using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace IconsCreationTool.Tests
{
    public class _0_IconAssetValidityTests
    {
        private const string DESIRED_NAME = "TestIcon";
        private const string DESIRED_PREFIX = "Test_";
        private const string DESIRED_SUFFIX = "_Icon";
        private const string FILE_EXTENSION = ".png";
        private const string PATH = "Assets/Textures/Icons/";
        private const TextureImporterCompression DESIRED_COMPRESSION = TextureImporterCompression.CompressedHQ;
        private const FilterMode DESIRED_FILTER_MODE = FilterMode.Point;

        private readonly IconsCreator _iconsCreator = new IconsCreator();

        private TextureImporter _textureImporter;


        [OneTimeSetUp]
        public void Initialize()
        {
            IconBackgroundData backgroundData = new IconBackgroundData(IconBackground.None, default, default);
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            target.name = DESIRED_NAME;
            target.transform.position = new Vector3(1024f, 0f, 1024f);
            List<Object> targets = new List<Object> {target};
            IconsCreatorData data = new IconsCreatorData(512, 0f, DESIRED_PREFIX, DESIRED_SUFFIX,
                DESIRED_COMPRESSION, DESIRED_FILTER_MODE, backgroundData, targets);
            _iconsCreator.SetData(data);

            _iconsCreator.CreateIcon();

            _textureImporter =
                (TextureImporter) AssetImporter.GetAtPath(PATH + DESIRED_PREFIX + DESIRED_NAME + DESIRED_SUFFIX +
                                                          FILE_EXTENSION);
        }


        [Test]
        public void Asset_Should_Exist_At_Given_Path()
        {
            Assert.IsNotNull(_textureImporter);
        }


        [Test]
        public void Asset_Should_Have_Expected_Name()
        {
            string name = _textureImporter.assetPath.Remove(0, PATH.Length);
            name = name.Remove(name.Length - FILE_EXTENSION.Length);

            Assert.AreEqual(DESIRED_PREFIX + DESIRED_NAME + DESIRED_SUFFIX, name);
        }


        [Test]
        public void Asset_Should_Be_A_Sprite()
        {
            TextureImporterType textureType = _textureImporter.textureType;
            
            Assert.AreEqual(TextureImporterType.Sprite, textureType);
        }
        
        
        [Test]
        public void Asset_Should_Have_Expected_Filter_Mode()
        {
            FilterMode textureFilterMode = _textureImporter.filterMode;
            
            Assert.AreEqual(DESIRED_FILTER_MODE, textureFilterMode);
        }


        [Test]
        public void Asset_Should_Have_Expected_Compression()
        {
            TextureImporterCompression textureCompression = _textureImporter.textureCompression;
            
            Assert.AreEqual(DESIRED_COMPRESSION, textureCompression);
        }
    }
}
