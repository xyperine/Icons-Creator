using System.IO;
using UnityEditor;
using UnityEngine;

namespace IconsCreationTool
{
    public class IconsSaver
    {
        private const string DIRECTORY = "/Textures/Icons/";
        
        private string _name;
        private TextureImporterCompression _compression;
        private FilterMode _filterMode;
        

        public void SetData(string name, TextureImporterCompression compression, FilterMode filterMode)
        {
            _name = name;
            _compression = compression;
            _filterMode = filterMode;
        }


        public void SaveIcon(Texture2D image)
        {
            byte[] bytes = image.EncodeToPNG();
            Object.DestroyImmediate(image);

            string path = Application.dataPath + DIRECTORY + _name + ".png";
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
            textureImporter.textureCompression = _compression;
            textureImporter.filterMode = _filterMode;
            textureImporter.mipmapEnabled = false;

            textureImporter.SaveAndReimport();
            AssetDatabase.Refresh();

            EditorGUIUtility.PingObject(textureImporter);
        }
    }
}