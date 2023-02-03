using UnityEditor;
using UnityEngine;

namespace IconsCreationTool.Editor.Utility.Helpers
{
    internal static class LayersHelper
    {
        /// <summary>
        /// Create a layer at the next available index. Returns silently if layer already exists.
        /// </summary>
        /// <param name="newLayerName">Name of the layer to create</param>
        public static void CreateLayer(string newLayerName)
        {
            if (string.IsNullOrEmpty(newLayerName))
            {
                throw new System.ArgumentNullException(nameof(newLayerName),
                    "New layer name string is either null or empty.");
            }

            const int builtInLayersCount = 5;

            SerializedObject tagManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProperty = tagManager.FindProperty("layers");
            int layersCount = layersProperty.arraySize;

            SerializedProperty firstEmptyLayerProperty = null;

            for (int i = 0; i < layersCount; i++)
            {
                SerializedProperty layerProperty = layersProperty.GetArrayElementAtIndex(i);

                string layerName = layerProperty.stringValue;

                if (layerName == newLayerName)
                {
                    return;
                }

                if (i < builtInLayersCount || layerName != string.Empty)
                {
                    continue;
                }

                firstEmptyLayerProperty ??= layerProperty;
            }

            if (firstEmptyLayerProperty == null)
            {
                Debug.LogError("Maximum limit of " + layersCount + " layers exceeded. Layer \"" + newLayerName + "\" not created.");
                return;
            }

            firstEmptyLayerProperty.stringValue = newLayerName;
            tagManager.ApplyModifiedProperties();
        }


        public static void RemoveLayer(string existingLayerName)
        {
            if (string.IsNullOrEmpty(existingLayerName))
            {
                throw new System.ArgumentNullException(nameof(existingLayerName),
                    "Layer name string is either null or empty.");
            }

            const int builtInLayersCount = 5;

            SerializedObject tagManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProperty = tagManager.FindProperty("layers");
            int layersCount = layersProperty.arraySize;

            SerializedProperty existingLayerProperty = null;

            for (int i = 0; i < layersCount; i++)
            {
                SerializedProperty layerProperty = layersProperty.GetArrayElementAtIndex(i);

                string layerName = layerProperty.stringValue;

                bool validLayer = i < builtInLayersCount || layerName != string.Empty;
                if (!validLayer)
                {
                    continue;
                }

                if (layerName != existingLayerName)
                {
                    continue;
                }
                
                existingLayerProperty ??= layerProperty;
            }

            if (existingLayerProperty == null)
            {
                Debug.LogError($"Layer named \"{existingLayerName}\" was not found!");
                return;
            }

            existingLayerProperty.stringValue = string.Empty;
            tagManager.ApplyModifiedProperties();
        }
    }
}