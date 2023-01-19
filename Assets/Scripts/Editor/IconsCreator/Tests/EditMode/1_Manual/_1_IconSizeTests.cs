using System;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IconsCreationTool.Tests._1_Manual
{
    public class _1_IconSizeTests
    {
        private const string MANUAL_SCENE_PATH = "Assets/Scenes/Icons Creation.unity";

        private const string DESIRED_NAME = "TestIcon";
        private const string FILE_EXTENSION = ".png";
        private const string PATH = "Assets/Textures/Icons/";
        private const TextureImporterCompression DESIRED_COMPRESSION = TextureImporterCompression.CompressedHQ;
        private const FilterMode DESIRED_FILTER_MODE = FilterMode.Point;

        private readonly IconsCreator _iconsCreator = new IconsCreator();

        private GameObject _target;

        private TextureImporter _textureImporter;


        [OneTimeSetUp]
        public void Initialize()
        {
            OpenManualScene();
            
            _target = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _target.transform.position = new Vector3(1024f, 0f, 1024f);
        }
        
        
        private void OpenManualScene()
        {
            Scene manualScene = EditorSceneManager.GetSceneByPath(MANUAL_SCENE_PATH);
            if (!manualScene.isLoaded)
            { 
                manualScene = EditorSceneManager.OpenScene(MANUAL_SCENE_PATH);
            }
            EditorSceneManager.SetActiveScene(manualScene);
        }
        

        private void SetResolution(int resolution)
        {
            IconsCreatorData data = new IconsCreatorData(IconsCreatorUserWorkflow.Manual, resolution, 0f, DESIRED_NAME,
                DESIRED_COMPRESSION, DESIRED_FILTER_MODE, _target);
            _iconsCreator.SetData(data);
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