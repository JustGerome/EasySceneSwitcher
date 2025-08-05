#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;
using System.IO;
using System.Collections.Generic;

[InitializeOnLoad]
public static class EasySceneSwitcher
{
    // Paths of scenes in Build Settings
    static List<string> buildScenePaths;
    static List<string> buildSceneNames;

    // Manually managed "Other Scenes"
    static List<string> otherScenePaths;
    static List<string> otherSceneNames;

    // File to persist Other Scenes list
    static string prefsFilePath;

    [System.Serializable]
    class OtherScenesData
    {
        public List<string> scenes;
    }

    static EasySceneSwitcher()
    {
        // Determine prefs file under Library
        prefsFilePath = Path.GetFullPath(
            Path.Combine(Application.dataPath, "..", "Library", "EasySceneSwitcher.json")
        );

        // Hook into the toolbar and assembly reload
        ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        AssemblyReloadEvents.afterAssemblyReload += RefreshSceneLists;

        // Initial population
        RefreshSceneLists();
    }

    static void RefreshSceneLists()
    {
        // 1) Load build scenes
        buildScenePaths = new List<string>();
        buildSceneNames = new List<string>();
        foreach (var s in EditorBuildSettings.scenes)
        {
            buildScenePaths.Add(s.path);
            buildSceneNames.Add(Path.GetFileNameWithoutExtension(s.path));
        }

        // 2) Load manually tracked other scenes
        LoadOtherScenesData();
    }

    static void LoadOtherScenesData()
    {
        otherScenePaths = new List<string>();
        otherSceneNames = new List<string>();

        if (!File.Exists(prefsFilePath))
            return;

        try
        {
            string json = File.ReadAllText(prefsFilePath);
            var data = JsonUtility.FromJson<OtherScenesData>(json);
            if (data != null && data.scenes != null)
            {
                foreach (var path in data.scenes)
                {
                    // Only include if not now in build settings
                    if (buildScenePaths.Contains(path))
                        continue;

                    // Verify file still exists
                    var full = Path.GetFullPath(Path.Combine(Application.dataPath, "..", path));
                    if (!File.Exists(full))
                        continue;

                    otherScenePaths.Add(path);
                    otherSceneNames.Add(Path.GetFileNameWithoutExtension(path));
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("EasySceneSwitcher: Failed to load Other Scenes list: " + ex.Message);
        }
    }

    static void SaveOtherScenesData()
    {
        var data = new OtherScenesData { scenes = otherScenePaths };
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(prefsFilePath));
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(prefsFilePath, json);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("EasySceneSwitcher: Failed to save Other Scenes list: " + ex.Message);
        }
    }

    static void OnToolbarGUI()
    {
        GUILayout.Space(10);
        var activeScene = EditorSceneManager.GetActiveScene();
        string currentName = Path.GetFileNameWithoutExtension(activeScene.path);

        if (EditorGUILayout.DropdownButton(
                new GUIContent(currentName),
                FocusType.Passive,
                EditorStyles.toolbarPopup,
                GUILayout.Width(150)))
        {
            var menu = new GenericMenu();

            // -- Build Scenes --
            menu.AddDisabledItem(new GUIContent("Build Scenes"));
            for (int i = 0; i < buildSceneNames.Count; i++)
            {
                bool isCurrent = buildScenePaths[i] == activeScene.path;
                menu.AddItem(
                    new GUIContent(buildSceneNames[i]),
                    isCurrent,
                    new GenericMenu.MenuFunction2(OpenScene),
                    buildScenePaths[i]
                );
            }

            // -- Other Scenes --
            if (otherSceneNames.Count > 0)
            {
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("Other Scenes"));
                for (int i = 0; i < otherSceneNames.Count; i++)
                {
                    menu.AddItem(
                        new GUIContent(otherSceneNames[i]),
                        false,
                        new GenericMenu.MenuFunction2(OpenScene),
                        otherScenePaths[i]
                    );
                }
            }

            // -- Add Other Scene --
            menu.AddSeparator("");
            menu.AddItem(
                new GUIContent("Add Other Scene"),
                false,
                new GenericMenu.MenuFunction(AddOtherScene)
            );

            menu.DropDown(GUILayoutUtility.GetLastRect());
        }
    }

    static void OpenScene(object userData)
    {
        string scenePath = userData as string;
        if (string.IsNullOrEmpty(scenePath)) return;

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(scenePath);
    }

    static void AddOtherScene()
    {
        // Pick a scene file
        string abs = EditorUtility.OpenFilePanel(
            "Select Scene to Track as 'Other'",
            Application.dataPath,
            "unity"
        );
        if (string.IsNullOrEmpty(abs)) return;

        if (!abs.StartsWith(Application.dataPath))
        {
            Debug.LogWarning("Please select a scene within the Assets folder.");
            return;
        }

        // Convert to project-relative
        string projPath = "Assets" + abs.Substring(Application.dataPath.Length);

        // Skip if it's in build settings
        if (buildScenePaths.Contains(projPath))
        {
            Debug.Log("Scene is already in Build Settings.");
            return;
        }

        // Add if not already tracked
        if (!otherScenePaths.Contains(projPath))
        {
            otherScenePaths.Add(projPath);
            otherSceneNames.Add(Path.GetFileNameWithoutExtension(projPath));
            SaveOtherScenesData();
        }
    }
}
#endif