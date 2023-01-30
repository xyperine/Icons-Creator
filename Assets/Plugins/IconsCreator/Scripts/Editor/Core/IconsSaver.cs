using System.IO;
using UnityEditor;
using UnityEngine;

namespace IconsCreationTool.Editor.Core
{
    public class IconsSaver
    {
        private const string DIRECTORY = "/Textures/Icons/";
        
        private string _prefix;
        private string _suffix;


        public void SetData(string prefix, string suffix)
        {
            _prefix = prefix;
            _suffix = suffix;
        }


        public void SaveIcon(Texture2D image, string name)
        {
            byte[] bytes = image.EncodeToPNG();
            Object.DestroyImmediate(image);

            string path = Application.dataPath + DIRECTORY + _prefix + name + _suffix + ".png";
            if (!Directory.Exists(Application.dataPath + DIRECTORY))
            {
                Directory.CreateDirectory(Application.dataPath + DIRECTORY);
            }

            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();

            ConvertToSprite(path);
        }


        private void ConvertToSprite(string path)
        {
            string relativePath = path.Remove(0, Application.dataPath.Length - "Assets".Length);

            TextureImporter textureImporter = (TextureImporter) AssetImporter.GetAtPath(relativePath);
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.mipmapEnabled = false;

            textureImporter.SaveAndReimport();
            AssetDatabase.Refresh();

            EditorGUIUtility.PingObject(textureImporter);
        }
    }
}