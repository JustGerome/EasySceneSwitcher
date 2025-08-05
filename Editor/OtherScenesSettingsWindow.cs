#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.IO;

public class OtherScenesSettingsWindow : EditorWindow
{
    // New path under your target folder:
    const string k_AssetPath = "Assets/Moncheridels/Utils/EasySceneSwitcher/Settings/OtherScenesSettings.asset";

    SerializedObject so;
    ReorderableList list;
    OtherScenesSettings settings;

    [MenuItem("Tools/Easy Scene Switcher/Manage Other Scenes")]
    static void OpenWindow()
    {
        GetWindow<OtherScenesSettingsWindow>("Other Scenes").minSize = new Vector2(400, 300);
    }

    void OnEnable()
    {
        // Ensure the Settings folder exists
        string folder = Path.GetDirectoryName(k_AssetPath);
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        // Load or create the ScriptableObject at the new path
        settings = AssetDatabase.LoadAssetAtPath<OtherScenesSettings>(k_AssetPath);
        if (settings == null)
        {
            settings = CreateInstance<OtherScenesSettings>();
            AssetDatabase.CreateAsset(settings, k_AssetPath);
            AssetDatabase.SaveAssets();
        }

        so = new SerializedObject(settings);
        var prop = so.FindProperty("scenes");

        list = new ReorderableList(so, prop, true, true, true, true)
        {
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Other Scenes"),
            drawElementCallback = (rect, index, active, focused) =>
            {
                var element = prop.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    element, GUIContent.none);
            }
        };
    }

    void OnGUI()
    {
        so.Update();
        list.DoLayoutList();

        if (so.hasModifiedProperties)
        {
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(settings);
        }
    }
}
#endif
