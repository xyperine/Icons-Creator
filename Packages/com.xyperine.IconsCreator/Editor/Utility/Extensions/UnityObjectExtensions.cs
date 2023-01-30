using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IconsCreationTool.Editor.Utility.Extensions
{
    public static class UnityObjectExtensions
    {
        public static List<GameObject> ExtractAllGameObjects(this List<Object> objects)
        {
            List<GameObject> result = new List<GameObject>();
            GameObject[] gameObjects = objects.OfType<GameObject>().ToArray();
            result.AddRange(gameObjects);

            Object[] folders = objects.Except(gameObjects).Where(o => o.IsFolder()).ToArray();
            foreach (Object folder in folders)
            {
                if (!folder)
                {
                    continue;
                }
                
                string folderPath = AssetDatabase.GetAssetPath(folder)[7..];
                string[] filesPaths = Directory.GetFiles(Application.dataPath + "/" + folderPath, "*",
                    SearchOption.AllDirectories);
                foreach (string filePath in filesPaths)
                {
                    string relativeFilePath = filePath.Remove(0, Application.dataPath.Length - 6);
                    GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(relativeFilePath);
                    if (gameObject)
                    {
                        result.Add(gameObject);
                    }
                }
            }

            return result;
        }


        private static bool IsFolder(this Object obj)
        {
            return AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(obj));
        }


        public static bool IsFolderContainingGameObjects(this Object obj)
        {
            bool isFolder = obj.IsFolder();
            if (!isFolder)
            {
                return false;
            }
            
            bool containsGameObjects = false;

            string folderPath = AssetDatabase.GetAssetPath(obj)[7..];
            string[] filesPaths = Directory.GetFiles(Application.dataPath + "/" + folderPath, "*",
                SearchOption.AllDirectories);
            foreach (string filePath in filesPaths)
            {
                string relativeFilePath = filePath.Remove(0, Application.dataPath.Length - 6);
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(relativeFilePath);
                if (!gameObject)
                {
                    continue;
                }

                containsGameObjects = true;
                break;
            }

            return containsGameObjects;
        }
    }
}