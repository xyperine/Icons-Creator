using UnityEngine;

namespace IconsCreationTool.Utility.Extensions
{
    public static class TextureExtensions
    {
        public static Texture2D Resize(this Texture2D texture, int targetSize)
        {
            FilterMode filterMode = texture.filterMode;

            RenderTexture temporaryRenderTexture = RenderTexture.GetTemporary(targetSize, targetSize);
            RenderTexture.active = temporaryRenderTexture;
            
            Graphics.Blit(texture, temporaryRenderTexture);
            
            texture.Reinitialize(targetSize, targetSize, texture.graphicsFormat, false);
            texture.filterMode = filterMode;

            texture.ReadPixels(new Rect(0, 0, targetSize, targetSize), 0, 0);
            texture.Apply();

            RenderTexture.ReleaseTemporary(temporaryRenderTexture);

            return texture;
        }
    }
}