using System;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace IconsCreationTool.Editor.Core
{
    public class IconsCreatorInternalSceneHandler
    {
        private const string ICONS_CREATOR_TARGETS_LAYER_NAME = "IconsCreatorTargets";
        private const string SCENE_NAME = "Icons_Creation";
        private readonly string _relativeScenePath = $"Packages/com.xyperine.icons_creator/Scenes/{SCENE_NAME}.unity";

        private Scene _openedScene;
        private Light[] _allLightSources;

        private string _iconsCreationCameraTag;


        public void TryCreateScene(string iconsCreationCameraTag)
        {
            string path = Path.GetFullPath(_relativeScenePath);
            if (File.Exists(path))
            {
                return;
            }
            
            _iconsCreationCameraTag = iconsCreationCameraTag;
            
            CreateScene();
        }


        private void CreateScene()
        {
            Scene prevActiveScene = EditorSceneManager.GetActiveScene();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
            scene.name = SCENE_NAME;
            ulong targetLayer = (ulong) LayerMask.GetMask(ICONS_CREATOR_TARGETS_LAYER_NAME);
            EditorSceneManager.SetSceneCullingMask(scene, targetLayer);

            SetupSceneComponents(scene);

            SetupSceneRendering();

            EditorSceneManager.SaveScene(scene, _relativeScenePath);

            EditorSceneManager.CloseScene(scene, true);
            EditorSceneManager.SetActiveScene(prevActiveScene);

            Debug.Log($"Scene {SCENE_NAME} was created!");
        }


        private void SetupSceneComponents(Scene scene)
        {
            Camera camera = null;
            Light light = null;

            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                camera ??= rootGameObject.GetComponentInChildren<Camera>();
                if (camera)
                {
                    camera.gameObject.tag = _iconsCreationCameraTag;
                    camera.clearFlags = CameraClearFlags.SolidColor;
                    camera.backgroundColor = Color.clear;
                    camera.orthographic = true;
                    camera.cullingMask = LayerMask.GetMask(ICONS_CREATOR_TARGETS_LAYER_NAME);
                }

                light ??= rootGameObject.GetComponentInChildren<Light>();
                if (light)
                {
                    light.useColorTemperature = false;
                    light.color = Color.white;
                }

                if (camera && light)
                {
                    return;
                }
            }
        }


        private void SetupSceneRendering()
        {
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientSkyColor = new Color(0.73f, 0.73f, 0.73f);
        }


        public void InteractWithTarget(GameObject targetObject, Action<GameObject> action)
        {
            Scene prevActiveScene = OpenScene();
            
            try
            {
                GameObject target = PlaceTarget(targetObject);
                int layer = LayerMask.NameToLayer(ICONS_CREATOR_TARGETS_LAYER_NAME);
                target.layer = layer;
                foreach (Transform transform in target.GetComponentsInChildren<Transform>())
                {
                    transform.gameObject.layer = layer;
                }

                action?.Invoke(target);
            }
            finally
            {
                CloseScene(prevActiveScene);
            }
        }


        private Scene OpenScene()
        {
            Scene prevActiveScene = EditorSceneManager.GetActiveScene();
            _allLightSources = Object.FindObjectsOfType<Light>();

            foreach (Light lightSource in _allLightSources)
            {
                lightSource.enabled = false;
            }

            _openedScene = EditorSceneManager.OpenScene(_relativeScenePath, OpenSceneMode.Additive);
            EditorSceneManager.SetActiveScene(_openedScene);

            return prevActiveScene;
        }


        private GameObject PlaceTarget(GameObject targetObject)
        {
            if (EditorSceneManager.GetActiveScene().name != SCENE_NAME)
            {
                Debug.LogWarning("Can place target only in the internal scene!");
                return null;
            }

            GameObject target = Object.Instantiate(targetObject);
            return target;
        }


        private void CloseScene(Scene prevActiveScene)
        {
            EditorSceneManager.SetActiveScene(prevActiveScene);

            if (_openedScene.IsValid())
            {
                EditorSceneManager.CloseScene(_openedScene, true);
            }

            foreach (Light lightSource in _allLightSources)
            {
                if (!lightSource)
                {
                    continue;
                }

                lightSource.enabled = true;
            }
        }
    }
}