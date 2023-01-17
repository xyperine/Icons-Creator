using System.Linq;
using UnityEditor;
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
            _iconsSaver = new IconsSaver(_cameraUtility);
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
            
            _sceneHandler.InteractWithTarget(_data.TargetObject, AdjustCamera);
        }


        public void CreateIcon()
        {
            if (!_data.TargetObject)
            {
                return;
            }
            
            _sceneHandler.InteractWithTarget(_data.TargetObject, target =>
            {
                AdjustCamera(target);
            
                _iconsSaver.CreateIcon();
            });
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
        
        
        #region --- Debug Gizmos ---
        
        /// <summary>
        /// Doesn't work currently
        /// </summary>
        public void StartDrawingGizmos()
        {
            SceneView.duringSceneGui += DrawDebugGizmos;
        }


        /// <summary>
        /// Doesn't work currently
        /// </summary>
        public void StopDrawingGizmos()
        {
            SceneView.duringSceneGui -= DrawDebugGizmos;
        }

        
        private void DrawDebugGizmos(SceneView sceneView)
        {
            if (_cameraUtility == null)
            {
                return;
            }

            IconsCreatorCameraUtilityDebugData debugData = _cameraUtility.GetDebugData();

            if (!debugData.Ready)
            {
                return;
            }
            
            Color centerColor = new Color(0.93f, 0.19f, 0.51f);
            Color minColor = new Color(0.04f, 0.35f, 0.77f);
            Color maxColor = new Color(1f, 0.42f, 0.18f);
            
            Handles.color = centerColor;
            Vector3 normal = -sceneView.camera.transform.forward;
            Handles.DrawSolidDisc(debugData.TargetBoundsCenter, normal, 0.25f);
            
            Handles.DrawLine(debugData.CameraPosition, debugData.TargetBoundsCenter);

            Bounds targetBounds = debugData.TargetBounds;
            Handles.color = minColor;
            Handles.DrawSolidDisc(targetBounds.min, normal, 0.2f);
            Handles.color = maxColor;
            Handles.DrawSolidDisc(targetBounds.max, normal, 0.2f);
            Handles.color = Color.white;
            Handles.DrawWireCube(targetBounds.center, targetBounds.size);
        }
        
        #endregion
    }
}