﻿using System;
using IconsCreationTool.Utility.Extensions;
using UnityEngine;

namespace IconsCreationTool
{
    public class IconsCreatorCameraUtility
    {
        private const string ICONS_CREATION_CAMERA_TAG = "IconsCreationCamera";

        private Camera _camera;
        private GameObject _targetObject;
        private Bounds _targetOrthographicBounds;

        private int _resolution;
        private float _padding;
        
        private float _distanceToTarget = 10f;

        private Vector3 CameraOffset => -_camera.transform.forward * _distanceToTarget;
        
        public bool Orthographic => _camera.orthographic;
        public string IconsCreationCameraTag => ICONS_CREATION_CAMERA_TAG;


        public void RetrieveCamera()
        {
            if (_camera)
            {
                if (_camera.gameObject.CompareTag(ICONS_CREATION_CAMERA_TAG)) 
                {
                    return;
                }
            }
            
            GameObject cameraObject = GameObject.FindGameObjectWithTag(ICONS_CREATION_CAMERA_TAG);
            if (!cameraObject)
            {
                Debug.LogWarning($"No game object tagged \"{ICONS_CREATION_CAMERA_TAG}\" found!");
                return;
            }
            
            _camera = cameraObject.GetComponent<Camera>();

            if (!_camera)
            {
                Debug.LogWarning($"No camera found! Create camera object and tag it \"{ICONS_CREATION_CAMERA_TAG}\"");
            }

            Debug.Log("Camera found!");
        }


        public void SetData(GameObject targetObject, int resolution, float padding)
        {
            if (resolution < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(resolution));
            }
            
            _targetObject = targetObject;
            _resolution = resolution;
            _padding = padding;
        }


        public void AdjustCamera()
        {
            if (!_targetObject)
            {
                Debug.LogWarning("No target found!");
                return;
            }

            _targetOrthographicBounds = _targetObject.GetOrthographicBounds(_camera);

            SetRotation();
            SetPosition();
            SetOrthographicSize();
        }


        private void SetRotation()
        {
            _camera.transform.rotation = Quaternion.AngleAxis(45, Vector3.right);
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
            if (_resolution < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(_resolution));
            }
            
            _camera.targetTexture = RenderTexture.GetTemporary(_resolution, _resolution);
            RenderTexture.active = _camera.targetTexture;

            _camera.Render();

            Texture2D image = new Texture2D(_resolution, _resolution);
            image.ReadPixels(new Rect(0, 0, _resolution, _resolution), 0, 0);
            image.Apply();

            _camera.targetTexture = null;
            RenderTexture.active = null;

            return image;
        }


        public IconsCreatorCameraUtilityDebugData GetDebugData()
        {
            bool ready = _targetObject && _camera;

            if (!ready)
            {
                return default;
            }

            return new IconsCreatorCameraUtilityDebugData(true, _camera.transform.position, _targetOrthographicBounds);
        }
    }
}