using System;
using System.IO;
using IconsCreationTool.Editor.Utility.Helpers;
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
        private readonly string _relativeScenePath = $"Assets/Plugins/IconsCreator/Scenes/{SCENE_NAME}.unity";

        private Scene _prevActiveScene;
        
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


        public void InteractWithTarget(GameObject targetObject, bool renderShadows, Action<GameObject> action)
        {
            Scene scene = default;
            
            try
            {
                LayersHelper.CreateLayer(ICONS_CREATOR_TARGETS_LAYER_NAME);
                scene = OpenScene();
                
                GameObject target = PlaceTarget(targetObject);
                
                int layer = LayerMask.NameToLayer(ICONS_CREATOR_TARGETS_LAYER_NAME);
                target.layer = layer;
                foreach (Transform transform in target.GetComponentsInChildren<Transform>())
                {
                    if (transform.TryGetComponent(out MeshRenderer renderer))
                    {
                        renderer.shadowCastingMode = renderShadows ? 
                            ShadowCastingMode.On : ShadowCastingMode.Off;
                    }
                    transform.gameObject.layer = layer;
                }
                
                int cullingMask = LayerMask.GetMask(ICONS_CREATOR_TARGETS_LAYER_NAME);
                GameObject[] sceneRootGameObjects = scene.GetRootGameObjects();
                foreach (GameObject rootGameObject in sceneRootGameObjects)
                {
                    Light light = rootGameObject.GetComponentInChildren<Light>();
                    if (light)
                    {
                        light.cullingMask = cullingMask;
                    }
                    
                    Camera camera = rootGameObject.GetComponentInChildren<Camera>();
                    if (camera)
                    {
                        camera.cullingMask = cullingMask;
                    }
                }

                action?.Invoke(target);
            }
            finally
            {
                CloseScene(scene);
                LayersHelper.RemoveLayer(ICONS_CREATOR_TARGETS_LAYER_NAME);
            }
        }


        private Scene OpenScene()
        {
            _prevActiveScene = EditorSceneManager.GetActiveScene();
            Light[] allLightSources = Object.FindObjectsOfType<Light>();

            foreach (Light lightSource in allLightSources)
            {
                lightSource.cullingMask &= ~LayerMask.GetMask(ICONS_CREATOR_TARGETS_LAYER_NAME);
            }

            var openedScene = EditorSceneManager.OpenScene(_relativeScenePath, OpenSceneMode.Additive);
            EditorSceneManager.SetActiveScene(openedScene);

            return openedScene;
        }


        private GameObject PlaceTarget(GameObject targetObject)
        {
            if (EditorSceneManager.GetActiveScene().name != SCENE_NAME)
            {
                Debug.LogWarning("Something went wrong! Target object can only be placed in the internal scene!");
                return null;
            }

            GameObject target = Object.Instantiate(targetObject);
            return target;
        }


        private void CloseScene(Scene scene)
        {
            EditorSceneManager.SetActiveScene(_prevActiveScene);

            if (scene.IsValid())
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }
    }
}