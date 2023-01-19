using UnityEditor;
using UnityEngine;

namespace IconsCreationTool
{
    public struct IconsCreatorData
    {
        public IconsCreatorUserWorkflow UserWorkflow { get; }
        public int Size { get; }
        public float Padding { get; }
        public string Name { get; }
        public TextureImporterCompression Compression { get; }
        public FilterMode FilterMode { get; }
        public GameObject TargetObject { get; }


        public IconsCreatorData(IconsCreatorUserWorkflow userWorkflow, int size, float padding, string name, TextureImporterCompression compression,
            FilterMode filterMode, GameObject targetObject)
        {
            UserWorkflow = userWorkflow;
            Size = size;
            Padding = padding;
            Name = name;
            Compression = compression;
            FilterMode = filterMode;
            TargetObject = targetObject;
        }
    }
}