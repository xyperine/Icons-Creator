using System.Collections.Generic;
using IconsCreationTool.Utility.Extensions;
using UnityEngine;

namespace IconsCreationTool
{
    public struct IconsCreatorData
    {
        public int Size { get; }
        public float Padding { get; }
        public string Prefix { get; }
        public string Suffix { get; }
        public IconBackgroundData BackgroundData { get; }
        public List<GameObject> Targets { get; }


        public IconsCreatorData(int size, float padding, string prefix, string suffix,
            IconBackgroundData backgroundData, List<Object> targets)
        {
            Size = size;
            Padding = padding;
            Prefix = prefix;
            Suffix = suffix;
            BackgroundData = backgroundData;
            Targets = targets.ExtractAllGameObjects();
        }
    }
}