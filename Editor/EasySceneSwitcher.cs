#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;
using System.Collections.Generic;

[InitializeOnLoad]
public static class EasySceneSwitcher
{
    static List<string> buildScenePaths;
    static List<string> buildSceneNames;

    static List<string> otherScenePaths;
    static List<string> otherSceneNames;

    static EasySceneSwitcher()
    {
        // Hook toolbar dropdown
        ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);

        // Refresh after compile/assembly reload
        AssemblyReloadEvents.afterAssemblyReload += RefreshSceneLists;

        // Refresh when assets change
        EditorApplication.projectChanged += RefreshSceneLists;
        Undo.undoRedoPerformed += RefreshSceneLists;

        // Initial population
        RefreshSceneLists();
    }

    static void RefreshSceneLists()
    {
        // 1) Build scenes
        buildScenePaths = new List<string>();
        buildSceneNames = new List<string>();
        foreach (var s in EditorBuildSettings.scenes)
        {
            buildScenePaths.Add(s.path);
            buildSceneNames.Add(System.IO.Path.GetFileNameWithoutExtension(s.path));
        }

        // 2) Other scenes from ScriptableObject
        var settings = AssetDatabase.LoadAssetAtPath<OtherScenesSettings>(
            "Assets/Moncheridels/Utils/EasySceneSwitcher/Settings/OtherScenesSettings.asset");

        otherScenePaths = new List<string>();
        otherSceneNames = new List<string>();

        if (settings != null)
        {
            foreach (var sceneAsset in settings.scenes)
            {
                string path = AssetDatabase.GetAssetPath(sceneAsset);
                if (string.IsNullOrEmpty(path) || buildScenePaths.Contains(path))
                    continue;

                otherScenePaths.Add(path);
                otherSceneNames.Add(System.IO.Path.GetFileNameWithoutExtension(path));
            }
        }
    }

    static void OnToolbarGUI()
    {
        // Always refresh just before drawing
        RefreshSceneLists();

        GUILayout.Space(10);
        var activeScene = EditorSceneManager.GetActiveScene();
        string currentName = System.IO.Path.GetFileNameWithoutExtension(activeScene.path);

        if (EditorGUILayout.DropdownButton(
                new GUIContent(currentName),
                FocusType.Passive,
                EditorStyles.toolbarPopup,
                GUILayout.Width(150)))
        {
            var menu = new GenericMenu();

            // Build Scenes
            menu.AddDisabledItem(new GUIContent("Build Scenes"));
            for (int i = 0; i < buildSceneNames.Count; i++)
            {
                menu.AddItem(
                    new GUIContent(buildSceneNames[i]),
                    buildScenePaths[i] == activeScene.path,
                    OpenScene,
                    buildScenePaths[i]
                );
            }

            // Other Scenes
            if (otherSceneNames.Count > 0)
            {
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("Other Scenes"));
                for (int i = 0; i < otherSceneNames.Count; i++)
                {
                    menu.AddItem(
                        new GUIContent(otherSceneNames[i]),
                        false,
                        OpenScene,
                        otherScenePaths[i]
                    );
                }
            }

            // Manage Other Scenes
            menu.AddSeparator("");
            menu.AddItem(
                new GUIContent("Manage Other Scenes"),
                false,
                () => EditorWindow.GetWindow<OtherScenesSettingsWindow>()
            );

            menu.DropDown(GUILayoutUtility.GetLastRect());
        }
    }

    static void OpenScene(object userData)
    {
        string scenePath = userData as string;
        if (string.IsNullOrEmpty(scenePath))
            return;

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(scenePath);
    }
}
#endif
