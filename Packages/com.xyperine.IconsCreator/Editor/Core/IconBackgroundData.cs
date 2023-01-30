using UnityEngine;

namespace IconsCreationTool.Editor.Core
{
    public struct IconBackgroundData
    {
        public IconBackground Type { get; }
        
        public Color Color { get; }
        public Texture2D Texture { get; }

        public IconBackgroundData(IconBackground type, Color color, Texture2D texture)
        {
            Type = type;
            Color = color;
            Texture = texture;
        }
    }
}