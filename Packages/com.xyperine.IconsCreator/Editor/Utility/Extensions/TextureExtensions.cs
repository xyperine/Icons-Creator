using UnityEngine;

namespace IconsCreationTool.Editor.Utility.Extensions
{
    public static class TextureExtensions
    {
        public static Texture2D Resize(this Texture2D texture, int targetSize)
        {
            FilterMode filterMode = texture.filterMode;

            RenderTexture temporaryRenderTexture = RenderTexture.GetTemporary(targetSize, targetSize);
            RenderTexture.active = temporaryRenderTexture;
            
            Graphics.Blit(texture, temporaryRenderTexture);

            Texture2D resizedTexture = new Texture2D(targetSize, targetSize);
            resizedTexture.filterMode = filterMode;

            resizedTexture.ReadPixels(new Rect(0, 0, targetSize, targetSize), 0, 0);
            resizedTexture.Apply();

            RenderTexture.ReleaseTemporary(temporaryRenderTexture);

            return resizedTexture;
        }
    }
}