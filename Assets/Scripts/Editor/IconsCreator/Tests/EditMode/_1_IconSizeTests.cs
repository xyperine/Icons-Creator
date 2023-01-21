﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IconsCreationTool.Tests
{
    public class _1_IconSizeTests
    {
        private const string DESIRED_NAME = "TestIcon";
        private const string DESIRED_PREFIX = "Test_";
        private const string DESIRED_SUFFIX = "_Icon";
        private const string FILE_EXTENSION = ".png";
        private const string PATH = "Assets/Textures/Icons/";
        private const TextureImporterCompression DESIRED_COMPRESSION = TextureImporterCompression.CompressedHQ;
        private const FilterMode DESIRED_FILTER_MODE = FilterMode.Point;

        private readonly IconsCreator _iconsCreator = new IconsCreator();

        private List<Object> _targets;

        private TextureImporter _textureImporter;


        [OneTimeSetUp]
        public void Initialize()
        {
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            target.name = DESIRED_NAME;
            target.transform.position = new Vector3(1024f, 0f, 1024f);
            _targets= new List<Object> {target};
        }


        private void SetSize(int size)
        {
            IconBackgroundData backgroundData = new IconBackgroundData(IconBackground.None, default, default);
            IconsCreatorData data = new IconsCreatorData(size, 0f, DESIRED_PREFIX, DESIRED_SUFFIX,
                DESIRED_COMPRESSION, DESIRED_FILTER_MODE, backgroundData, _targets);
            _iconsCreator.SetData(data);
        }


        private void CreateIcon()
        {
            _iconsCreator.CreateIcon();

            _textureImporter =
                (TextureImporter) AssetImporter.GetAtPath(PATH + DESIRED_PREFIX + DESIRED_NAME + DESIRED_SUFFIX +
                                                          FILE_EXTENSION);
        }
        
        
        [Test]
        public void Asset_Should_Be_32px_Wide()
        {
            const int size = 32;
            
            SetSize(size);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(size, width);
        }
        
        
        [Test]
        public void Asset_Should_Be_512px_Wide()
        {
            const int size = 512;
            
            SetSize(size);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(size, width);
        }

        
        
        [Test]
        public void Asset_Should_Be_1024px_Wide()
        {
            const int size = 1024;
            
            SetSize(size);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(size, width);
        }
        
        
        [Test]
        public void Asset_Should_Be_381px_Wide()
        {
            const int size = 381;
            
            SetSize(size);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(size, width);
        }
        
        
        [Test]
        public void Asset_Should_Be_32px_High()
        {
            const int size = 1024;
            
            SetSize(size);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(size, height);
        }
        
        
        [Test]
        public void Asset_Should_Be_512px_High()
        {
            const int size = 512;
            
            SetSize(size);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(size, height);
        }

        
        [Test]
        public void Asset_Should_Be_1024px_High()
        {
            const int size = 1024;
            
            SetSize(size);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(size, height);
        }

        
        [Test]
        public void Asset_Should_Be_381px_High()
        {
            const int size = 1024;
            
            SetSize(size);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            Assert.AreEqual(size, height);
        }
        
        
        [Test]
        public void Asset_Should_Have_1to1_Aspect_Ratio()
        {
            SetSize(512);
            CreateIcon();
            
            _textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);
            float aspectRatio = (float) width / height;
            
            Assert.AreEqual(1, aspectRatio);
        }
        
        
        [Test]
        public void Passing_Less_Than_1px_Size_To_CameraUtility_Should_Throw_ArgumentOutOfRangeException()
        {
            const int size = -1;
            
            TestDelegate setCameraDataAction = () => SetSize(size);

            Assert.Throws<ArgumentOutOfRangeException>(setCameraDataAction);
        }
    }
}