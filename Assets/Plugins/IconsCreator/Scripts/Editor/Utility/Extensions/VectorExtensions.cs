using UnityEngine;

namespace IconsCreationTool.Editor.Utility.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 Abs(this Vector2 a)
        {
            a.Set(Mathf.Abs(a.x), Mathf.Abs(a.y));
            return a;
        }


        public static float BiggestComponentValue(this Vector2 a)
        {
            return Mathf.Max(a.x, a.y);
        }
    }
}