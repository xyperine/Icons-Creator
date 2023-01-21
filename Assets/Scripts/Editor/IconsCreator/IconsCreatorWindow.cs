﻿using System;
using System.Collections.Generic;
using System.Linq;
using IconsCreationTool.Utility.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IconsCreationTool
{
    public class IconsCreatorWindow : EditorWindow
    {
        [SerializeField] private IconBackground backgroundType;
        [SerializeField] private Color backgroundColor = Color.white;
        [SerializeField] private Texture2D backgroundTexture;
        
        [SerializeField] private string prefix;
        [SerializeField] private string suffix;
        [SerializeField] private int size = 512;
        [SerializeField] private float padding;
        
        [SerializeField] private List<Object> targets = new List<Object>();

        [SerializeField] private TextureImporterCompression compression = TextureImporterCompression.Compressed;
        [SerializeField] private FilterMode filterMode = FilterMode.Bilinear;

        private const int PREVIEW_SIZE = 256;
        
        private readonly IconsCreator _iconsCreator = new IconsCreator();
        
        private Vector2 _scrollPosition;
        private bool _advancedSettingsUnfolded;
        
        private Texture2D _previewTexture;
        
        private bool AnyTargets => targets.ExtractAllGameObjects().Any();

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
        private SerializedProperty _compressionSerializedProperty;
        private SerializedProperty _filterModeSerializedProperty;
        
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
            compression = (TextureImporterCompression) EditorPrefs.GetInt(nameof(compression));
            filterMode = (FilterMode) EditorPrefs.GetInt(nameof(filterMode));
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
            _compressionSerializedProperty = _serializedObject.FindProperty(nameof(compression));
            _filterModeSerializedProperty = _serializedObject.FindProperty(nameof(filterMode));
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
            EditorPrefs.SetInt(nameof(compression), (int) compression);
            EditorPrefs.SetInt(nameof(filterMode), (int) filterMode);
        }


        protected void OnGUI()
        {
            using GUILayout.ScrollViewScope scrollView = new GUILayout.ScrollViewScope(_scrollPosition);
            _scrollPosition = scrollView.scrollPosition;
                
            _serializedObject.Update();

            DrawSettings();
            
            GUILayout.Space(8f);

            DrawTargetsProperties();
            
            GUILayout.Space(8f);

            if (_serializedObject.ApplyModifiedProperties())
            {
                UpdateIconsCreator();
            }
            
            DrawPreview();
            
            GUILayout.Space(8f);
            
            DrawCreateIconButton();
        }


        private void DrawSettings()
        {
            using (new GUILayout.VerticalScope(new GUIStyle(EditorStyles.helpBox)))
            {
                GUILayout.Label("Settings", EditorStyles.boldLabel);
                GUILayout.Space(4f);

                DrawBasicSettings();
            
                GUILayout.Space(8f);

                DrawAdvancedSettings();
                
                GUILayout.Space(8f);
            }
        }


        private void DrawBasicSettings()
        {
            DrawBackgroundOptions();

            GUILayout.Space(8f);

            DrawNamingProperties();
            
            GUILayout.Space(8f);

            DrawSizingProperties();
        }


        private void DrawBackgroundOptions()
        {
            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label("Background", new GUIStyle(EditorStyles.boldLabel));

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Type");
                    
                    Undo.RecordObject(this, TITLE);
                    backgroundType =
                        (IconBackground) GUILayout.Toolbar((int) backgroundType, Enum.GetNames(typeof(IconBackground)));
                    _backgroundTypeSerializedProperty.enumValueIndex = (int) backgroundType;
                }
                
                GUILayout.Space(4f);

                switch (backgroundType)
                {
                    case IconBackground.None:
                        break;
                
                    case IconBackground.Color:
                        EditorGUILayout.PropertyField(_backgroundColorSerializedProperty);
                        break;
                
                    case IconBackground.Texture:
                        EditorGUILayout.PropertyField(_backgroundTextureSerializedProperty);
                        break;
                
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                GUILayout.Space(4f);
            }
        }


        private void DrawNamingProperties()
        {
            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label("Naming", new GUIStyle(EditorStyles.boldLabel));
                
                EditorGUILayout.PropertyField(_prefixSerializedProperty);
                EditorGUILayout.PropertyField(_suffixSerializedProperty);
                
                GUILayout.Space(4f);
            }
        }


        private void DrawSizingProperties()
        {
            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label("Sizing", new GUIStyle(EditorStyles.boldLabel));

                EditorGUILayout.IntSlider(_sizeSerializedProperty, 1, 1024);
                EditorGUILayout.Slider(_paddingSerializedProperty, 0f, 0.9f);
            }
        }


        private void DrawTargetsProperties()
        {
            using (new GUILayout.VerticalScope(new GUIStyle(EditorStyles.helpBox)))
            {
                GUILayout.Label("Targets", new GUIStyle(EditorStyles.boldLabel));

                GUIContent content = new GUIContent("List");
                EditorGUILayout.PropertyField(_targetsObjectSerializedProperty, content);
                int targetsCount = _targetsObjectSerializedProperty.arraySize;
                for (int i = 0; i < targetsCount; i++)
                {
                    SerializedProperty targetProperty = _targetsObjectSerializedProperty.GetArrayElementAtIndex(i);
                    Object target = targetProperty.objectReferenceValue;
                    if (!target)
                    {
                        continue;
                    }

                    bool isAllowedType = target is GameObject || target.IsFolderContainingGameObjects();
                    if (!isAllowedType)
                    {
                        Debug.LogWarning("Asset must be either a folder containing game objects or a game object!");
                        targetProperty.objectReferenceValue = null;
                        continue;
                    }

                    GameObject targetObject = target as GameObject;
                    if (targetObject)
                    {
                        bool isSceneObject = targetObject.scene.IsValid();
                        if (isSceneObject)
                        {
                            continue;
                        }
                    }

                    bool isInAssetsFolder = AssetDatabase.GetAssetPath(target)[..6] == "Assets";
                    if (!isInAssetsFolder)
                    {
                        Debug.LogWarning("Select scene object or an asset from Assets folder!");
                        targetProperty.objectReferenceValue = null;
                    }
                }

                if (targets.Any(t => !t) || targets.Distinct().Count() < targets.Count)
                {
                    if (GUILayout.Button("Remove invalid elements"))
                    {
                        RemoveInvalidTargetReferences();
                    }
                }
                
                GUILayout.Space(4f);
            }
        }


        private void RemoveInvalidTargetReferences()
        {
            targets?.RemoveAll(t => !t);
            targets = targets?.Distinct().ToList();
        }


        private void DrawAdvancedSettings()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                _advancedSettingsUnfolded = EditorGUILayout.Foldout(_advancedSettingsUnfolded, "Advanced");

                if (!_advancedSettingsUnfolded)
                {
                    return;
                }

                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(_compressionSerializedProperty);
                    EditorGUILayout.PropertyField(_filterModeSerializedProperty);
                }
                
                GUILayout.Space(4f);
            }
        }


        private void DrawCreateIconButton()
        {
            using (new EditorGUI.DisabledScope(!AnyTargets))
            {
                GUILayout.Space(4f);
                
                string buttonText = targets.ExtractAllGameObjects().Count > 1 ?
                    "Create Icons" : "Create Icon";
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
                    {fixedHeight = 32, fontSize = 16, fontStyle = FontStyle.Bold};
                if (GUILayout.Button(buttonText, buttonStyle))
                {
                    RemoveInvalidTargetReferences();
                    UpdateIconsCreator();
                    _iconsCreator.CreateIcon();
                }

                GUILayout.Space(8f);
            }
        }


        private void UpdateIconsCreator()
        {
            IconBackgroundData backgroundData = new IconBackgroundData(backgroundType, backgroundColor, backgroundTexture);
            IconsCreatorData data =
                new IconsCreatorData(size, padding, prefix, suffix, compression, filterMode, backgroundData, targets);
            _iconsCreator.SetData(data);
                
            UpdatePreviewTexture();
        }


        private void UpdatePreviewTexture()
        {
            if (!AnyTargets)
            {
                return;
            }
            
            Texture2D cameraView = _iconsCreator.CameraView;
            cameraView.filterMode = filterMode;

            _previewTexture = cameraView.Resize(PREVIEW_SIZE);
        }


        private void DrawPreview()
        {
            if (!AnyTargets)
            {
                return;
            }
            
            if (!_previewTexture)
            {
                return;
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Preview", EditorStyles.boldLabel);
                
                GUILayout.Space(4f);

                GUIStyle boxStyle = new GUIStyle(GUI.skin.box) {margin = new RectOffset(32, 32, 32, 32)};
                GUILayoutOption[] boxOptions = {GUILayout.Width(256f), GUILayout.Height(256f)};
                GUILayout.Box(_previewTexture, boxStyle, boxOptions);
            }
        }
    }
}