using System;
using UnityEngine;

namespace IconsCreationTool
{
    public class IconsCreatorCompatibleTargetProvider
    {
        private GameObject _previousManualTarget;
        private GameObject _previousAutoTarget;


        public void SetPreviousTarget(GameObject targetObject)
        {
            if (!targetObject)
            {
                return;
            }
            
            if (IsObjectCompatibleWithWorkflow(targetObject, IconsCreatorUserWorkflow.Manual))
            {
                _previousManualTarget = targetObject;
                return;
            }

            _previousAutoTarget = targetObject;
        }


        public GameObject GetTarget(IconsCreatorUserWorkflow workflow)
        {
            return workflow switch
            {
                IconsCreatorUserWorkflow.Auto => _previousAutoTarget,
                IconsCreatorUserWorkflow.Manual => _previousManualTarget,
                _ => throw new ArgumentOutOfRangeException(nameof(workflow), workflow, null),
            };
        }


        public bool IsObjectCompatibleWithWorkflow(GameObject gameObject, IconsCreatorUserWorkflow workflow)
        {
            if (!gameObject)
            {
                return false;
            }
            
            bool isSceneObject = gameObject.scene.IsValid();
            bool isCompatibleWithWorkflow = (workflow == IconsCreatorUserWorkflow.Auto && !isSceneObject) ||
                                            (workflow == IconsCreatorUserWorkflow.Manual && isSceneObject);

            return isCompatibleWithWorkflow;
        }
    }
}