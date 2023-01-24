﻿using System.Linq;
using IconsCreationTool.Utility.Helpers;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace IconsCreationTool
{
    public class IconsCreator
    {
        private IconsCreatorData _data;

        private readonly IconsCreatorInternalSceneHandler _sceneHandler;
        private readonly IconsCreatorCameraUtility _cameraUtility;
        private readonly IconsSaver _iconsSaver;

        private bool AnyTargets => _data.Targets.Any(t => t);
        
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
            LayersHelper.CreateLayer("IconsCreatorTargets");
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

            _cameraUtility.SetData(_data.Targets.FirstOrDefault(), _data.Size, _data.Padding);
            _iconsSaver.SetData(_data.Prefix, _data.Suffix);

            Debug.Log("Data passed");
            
            OnDataChanged();
        }


        private void OnDataChanged()
        {
            if (!AnyTargets)
            {
                return;
            }
            
            UpdateCameraView();
        }


        private void UpdateCameraView()
        {
            if (!AnyTargets)
            {
                return;
            }

            _sceneHandler.InteractWithTarget(_data.Targets[0], AdjustCamera);
        }


        private void AdjustCamera(GameObject target)
        {
            _cameraUtility.SetData(target, _data.Size, _data.Padding);
            _cameraUtility.RetrieveCamera();
            _cameraUtility.SetBackground(_data.BackgroundData);
            _cameraUtility.AdjustCamera();
            _cameraUtility.AdjustCamera();
            
            CameraView = _cameraUtility.CaptureCameraView();
        }


        public void CreateIcon()
        {
            if (!_data.Targets.Any())
            {
                return;
            }

            foreach (GameObject target in _data.Targets)
            {
                _sceneHandler.InteractWithTarget(target, t =>
                {
                    AdjustCamera(t);
            
                    Texture2D icon = _cameraUtility.CaptureCameraView();
                    _iconsSaver.SaveIcon(icon, target.name);
                });
            }
        }
    }
}