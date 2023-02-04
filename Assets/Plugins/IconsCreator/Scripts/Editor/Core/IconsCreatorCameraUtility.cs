using System;
using IconsCreationTool.Editor.Utility.Extensions;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IconsCreationTool.Editor.Core
{
    public class IconsCreatorCameraUtility
    {
        private const string ICONS_CREATION_CAMERA_TAG = "IconsCreationCamera";

        private Camera _camera;
        private GameObject _targetObject;
        private Bounds _targetOrthographicBounds;

        private int _size;
        private float _padding;

        private IconBackgroundData _backgroundData;

        private float _distanceToTarget = 10f;

        private Vector3 CameraOffset => -_camera.transform.forward * _distanceToTarget;
        public string IconsCreationCameraTag => ICONS_CREATION_CAMERA_TAG;


        public void RetrieveCamera()
        {
            Scene activeScene = EditorSceneManager.GetActiveScene();

            if (_camera)
            {
                bool isInValidScene = _camera.scene == activeScene;
                bool isTagged = _camera.gameObject.CompareTag(ICONS_CREATION_CAMERA_TAG);

                if (isInValidScene && isTagged)
                {
                    return;
                }
            }

            foreach (GameObject rootGameObject in activeScene.GetRootGameObjects())
            {
                Camera camera = rootGameObject.GetComponentInChildren<Camera>();
                if (!camera)
                {
                    continue;
                }

                if (camera.CompareTag(ICONS_CREATION_CAMERA_TAG))
                {
                    _camera = camera;
                }
            }

            if (!_camera)
            {
                Debug.LogWarning($"Something went wrong! No camera tagged \"{ICONS_CREATION_CAMERA_TAG}\" was found!");
            }
        }


        public void SetData(GameObject targetObject, int size, float padding)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            _targetObject = targetObject;
            _size = size;
            _padding = padding;
        }


        public void SetBackground(IconBackgroundData backgroundData)
        {
            _backgroundData = backgroundData;
            
            switch (_backgroundData.Type)
            {
                case IconBackground.None:
                    _camera.clearFlags = CameraClearFlags.SolidColor;
                    _camera.backgroundColor = Color.clear;
                    break;
                
                case IconBackground.Color:
                    _camera.clearFlags = CameraClearFlags.SolidColor;
                    _camera.backgroundColor = _backgroundData.Color;
                    break;
                
                case IconBackground.Texture:
                    _camera.clearFlags = CameraClearFlags.Nothing;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public void AdjustCamera()
        {
            if (!_targetObject)
            {
                Debug.LogWarning("No target object found!");
                return;
            }

            _targetOrthographicBounds = _targetObject.GetOrthographicBounds(_camera);

            SetRotation();
            SetPosition();
            SetOrthographicSize();
        }


        private void SetRotation()
        {
            _camera.transform.rotation = Quaternion.Euler(45f, -45f, 0f);
        }
        

        private void SetPosition()
        {
            _distanceToTarget = _targetOrthographicBounds.size.z / 2 + 10;
            Vector3 targetCenter = _targetOrthographicBounds.center;
            _camera.transform.position = targetCenter + CameraOffset;
        }


        private void SetOrthographicSize()
        {
            Vector2 minVertexPositionOnCameraPlane = _camera.transform.InverseTransformPoint(_targetOrthographicBounds.min);
            Vector2 maxVertexPositionOnCameraPlane = _camera.transform.InverseTransformPoint(_targetOrthographicBounds.max);
            Vector2 distance = maxVertexPositionOnCameraPlane - minVertexPositionOnCameraPlane;

            _camera.orthographicSize = distance.Abs().BiggestComponentValue() * 0.5f / (1 - _padding);
        }


        public Texture2D CaptureCameraView()
        {
            if (_size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(_size));
            }

            RenderTexture temporaryRenderTexture = RenderTexture.GetTemporary(_size, _size);

            if (_backgroundData.Type == IconBackground.Texture)
            {
                Texture2D backgroundTexture = _backgroundData.Texture;
                if (backgroundTexture)
                {
                    Graphics.Blit(backgroundTexture, temporaryRenderTexture);
                }
            }

            _camera.targetTexture = temporaryRenderTexture;
            RenderTexture.active = _camera.targetTexture;

            _camera.Render();

            Texture2D image = new Texture2D(_size, _size);
            image.ReadPixels(new Rect(0, 0, _size, _size), 0, 0);
            image.Apply();
            
            _camera.targetTexture = null;
            RenderTexture.active = null;
            
            RenderTexture.ReleaseTemporary(temporaryRenderTexture);

            return image;
        }
    }
}