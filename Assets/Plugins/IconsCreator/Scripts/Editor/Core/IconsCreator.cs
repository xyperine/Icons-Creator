using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace IconsCreationTool.Editor.Core
{
    public class IconsCreator
    {
        private IconsCreatorData _data;

        private readonly IconsCreatorInternalSceneHandler _sceneHandler;
        private readonly IconsCreatorCamera _iconsCreatorCamera;
        private readonly IconsSaver _iconsSaver;

        private bool AnyTargets => _data.Targets.Any(t => t);
        
        public Texture2D CameraView { get; private set; }


        public IconsCreator()
        {
            _sceneHandler = new IconsCreatorInternalSceneHandler();
            _iconsCreatorCamera = new IconsCreatorCamera();
            _iconsSaver = new IconsSaver();
        }


        public void InitializeEnvironment()
        {
            AddIconsCreationCameraTag();
            _sceneHandler.TryCreateScene(_iconsCreatorCamera.IconsCreationCameraTag);
        }


        private void AddIconsCreationCameraTag()
        {
            if (!InternalEditorUtility.tags.Contains(_iconsCreatorCamera.IconsCreationCameraTag))
            {
                InternalEditorUtility.AddTag(_iconsCreatorCamera.IconsCreationCameraTag);
            }
        }


        public void SetData(IconsCreatorData data)
        {
            _data = data;

            _iconsCreatorCamera.SetData(_data.Targets.FirstOrDefault(), _data.Size, _data.Padding);
            _iconsCreatorCamera.SetFrameTexture(data.FrameTexture);
            _iconsSaver.SetData(_data.Prefix, _data.Suffix);

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

            _sceneHandler.InteractWithTarget(_data.Targets[0], _data.RenderShadows, AdjustCamera);
        }


        private void AdjustCamera(GameObject target)
        {
            _iconsCreatorCamera.SetData(target, _data.Size, _data.Padding);
            _iconsCreatorCamera.RetrieveCamera();
            _iconsCreatorCamera.SetBackground(_data.BackgroundData);
            _iconsCreatorCamera.AdjustCamera();
            _iconsCreatorCamera.AdjustCamera(); // have to do a double call for some reason
            
            CameraView = _iconsCreatorCamera.CaptureCameraView();
        }


        public void CreateIcon()
        {
            if (!_data.Targets.Any())
            {
                return;
            }
            
            List<Texture2D> images = new List<Texture2D>(_data.Targets.Length);
            List<string> names = new List<string>(_data.Targets.Length);

            foreach (GameObject target in _data.Targets)
            {
                _sceneHandler.InteractWithTarget(target, _data.RenderShadows, t =>
                {
                    AdjustCamera(t);
            
                    Texture2D icon = _iconsCreatorCamera.CaptureCameraView();
                    images.Add(icon);
                    names.Add(target.name);
                });
            }
            
            _iconsSaver.SaveIcons(images.ToArray(), names.ToArray());
        }
    }
}