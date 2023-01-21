using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IconsCreationTool
{
    public struct IconsCreatorData
    {
        public int Size { get; }
        public float Padding { get; }
        public string Prefix { get; }
        public string Suffix { get; }
        public TextureImporterCompression Compression { get; }
        public FilterMode FilterMode { get; }
        public IconBackgroundData BackgroundData { get; }
        public List<GameObject> Targets { get; }


        public IconsCreatorData(int size, float padding, string prefix, string suffix, TextureImporterCompression compression,
            FilterMode filterMode, IconBackgroundData backgroundData, List<Object> targets)
        {
            Size = size;
            Padding = padding;
            Prefix = prefix;
            Suffix = suffix;
            Compression = compression;
            FilterMode = filterMode;
            BackgroundData = backgroundData;
            Targets = ExtractAllGameObjects(targets);
        }


        private static List<GameObject> ExtractAllGameObjects(List<Object> objects)
        {
            List<GameObject> result = new List<GameObject>();
            GameObject[] gameObjects = objects.OfType<GameObject>().ToArray();
            result.AddRange(gameObjects);

            objects = objects.Except(gameObjects).ToList();
            foreach (Object obj in objects)
            {
                if (!obj)
                {
                    continue;
                }
                
                string objPath = AssetDatabase.GetAssetPath(obj)[7..];
                string[] objs = Directory.GetFiles(Application.dataPath + "/" + objPath, "*", SearchOption.AllDirectories);
                foreach (string p in objs)
                {
                    string p1 = p.Remove(0, Application.dataPath.Length - 6);
                    GameObject g = AssetDatabase.LoadAssetAtPath<GameObject>(p1);
                    if (g)
                    {
                        result.Add(g);
                    }
                }
            }

            return result;
        }
    }
}