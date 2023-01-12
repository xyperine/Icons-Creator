using System;
using UnityEngine;

namespace IconsCreationTool.Generator
{
    public sealed class GeneratorLightingController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material lightMaterial;
        [SerializeField] private float lightLevel = 1f;

        private static readonly int EmissionStrengthPropertyID = Shader.PropertyToID("_Emission_Strength");

        private Material _lightMaterialCopy;

        private float _defaultEmission;


        private void Awake()
        {
            _defaultEmission = lightMaterial.GetFloat(EmissionStrengthPropertyID);

            int materialIndex = Array.IndexOf(meshRenderer.sharedMaterials, lightMaterial);
            _lightMaterialCopy = meshRenderer.materials[materialIndex];
        }


        private void Update()
        {
            _lightMaterialCopy.SetFloat(EmissionStrengthPropertyID, _defaultEmission * lightLevel);
        }
    }
}
