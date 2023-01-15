using UnityEngine;

// ReSharper disable once CheckNamespace
namespace IconsCreatorNS
{
    public readonly struct IconsCreatorCameraUtilityDebugData
    {
        public bool Ready { get; }
        public Vector3 CameraPosition { get; }
        public Bounds TargetBounds { get; }
        public Vector3 TargetBoundsCenter { get; }


        public IconsCreatorCameraUtilityDebugData(bool ready, Vector3 cameraPosition, Bounds targetBounds)
        {
            Ready = ready;
            CameraPosition = cameraPosition;
            TargetBounds = targetBounds;
            TargetBoundsCenter = targetBounds.center;
        }
    }
}