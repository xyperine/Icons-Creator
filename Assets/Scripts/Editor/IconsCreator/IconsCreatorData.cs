using UnityEditor;
using UnityEngine;

namespace IconsCreationTool
{
    public struct IconsCreatorData
    {
        public int Size { get; }
        public float Padding { get; }
        public string Name { get; }
        public TextureImporterCompression Compression { get; }
        public FilterMode FilterMode { get; }
        public IconBackgroundData BackgroundData { get; }
        public GameObject TargetObject { get; }


        public IconsCreatorData(int size, float padding, string name, TextureImporterCompression compression,
            FilterMode filterMode, IconBackgroundData backgroundData, GameObject targetObject)
        {
            Size = size;
            Padding = padding;
            Name = name;
            Compression = compression;
            FilterMode = filterMode;
            BackgroundData = backgroundData;
            TargetObject = targetObject;
        }
    }
}