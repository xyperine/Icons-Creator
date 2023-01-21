using System;
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
        [SerializeField] private Color backgroundColor;
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

        private bool _advancedSettingsUnfolded;

        private Texture2D _previewTexture;

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
            _serializedObject.Update();

            DrawSettings();
            
            GUILayout.Space(8f);

            DrawButtons();
                
            GUILayout.Space(8f);

            if (_serializedObject.ApplyModifiedProperties())
            {
                OnDataChanged();
            }

            DrawPreview();
        }


        private void DrawSettings()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Settings", EditorStyles.boldLabel);
                GUILayout.Space(4f);

                DrawBasicSettings();
            
                GUILayout.Space(8f);

                DrawAdvancedSettings();
            }
        }


        private void DrawBasicSettings()
        {
            DrawBackgroundOptions();

            GUILayout.Space(4f);

            EditorGUILayout.PropertyField(_prefixSerializedProperty);
            EditorGUILayout.PropertyField(_suffixSerializedProperty);
            EditorGUILayout.IntSlider(_sizeSerializedProperty, 1, 1024);
            EditorGUILayout.Slider(_paddingSerializedProperty, 0f, 0.9f);

            DrawTargetsProperty();
        }


        private void DrawBackgroundOptions()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Background");

                Undo.RecordObject(this, TITLE);
                backgroundType =
                    (IconBackground) GUILayout.Toolbar((int) backgroundType, Enum.GetNames(typeof(IconBackground)));
                _backgroundTypeSerializedProperty.enumValueIndex = (int) backgroundType;
            }

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
        }


        private void DrawTargetsProperty()
        {
            EditorGUILayout.PropertyField(_targetsObjectSerializedProperty);
            int targetsCount = _targetsObjectSerializedProperty.arraySize;
            for (int i = 0; i < targetsCount; i++)
            {
                SerializedProperty targetProperty = _targetsObjectSerializedProperty.GetArrayElementAtIndex(i);
                Object target = targetProperty.objectReferenceValue;
                if (!target)
                {
                    continue;
                }

                bool isAllowedType = target is GameObject or DefaultAsset;
                if (!isAllowedType)
                {
                    Debug.LogWarning("Asset must be either a folder or a GameObject!");
                    targetProperty.objectReferenceValue = null;
                    continue;
                }
                
                bool isInAssetsFolder = AssetDatabase.GetAssetPath(target)[..6] == "Assets";
                if (!isInAssetsFolder)
                {
                    Debug.LogWarning("Select asset from Assets folder!");
                    targetProperty.objectReferenceValue = null;
                }
            }
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
            }
        }


        private void DrawButtons()
        {
            using (new EditorGUI.DisabledScope(!targets.Any(t => t)))
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Actions", EditorStyles.boldLabel);
                    
                    GUILayout.Space(4f);
                    
                    if (GUILayout.Button("Create Icon"))
                    {
                        _iconsCreator.CreateIcon();
                    }
                }
            }
        }


        private void OnDataChanged()
        {
            IconBackgroundData backgroundData = new IconBackgroundData(backgroundType, backgroundColor, backgroundTexture);
            IconsCreatorData data =
                new IconsCreatorData(size, padding, prefix, suffix, compression, filterMode, backgroundData, targets);
            _iconsCreator.SetData(data);
                
            UpdatePreviewTexture();
        }


        private void UpdatePreviewTexture()
        {
            if (!targets.Any(t => t))
            {
                return;
            }

            Texture2D cameraView = _iconsCreator.CameraView;
            cameraView.filterMode = filterMode;

            _previewTexture = cameraView.Resize(PREVIEW_SIZE);
        }


        private void DrawPreview()
        {
            if (!targets.Any(t => t))
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