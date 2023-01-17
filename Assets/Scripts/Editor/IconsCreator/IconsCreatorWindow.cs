using UnityEditor;
using UnityEngine;

namespace IconsCreationTool
{
    public class IconsCreatorWindow : EditorWindow
    {
        [SerializeField] private new string name = "Icon";
        [SerializeField] private int resolution = 512;
        [SerializeField] private float padding;

        [SerializeField] private GameObject targetObject;

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

        private SerializedProperty _nameSerializedProperty;
        private SerializedProperty _resolutionSerializedProperty;
        private SerializedProperty _paddingSerializedProperty;
        private SerializedProperty _targetObjectSerializedProperty;
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
            name = EditorPrefs.GetString(nameof(name));
            resolution = EditorPrefs.GetInt(nameof(resolution));
            padding = EditorPrefs.GetFloat(nameof(padding));
            compression = (TextureImporterCompression) EditorPrefs.GetInt(nameof(compression));
            filterMode = (FilterMode) EditorPrefs.GetInt(nameof(filterMode));
        }


        private void SetupSerializedProperties()
        {
            _serializedObject = new SerializedObject(this);
            
            _nameSerializedProperty = _serializedObject.FindProperty(nameof(name));
            _resolutionSerializedProperty = _serializedObject.FindProperty(nameof(resolution));
            _paddingSerializedProperty = _serializedObject.FindProperty(nameof(padding));
            _targetObjectSerializedProperty = _serializedObject.FindProperty(nameof(targetObject));
            _compressionSerializedProperty = _serializedObject.FindProperty(nameof(compression));
            _filterModeSerializedProperty = _serializedObject.FindProperty(nameof(filterMode));
        }


        private void OnDisable()
        {
            Save();
        }


        private void Save()
        {
            EditorPrefs.SetString(nameof(name), name);
            EditorPrefs.SetInt(nameof(resolution), resolution);
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
            EditorGUILayout.PropertyField(_nameSerializedProperty);
            EditorGUILayout.IntSlider(_resolutionSerializedProperty, 1, 1024);
            EditorGUILayout.Slider(_paddingSerializedProperty, 0f, 0.9f);
            EditorGUILayout.PropertyField(_targetObjectSerializedProperty);
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
            using (new EditorGUI.DisabledScope(!targetObject))
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
            IconsCreatorData data =
                new IconsCreatorData(resolution, padding, name, compression, filterMode, targetObject);
            _iconsCreator.SetData(data);
                
            UpdatePreviewTexture();
        }


        private void UpdatePreviewTexture()
        {
            if (!targetObject)
            {
                return;
            }

            _previewTexture = _iconsCreator.CameraView;
            _previewTexture.filterMode = filterMode;
            
            RenderTexture temporaryRenderTexture = RenderTexture.GetTemporary(PREVIEW_SIZE, PREVIEW_SIZE);
            RenderTexture.active = temporaryRenderTexture;
            
            Graphics.Blit(_previewTexture, temporaryRenderTexture);
            
            _previewTexture.Reinitialize(PREVIEW_SIZE, PREVIEW_SIZE, _previewTexture.graphicsFormat, false);
            _previewTexture.filterMode = filterMode;
            _previewTexture.ReadPixels(new Rect(0, 0, PREVIEW_SIZE, PREVIEW_SIZE), 0, 0);
            _previewTexture.Apply();
            
            RenderTexture.ReleaseTemporary(temporaryRenderTexture);
        }


        private void DrawPreview()
        {
            if (!targetObject)
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