using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace IconsCreationTool.Tests
{
    public class IconSizeTests
    {
        private const string DESIRED_NAME = "TestIcon";
        private const string FILE_EXTENSION = ".png";
        private const string PATH = "Assets/Textures/Icons/";
        private const TextureImporterCompression DESIRED_COMPRESSION = TextureImporterCompression.CompressedHQ;
        private const FilterMode DESIRED_FILTER_MODE = FilterMode.Point;

        private readonly IconsCreatorCameraUtility _cameraUtility = new IconsCreatorCameraUtility();
        private IconsCreator _iconsCreator;

        private GameObject _target;

        private TextureImporter _textureImporter;


        [OneTimeSetUp]
        public void Initialize()
        {
            CreateTarget();
            
            SetupCamera();
            SetupIconsCreator();
        }


        private void CreateTarget()
        {
            _target = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _target.transform.position = new Vector3(1024f, 0f, 1024f);
        }
        
        
        private void SetupCamera()
        {
            _cameraUtility.RetrieveCamera();
            
            SetResolution(512);
        }


        private void SetResolution(int resolution)
        {
            _cameraUtility.SetData(_target, resolution, 0f);
            _cameraUtility.AdjustCamera();
        }


        private void SetupIconsCreator()
        {
            _iconsCreator = new IconsCreator(_cameraUtility);
            _iconsCreator.SetData(DESIRED_NAME, DESIRED_COMPRESSION, DESIRED_FILTER_MODE);
        }


        private void CreateIcon()
        {
            _iconsCreator.CreateIcon();
            
            _textureImporter = (TextureImporter) AssetImporter.GetAtPath(PATH + DESIRED_NAME + FILE_EXTENSION);
        }
        
        
        [Test]
        public void Asset_Should_Be_32px_Wide()
        {
            const int resolution = 32;
            
            SetResolution(resolution);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(resolution, width);
        }
        
        
        [Test]
        public void Asset_Should_Be_512px_Wide()
        {
            const int resolution = 512;
            
            SetResolution(resolution);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(resolution, width);
        }

        
        
        [Test]
        public void Asset_Should_Be_1024px_Wide()
        {
            const int resolution = 1024;
            
            SetResolution(resolution);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(resolution, width);
        }
        
        
        [Test]
        public void Asset_Should_Be_381px_Wide()
        {
            const int resolution = 381;
            
            SetResolution(resolution);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(resolution, width);
        }
        
        
        [Test]
        public void Asset_Should_Be_32px_High()
        {
            const int resolution = 1024;
            
            SetResolution(resolution);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(resolution, height);
        }
        
        
        [Test]
        public void Asset_Should_Be_512px_High()
        {
            const int resolution = 512;
            
            SetResolution(resolution);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(resolution, height);
        }

        
        [Test]
        public void Asset_Should_Be_1024px_High()
        {
            const int resolution = 1024;
            
            SetResolution(resolution);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(resolution, height);
        }

        
        [Test]
        public void Asset_Should_Be_381px_High()
        {
            const int resolution = 1024;
            
            SetResolution(resolution);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(resolution, height);
        }
        
        
        [Test]
        public void Asset_Should_Have_1to1_Aspect_Ratio()
        {
            SetResolution(512);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);
            float aspectRatio = (float) width / height;
            
            Assert.AreEqual(1, aspectRatio);
        }
        
        
        [Test]
        public void Passing_Less_Than_1px_Resolution_To_CameraUtility_Should_Throw_ArgumentOutOfRangeException()
        {
            const int resolution = -1;
            
            TestDelegate setCameraDataAction = () => SetResolution(resolution);

            Assert.Throws<ArgumentOutOfRangeException>(setCameraDataAction);
        }
    }
}