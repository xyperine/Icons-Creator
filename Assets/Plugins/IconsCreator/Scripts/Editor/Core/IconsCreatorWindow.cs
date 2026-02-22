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
        [SerializeField] private IconsCreatorSettingsProfile settingsProfile;
        [SerializeField] private IconBackground backgroundType;
        [SerializeField] private Color backgroundColor = Color.white;
        [SerializeField] private Texture2D backgroundTexture;
        [SerializeField] private Texture2D frameTexture;
        
        [SerializeField] private string prefix;
        [SerializeField] private string suffix = "_Icon";
        [SerializeField] private int size = 512;
        [SerializeField] private float padding;
        
        [SerializeField] private bool renderShadows;

        [SerializeField] private List<Object> targets = new List<Object>();

        private const string DEFAULT_SETTINGS_PROFILE_PATH =
            "Assets/Plugins/IconsCreator/Data/Default_Settings_Profile.asset";
        
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

        private const string WHITE_ICON_PATH = "Assets/Plugins/IconsCreator/Textures/Icon_Dark_Theme.png";
        private const string BLACK_ICON_PATH = "Assets/Plugins/IconsCreator/Textures/Icon_Light_Theme.png";

        #endregion
        
        #region --- Serialized properties ---

        private SerializedObject _serializedObject;

        private SerializedProperty _settingsProfileSerializedProperty;
        private SerializedProperty _backgroundTypeSerializedProperty;
        private SerializedProperty _backgroundColorSerializedProperty;
        private SerializedProperty _backgroundTextureSerializedProperty;
        private SerializedProperty _frameTextureSerializedProperty;
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
            IconsCreatorWindow window = GetWindow<IconsCreatorWindow>(TITLE);

            string path = EditorGUIUtility.isProSkin ? WHITE_ICON_PATH : BLACK_ICON_PATH;

            window.titleContent.image = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }


        private void Awake()
        {
            _iconsCreator.InitializeEnvironment();
        }


        private void OnEnable()
        {
            settingsProfile = AssetDatabase.LoadAssetAtPath<IconsCreatorSettingsProfile>(DEFAULT_SETTINGS_PROFILE_PATH);
            if (!settingsProfile)
            {
                IconsCreatorSettingsProfile newSettingsProfile = CreateInstance<IconsCreatorSettingsProfile>();
                newSettingsProfile.SetValues(IconBackground.None, Color.white, null, null, string.Empty, "_Icon", 512,
                    0.1f, false);
                
                AssetDatabase.CreateAsset(newSettingsProfile, DEFAULT_SETTINGS_PROFILE_PATH);
                
                AssetDatabase.Refresh();
                
                settingsProfile = AssetDatabase.LoadAssetAtPath<IconsCreatorSettingsProfile>(DEFAULT_SETTINGS_PROFILE_PATH);
            }

            SetupSerializedProperties();
            
            ApplySettingsProfile(settingsProfile);
        }


        private void SetupSerializedProperties()
        {
            _serializedObject = new SerializedObject(this);
            
            _settingsProfileSerializedProperty = _serializedObject.FindProperty(nameof(settingsProfile));
            _backgroundTypeSerializedProperty = _serializedObject.FindProperty(nameof(backgroundType));
            _backgroundColorSerializedProperty = _serializedObject.FindProperty(nameof(backgroundColor));
            _backgroundTextureSerializedProperty = _serializedObject.FindProperty(nameof(backgroundTexture));
            _frameTextureSerializedProperty = _serializedObject.FindProperty(nameof(frameTexture));
            _prefixSerializedProperty = _serializedObject.FindProperty(nameof(prefix));
            _suffixSerializedProperty = _serializedObject.FindProperty(nameof(suffix));
            _sizeSerializedProperty = _serializedObject.FindProperty(nameof(size));
            _paddingSerializedProperty = _serializedObject.FindProperty(nameof(padding));
            _targetsObjectSerializedProperty = _serializedObject.FindProperty(nameof(targets));
            _renderShadowsSerializedProperty = _serializedObject.FindProperty(nameof(renderShadows));
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
                
                DrawSettingsProfileOptions();
                
                IconsCreatorWindowElements.DrawRegularSpace();
                
                DrawBackgroundOptions();

                IconsCreatorWindowElements.DrawRegularSpace();
                
                DrawFrameOption();
                
                IconsCreatorWindowElements.DrawRegularSpace();

                DrawNamingOptions();
            
                IconsCreatorWindowElements.DrawRegularSpace();

                DrawSizingOptions();

                IconsCreatorWindowElements.DrawRegularSpace();
                
                DrawOtherOptions();
                
                IconsCreatorWindowElements.DrawRegularSpace();
            }
        }


        private void DrawSettingsProfileOptions()
        {
            IconsCreatorWindowElements.DrawBoldLabel("Profile");
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(_settingsProfileSerializedProperty);
            
            if (EditorGUI.EndChangeCheck())
            {
                ApplySettingsProfile((IconsCreatorSettingsProfile) _settingsProfileSerializedProperty.objectReferenceValue);
            }

            if (GUILayout.Button("Save as"))
            {
                string path = EditorUtility.SaveFilePanelInProject("Save current settings", "New_Settings_Profile", "asset",
                    "OK");

                if (string.IsNullOrEmpty(path))
                {
                    return;
                }

                IconsCreatorSettingsProfile newSettingsProfile = CreateInstance<IconsCreatorSettingsProfile>();
                newSettingsProfile.SetValues(backgroundType, backgroundColor, backgroundTexture, frameTexture, prefix, suffix, size, padding, renderShadows);
                
                AssetDatabase.CreateAsset(newSettingsProfile, path);
                
                AssetDatabase.Refresh();
            }
        }


        private void ApplySettingsProfile(IconsCreatorSettingsProfile settingsProfile)
        {
            if (settingsProfile == null)
            {
                Debug.LogWarning("No settings profile!");
                return;
            }
            
            _backgroundTypeSerializedProperty.enumValueIndex = (int) settingsProfile.BackgroundType;
            _backgroundColorSerializedProperty.colorValue = settingsProfile.BackgroundColor;
            _backgroundTextureSerializedProperty.objectReferenceValue = settingsProfile.BackgroundTexture;

            _frameTextureSerializedProperty.objectReferenceValue = settingsProfile.FrameTexture;

            _prefixSerializedProperty.stringValue = settingsProfile.Prefix;
            _suffixSerializedProperty.stringValue = settingsProfile.Suffix;

            _sizeSerializedProperty.intValue = settingsProfile.Size;
            _paddingSerializedProperty.floatValue = settingsProfile.Padding;

            _renderShadowsSerializedProperty.boolValue = settingsProfile.RenderShadows;

            _serializedObject.ApplyModifiedProperties();
            
            UpdateIconsCreator();
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


        private void DrawFrameOption()
        {
            EditorGUILayout.PropertyField(_frameTextureSerializedProperty);
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
                new IconsCreatorData(size, padding, prefix, suffix, backgroundData, frameTexture, targets, renderShadows);
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