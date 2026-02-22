using UnityEngine;

namespace IconsCreationTool.Editor.Core
{
    internal class IconsCreatorSettingsProfile : ScriptableObject
    {
        [SerializeField] private IconBackground backgroundType;
        [SerializeField] private Color backgroundColor = Color.white;
        [SerializeField] private Texture2D backgroundTexture;
        
        [SerializeField] private Texture2D frameTexture;
        
        [SerializeField] private string prefix;
        [SerializeField] private string suffix = "_Icon";
        
        [SerializeField] private int size = 512;
        [SerializeField] private float padding;
        
        [SerializeField] private bool renderShadows;
        
        public IconBackground BackgroundType => backgroundType;
        public Color BackgroundColor => backgroundColor;
        public Texture2D BackgroundTexture => backgroundTexture;
        
        public Texture2D FrameTexture => frameTexture;

        public string Prefix => prefix;
        public string Suffix => suffix;

        public int Size => size;
        public float Padding => padding;

        public bool RenderShadows => renderShadows;


        public void SetValues(IconBackground backgroundType, Color backgroundColor, Texture2D backgroundTexture, Texture2D frameTexture, string prefix, string suffix, int size, float padding, bool renderShadows)
        {
            this.backgroundType = backgroundType;
            this.backgroundColor = backgroundColor;
            this.backgroundTexture = backgroundTexture;
            this.frameTexture = frameTexture;
            this.prefix = prefix;
            this.suffix = suffix;
            this.size = size;
            this.padding = padding;
            this.renderShadows = renderShadows;
        }
    }
}