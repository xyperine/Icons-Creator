using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace IconsCreationTool.Tests._0_Auto
{
    public class _0_IconAssetValidityTests
    {
        private const string DESIRED_NAME = "TestIcon";
        private const string FILE_EXTENSION = ".png";
        private const string PATH = "Assets/Textures/Icons/";
        private const TextureImporterCompression DESIRED_COMPRESSION = TextureImporterCompression.CompressedHQ;
        private const FilterMode DESIRED_FILTER_MODE = FilterMode.Point;

        private readonly IconsCreator _iconsCreator = new IconsCreator();

        private TextureImporter _textureImporter;


        [OneTimeSetUp]
        public void Initialize()
        {
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            target.transform.position = new Vector3(1024f, 0f, 1024f);
            IconsCreatorData data = new IconsCreatorData(IconsCreatorUserWorkflow.Auto, 512, 0f, DESIRED_NAME,
                DESIRED_COMPRESSION, DESIRED_FILTER_MODE, target);
            _iconsCreator.SetData(data);

            _iconsCreator.CreateIcon();
            
            _textureImporter = (TextureImporter) AssetImporter.GetAtPath(PATH + DESIRED_NAME + FILE_EXTENSION);
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

            Assert.AreEqual(DESIRED_NAME, name);
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
