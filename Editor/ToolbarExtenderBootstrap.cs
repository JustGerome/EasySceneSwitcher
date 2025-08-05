// Editor/ToolbarExtenderBootstrap.cs
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

[InitializeOnLoad]
static class ToolbarExtenderBootstrap
{
    // The exact package name and Git URL you want to install
    const string pkgName = "com.marijnzwemmer.unity-toolbar-extender";
    const string pkgGitUrl = "https://github.com/marijnz/unity-toolbar-extender.git#v1.4.2";

    static ListRequest listRequest;
    static AddRequest addRequest;

    static ToolbarExtenderBootstrap()
    {
        // Kick off a list() to see which packages are already installed
        listRequest = Client.List(true, true);
        EditorApplication.update += ProgressList;
    }

    static void ProgressList()
    {
        if (!listRequest.IsCompleted) return;
        EditorApplication.update -= ProgressList;

        // If the toolbar-extender package is not found, prompt the user
        bool found = false;
        foreach (var p in listRequest.Result)
            if (p.name == pkgName) { found = true; break; }

        if (!found)
            EditorApplication.delayCall += () => EditorWindow.GetWindow<InstallExtenderWindow>();
    }

    // A simple popup window
    class InstallExtenderWindow : EditorWindow
    {
        void OnEnable()
        {
            titleContent = new GUIContent("Install Dependency");
            minSize = new Vector2(350, 100);
        }

        void OnGUI()
        {
            GUILayout.Label("The toolbar-extender package is not installed.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);
            if (GUILayout.Button("Install toolbar-extender now"))
            {
                addRequest = Client.Add(pkgGitUrl);
                EditorApplication.update += ProgressAdd;
            }
        }

        void ProgressAdd()
        {
            if (!addRequest.IsCompleted) return;
            EditorApplication.update -= ProgressAdd;

            if (addRequest.Status == StatusCode.Success)
                Debug.Log($"Installed {pkgName} successfully.");
            else
                Debug.LogError($"Failed to install {pkgName}: {addRequest.Error.message}");

            Close();
        }
    }
}
