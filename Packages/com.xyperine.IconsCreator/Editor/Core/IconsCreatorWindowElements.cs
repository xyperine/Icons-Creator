using UnityEditor;
using UnityEngine;

namespace IconsCreationTool.Editor.Core
{
    public static class IconsCreatorWindowElements
    {
        private const float REGULAR_SPACE_SIZE_PX = 4f;
        private const float SMALL_SPACE_SIZE_PX = 4f;
        
        private static readonly GUIStyle BoldLabelStyle = new GUIStyle(EditorStyles.boldLabel);
        private static readonly GUIStyle ScopeBoxStyle = new GUIStyle(EditorStyles.helpBox);

        public static GUILayout.HorizontalScope HorizontalScope => new GUILayout.HorizontalScope();
        public static GUILayout.VerticalScope VerticalScope => new GUILayout.VerticalScope();
        public static GUILayout.VerticalScope VerticalScopeBox => new GUILayout.VerticalScope(ScopeBoxStyle);
        
        
        public static void DrawSmallSpace()
        {
            GUILayout.Space(SMALL_SPACE_SIZE_PX);
        }


        public static void DrawRegularSpace()
        {
            GUILayout.Space(REGULAR_SPACE_SIZE_PX);
        }


        public static void DrawBoldLabel(string labelText)
        {
            GUILayout.Label(labelText, BoldLabelStyle);
        }
    }
}