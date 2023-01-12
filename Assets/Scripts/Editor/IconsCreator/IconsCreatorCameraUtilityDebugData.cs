using UnityEngine;

// ReSharper disable once CheckNamespace
namespace IconsCreatorNS
{
    public record IconsCreatorCameraUtilityDebugData(
        bool Ready,
        Vector3 CameraPosition,
        Bounds TargetBounds,
        Vector3 TargetBoundsCenter
        );
}