using System;
using System.Collections.Generic;
using System.Linq;
using IconsCreationTool.Editor.Utility.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IconsCreationTool.Editor.Core
{
    public class IconsCreatorWindow : EditorWindow
    {
        [SerializeField] private IconBackground backgroundType;
        [SerializeField] private Color backgroundColor = Color.white;
        [SerializeField] private Texture2D backgroundTexture;
        
        [SerializeField] private string prefix;
        [SerializeField] private string suffix = "_Icon";
        [SerializeField] private int size = 512;
        [SerializeField] private float padding;
        
        [SerializeField] private bool renderShadows;

        [SerializeField] private List<Object> targets = new List<Object>();

        private const int PREVIEW_SIZE = 256;
        
        private readonly IconsCreator _iconsCreator = new IconsCreator();
        
        private Vector2 _scrollPosition;

        private Texture2D _previewTexture;
        
        private bool AnyTargets => targets.ExtractAllGameObjects().Where(g => g.HasVisibleMesh()).ToList().Any();

        #region --- Window name ---

        private const string MENU_NAME = "Tools/Icons Creator";
        private const string HOTKEYS = "%#I";
        private const string FULL_MENU_NAME = MENU_NAME + " " + HOTKEYS;
        
        private const string TITLE = "Icons Creator";

        #endregion
        
        #region --- Serialized properties ---

        private SerializedObject _serializedObject;

        private SerializedProperty _backgroundTypeSerializedProperty;
        private SerializedProperty _backgroundColorSerializedProperty;
        private SerializedProperty _backgroundTextureSerializedProperty;
        private SerializedProperty _prefixSerializedProperty;
        private SerializedProperty _suffixSerializedProperty;
        private SerializedProperty _sizeSerializedProperty;
        private SerializedProperty _paddingSerializedProperty;
        private SerializedProperty _targetsObjectSerializedProperty;
        private SerializedProperty _renderShadowsSerializedProperty;

        #endregion


        [MenuItem(FULL_MENU_NAME)]
        private static void OpenWindow()
        {
            GetWindow<IconsCreatorWindow>(TITLE);
        }


        private void Awake()
        {
            _iconsCreator.InitializeEnvironment();
        }


        private void OnEnable()
        {
            Load();
            
            SetupSerializedProperties();
        }


        private void Load()
        {
            prefix = EditorPrefs.GetString(nameof(prefix));
            suffix = EditorPrefs.GetString(nameof(suffix));
            size = EditorPrefs.GetInt(nameof(size));
            padding = EditorPrefs.GetFloat(nameof(padding));
        }


        private void SetupSerializedProperties()
        {
            _serializedObject = new SerializedObject(this);
            
            _backgroundTypeSerializedProperty = _serializedObject.FindProperty(nameof(backgroundType));
            _backgroundColorSerializedProperty = _serializedObject.FindProperty(nameof(backgroundColor));
            _backgroundTextureSerializedProperty = _serializedObject.FindProperty(nameof(backgroundTexture));
            _prefixSerializedProperty = _serializedObject.FindProperty(nameof(prefix));
            _suffixSerializedProperty = _serializedObject.FindProperty(nameof(suffix));
            _sizeSerializedProperty = _serializedObject.FindProperty(nameof(size));
            _paddingSerializedProperty = _serializedObject.FindProperty(nameof(padding));
            _targetsObjectSerializedProperty = _serializedObject.FindProperty(nameof(targets));
            _renderShadowsSerializedProperty = _serializedObject.FindProperty(nameof(renderShadows));
        }


        private void OnDisable()
        {
            Save();
        }


        private void Save()
        {
            EditorPrefs.SetString(nameof(prefix), prefix);
            EditorPrefs.SetString(nameof(suffix), suffix);
            EditorPrefs.SetInt(nameof(size), size);
            EditorPrefs.SetFloat(nameof(padding), padding);
        }


        protected void OnGUI()
        {
            using GUILayout.ScrollViewScope scrollView = new GUILayout.ScrollViewScope(_scrollPosition);
            _scrollPosition = scrollView.scrollPosition;
                
            _serializedObject.Update();

            DrawSettings();
            
            IconsCreatorWindowElements.DrawRegularSpace();

            DrawObjectsOptions();
            
            IconsCreatorWindowElements.DrawRegularSpace();

            if (_serializedObject.ApplyModifiedProperties())
            {
                UpdateIconsCreator();
            }
            
            DrawPreview();
            
            IconsCreatorWindowElements.DrawRegularSpace();
            
            DrawCreateIconButton();
        }


        private void DrawSettings()
        {
            using (IconsCreatorWindowElements.VerticalScopeBox)
            {
                IconsCreatorWindowElements.DrawBoldLabel("Settings");
                
                IconsCreatorWindowElements.DrawSmallSpace();
                
                DrawBackgroundOptions();

                IconsCreatorWindowElements.DrawRegularSpace();

                DrawNamingOptions();
            
                IconsCreatorWindowElements.DrawRegularSpace();

                DrawSizingOptions();

                IconsCreatorWindowElements.DrawRegularSpace();
                
                DrawOtherOptions();
                
                IconsCreatorWindowElements.DrawRegularSpace();
            }
        }


        private void DrawBackgroundOptions()
        {
            using (IconsCreatorWindowElements.VerticalScope)
            {
                IconsCreatorWindowElements.DrawBoldLabel("Background");

                DrawBackgroundTypeOption();

                IconsCreatorWindowElements.DrawSmallSpace();

                DrawBackgroundPickerOption();
            }
        }


        private void DrawBackgroundTypeOption()
        {
            using (IconsCreatorWindowElements.HorizontalScope)
            {
                GUILayout.Label("Type", GUILayout.Width(EditorGUIUtility.labelWidth));

                Undo.RecordObject(this, TITLE);
                backgroundType =
                    (IconBackground) GUILayout.Toolbar((int) backgroundType, Enum.GetNames(typeof(IconBackground)));
                _backgroundTypeSerializedProperty.enumValueIndex = (int) backgroundType;
            }
        }


        private void DrawBackgroundPickerOption()
        {
            switch (backgroundType)
            {
                case IconBackground.None:
                    break;

                case IconBackground.Color:
                    EditorGUILayout.PropertyField(_backgroundColorSerializedProperty);
                    IconsCreatorWindowElements.DrawSmallSpace();
                    break;

                case IconBackground.Texture:
                    EditorGUILayout.PropertyField(_backgroundTextureSerializedProperty);
                    IconsCreatorWindowElements.DrawSmallSpace();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void DrawNamingOptions()
        {
            using (IconsCreatorWindowElements.VerticalScope)
            {
                IconsCreatorWindowElements.DrawBoldLabel("Naming");
                
                EditorGUILayout.PropertyField(_prefixSerializedProperty);
                EditorGUILayout.PropertyField(_suffixSerializedProperty);
                
                IconsCreatorWindowElements.DrawSmallSpace();
            }
        }


        private void DrawSizingOptions()
        {
            using (IconsCreatorWindowElements.VerticalScope)
            {
                IconsCreatorWindowElements.DrawBoldLabel("Sizing");

                EditorGUILayout.IntSlider(_sizeSerializedProperty, 1, 1024);
                EditorGUILayout.Slider(_paddingSerializedProperty, 0f, 0.9f);
            }
        }


        private void DrawOtherOptions()
        {
            using (IconsCreatorWindowElements.VerticalScope)
            {
                IconsCreatorWindowElements.DrawBoldLabel("Other");
                
                EditorGUILayout.PropertyField(_renderShadowsSerializedProperty);

                IconsCreatorWindowElements.DrawSmallSpace();
            }
        }


        private void DrawObjectsOptions()
        {
            using (IconsCreatorWindowElements.VerticalScopeBox)
            {
                IconsCreatorWindowElements.DrawBoldLabel("Objects");

                GUIContent content = new GUIContent("List");
                EditorGUILayout.PropertyField(_targetsObjectSerializedProperty, content);
                
                int targetsCount = _targetsObjectSerializedProperty.arraySize;
                for (int i = 0; i < targetsCount; i++)
                {
                    SerializedProperty targetProperty = _targetsObjectSerializedProperty.GetArrayElementAtIndex(i);
                    ValidateTargetProperty(targetProperty);
                }

                if (targets.Any(t => !t) || targets.Distinct().Count() < targets.Count)
                {
                    if (GUILayout.Button("Remove invalid objects"))
                    {
                        RemoveInvalidTargetReferences();
                    }
                }

                IconsCreatorWindowElements.DrawSmallSpace();
            }
        }


        private void ValidateTargetProperty(SerializedProperty targetProperty)
        {
            Object target = targetProperty.objectReferenceValue;
            if (!target)
            {
                return;
            }

            bool isAllowedType = target is GameObject || target.IsFolderContainingGameObjects();
            if (!isAllowedType)
            {
                Debug.LogWarning(
                    $"Asset \"{target.name}\" is invalid! Asset has to be either a game object or a folder containing game objects!");
                targetProperty.objectReferenceValue = null;
                return;
            }

            GameObject targetObject = target as GameObject;
            if (targetObject)
            {
                string objectName = targetObject.name;

                bool objectHasRenderingComponents = targetObject.GetComponentInChildren<MeshRenderer>() && 
                                                    targetObject.GetComponentInChildren<MeshFilter>();
                if (!objectHasRenderingComponents)
                {
                    Debug.LogWarning($"Game object \"{objectName}\" must have active MeshFilter and MeshRenderer components in its hierarchy!");
                    targetProperty.objectReferenceValue = null;
                    return;
                }

                bool objectHasAtLeastOneMesh = targetObject.GetComponentsInChildren<MeshFilter>().Any(f => f.sharedMesh);
                if (!objectHasAtLeastOneMesh)
                {
                    Debug.LogWarning($"Game object \"{objectName}\" must have at least one MeshFilter with an assigned mesh in its hierarchy!");
                    targetProperty.objectReferenceValue = null;
                    return;
                }
                
                bool isSceneObject = targetObject.scene.IsValid();
                if (isSceneObject)
                {
                    return;
                }
            }

            bool isInAssetsFolder = AssetDatabase.GetAssetPath(target)[..6] == "Assets";
            if (!isInAssetsFolder)
            {
                Debug.LogWarning("Select scene object or an asset from Assets folder!");
                targetProperty.objectReferenceValue = null;
            }
        }


        private void RemoveInvalidTargetReferences()
        {
            targets?.RemoveAll(t => !t);
            targets = targets?.Distinct().ToList();
        }


        private void DrawCreateIconButton()
        {
            if (!AnyTargets)
            {
                return;
            }
            
            using (new EditorGUI.DisabledScope(!AnyTargets))
            {
                IconsCreatorWindowElements.DrawSmallSpace();
                
                string buttonText = targets.ExtractAllGameObjects().Where(g => g.HasVisibleMesh()).ToList().Count > 1 ?
                    "Create Icons" : "Create Icon";
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
                    {fixedHeight = 28, fontSize = 13, fontStyle = FontStyle.Bold};
                if (GUILayout.Button(buttonText, buttonStyle))
                {
                    RemoveInvalidTargetReferences();
                    UpdateIconsCreator();
                    _iconsCreator.CreateIcon();
                }

                IconsCreatorWindowElements.DrawRegularSpace();
            }
        }


        private void UpdateIconsCreator()
        {
            IconBackgroundData backgroundData = new IconBackgroundData(backgroundType, backgroundColor, backgroundTexture);
            IconsCreatorData data =
                new IconsCreatorData(size, padding, prefix, suffix, backgroundData, targets, renderShadows);
            _iconsCreator.SetData(data);
                
            UpdatePreviewTexture();
        }


        private void UpdatePreviewTexture()
        {
            if (!AnyTargets)
            {
                _previewTexture = null;
                return;
            }
            
            Texture2D cameraView = _iconsCreator.CameraView;
            _previewTexture = cameraView.Resize(PREVIEW_SIZE);
        }


        private void DrawPreview()
        {
            if (!_previewTexture)
            {
                return;
            }

            using (IconsCreatorWindowElements.VerticalScopeBox)
            {
                IconsCreatorWindowElements.DrawBoldLabel("Preview");

                IconsCreatorWindowElements.DrawSmallSpace();

                GUIStyle boxStyle = new GUIStyle(GUI.skin.box) {margin = new RectOffset(32, 32, 32, 32)};
                GUILayoutOption[] boxOptions = {GUILayout.Width(PREVIEW_SIZE), GUILayout.Height(PREVIEW_SIZE)};
                GUILayout.Box(_previewTexture, boxStyle, boxOptions);
            }
        }
    }
}