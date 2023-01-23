using System.Collections.Generic;
using IconsCreationTool.Utility.Extensions;
using UnityEditor;
using UnityEngine;

namespace IconsCreationTool
{
    public struct IconsCreatorData
    {
        public int Size { get; }
        public float Padding { get; }
        public string Prefix { get; }
        public string Suffix { get; }
        public TextureImporterCompression Compression { get; }
        public IconBackgroundData BackgroundData { get; }
        public List<GameObject> Targets { get; }


        public IconsCreatorData(int size, float padding, string prefix, string suffix,
            TextureImporterCompression compression, IconBackgroundData backgroundData, List<Object> targets)
        {
            Size = size;
            Padding = padding;
            Prefix = prefix;
            Suffix = suffix;
            Compression = compression;
            BackgroundData = backgroundData;
            Targets = targets.ExtractAllGameObjects();
        }
    }
}