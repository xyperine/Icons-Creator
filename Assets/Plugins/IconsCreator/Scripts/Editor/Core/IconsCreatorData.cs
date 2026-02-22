using System.Collections.Generic;
using System.Linq;
using IconsCreationTool.Editor.Utility.Extensions;
using UnityEngine;

namespace IconsCreationTool.Editor.Core
{
    public struct IconsCreatorData
    {
        public int Size { get; }
        public float Padding { get; }
        public string Prefix { get; }
        public string Suffix { get; }
        public IconBackgroundData BackgroundData { get; }
        public Texture2D FrameTexture { get; }
        public GameObject[] Targets { get; }
        public bool RenderShadows { get; }


        public IconsCreatorData(int size, float padding, string prefix, string suffix,
            IconBackgroundData backgroundData, Texture2D frameTexture, List<Object> targets, bool renderShadows)
        {
            Size = size;
            Padding = padding;
            Prefix = prefix;
            Suffix = suffix;
            BackgroundData = backgroundData;
            FrameTexture = frameTexture;
            Targets = targets.ExtractAllGameObjects().Where(g => g.HasVisibleMesh()).ToArray();
            RenderShadows = renderShadows;
        }
    }
}