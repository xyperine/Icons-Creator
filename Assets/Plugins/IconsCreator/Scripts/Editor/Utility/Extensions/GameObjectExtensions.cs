using UnityEngine;

namespace IconsCreationTool.Editor.Utility.Extensions
{
    public static class GameObjectExtensions
    {
        public static Bounds GetOrthographicBounds(this GameObject gameObject, Camera camera)
        {
            Vector3 minScreenPosition = Vector3.positiveInfinity;
            Vector3 maxScreenPosition = Vector3.negativeInfinity;

            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            
            foreach (MeshFilter meshFilter in meshFilters)
            {
                if (!meshFilter.sharedMesh)
                {
                    continue;
                }
                
                Vector3[] vertices = meshFilter.sharedMesh.vertices;

                foreach (Vector3 vertex in vertices)
                {
                    Vector3 wsVertexPosition = meshFilter.transform.TransformPoint(vertex);
                    Vector3 screenPosition = camera.WorldToScreenPoint(wsVertexPosition);

                    for (int i = 0; i < 3; i++)
                    {
                        minScreenPosition[i] = Mathf.Min(minScreenPosition[i], screenPosition[i]);
                        maxScreenPosition[i] = Mathf.Max(maxScreenPosition[i], screenPosition[i]);
                    }
                }
            }
            
            Vector3 min = camera.ScreenToWorldPoint(minScreenPosition);
            Vector3 max = camera.ScreenToWorldPoint(maxScreenPosition);

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);
            
            return bounds;
        }


        public static bool HasVisibleMesh(this GameObject gameObject)
        {
            bool hasVisibleMesh = false;
            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            
            foreach (MeshFilter meshFilter in meshFilters)
            {
                if (!meshFilter)
                {
                    continue;
                }

                bool hasMesh = meshFilter.sharedMesh;
                if (!hasMesh)
                {
                    continue;
                }

                bool hasRendererForMesh = meshFilter.GetComponent<MeshRenderer>();
                if (!hasRendererForMesh)
                {
                    continue;
                }
                
                hasVisibleMesh = true;
                break;
            }

            return hasVisibleMesh;
        }
    }
}