﻿using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace IconsCreationTool
{
    public class IconsCreator
    {
        private IconsCreatorData _data;

        private readonly IconsCreatorInternalSceneHandler _sceneHandler;
        private readonly IconsCreatorCameraUtility _cameraUtility;
        private readonly IconsSaver _iconsSaver;

        public Texture2D CameraView { get; private set; }


        public IconsCreator()
        {
            _sceneHandler = new IconsCreatorInternalSceneHandler();
            _cameraUtility = new IconsCreatorCameraUtility();
            _iconsSaver = new IconsSaver();
        }


        public void InitializeEnvironment()
        {
            AddIconsCreationCameraTag();
            _sceneHandler.CreateSceneIfItDoesNotExist(_cameraUtility.IconsCreationCameraTag);
        }
        
        
        private void AddIconsCreationCameraTag()
        {
            if (!InternalEditorUtility.tags.Contains(_cameraUtility.IconsCreationCameraTag))
            {
                InternalEditorUtility.AddTag(_cameraUtility.IconsCreationCameraTag);
            }
        }


        public void SetData(IconsCreatorData data)
        {
            _data = data;

            _cameraUtility.SetData(_data.TargetObject, _data.Resolution, _data.Padding);
            _iconsSaver.SetData(_data.Name, _data.Compression, _data.FilterMode);

            Debug.Log("Data passed");
            
            OnDataChanged();
        }


        private void OnDataChanged()
        {
            if (!_data.TargetObject)
            {
                return;
            }
            
            UpdateCameraView();
            _iconsSaver.SetData(_data.Name, _data.Compression, _data.FilterMode);
        }


        private void UpdateCameraView()
        {
            if (!_data.TargetObject)
            {
                return;
            }

            if (_data.UserWorkflow == IconsCreatorUserWorkflow.Manual)
            {
                AdjustCamera(_data.TargetObject);
                return;
            }

            _sceneHandler.InteractWithTarget(_data.TargetObject, AdjustCamera);
        }


        private void AdjustCamera(GameObject target)
        {
            _cameraUtility.SetData(target, _data.Resolution, _data.Padding);
            _cameraUtility.RetrieveCamera();
            _cameraUtility.AdjustCamera();
            _cameraUtility.AdjustCamera();
            
            CameraView = _cameraUtility.CaptureCameraView();
            CameraView.filterMode = _data.FilterMode;
        }


        public void CreateIcon()
        {
            if (!_data.TargetObject)
            {
                return;
            }

            if (_data.UserWorkflow == IconsCreatorUserWorkflow.Manual)
            {
                CreateIconFromSceneObject();
                return;
            }

            CreateIconFromPrefab();
        }


        private void CreateIconFromSceneObject()
        {
            AdjustCamera(_data.TargetObject);
            
            Texture2D icon = _cameraUtility.CaptureCameraView();
            _iconsSaver.SaveIcon(icon);
        }


        private void CreateIconFromPrefab()
        {
            _sceneHandler.InteractWithTarget(_data.TargetObject, target =>
                {
                    AdjustCamera(target);
            
                    Texture2D icon = _cameraUtility.CaptureCameraView();
                    _iconsSaver.SaveIcon(icon);
                });
        }
    }
}