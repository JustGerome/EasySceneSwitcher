#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(
    fileName = "OtherScenesSettings",
    menuName = "Easy Scene Switcher/Other Scenes Settings"
)]
public class OtherScenesSettings : ScriptableObject
{
    [Tooltip("Drag & drop scenes here")]
    public List<SceneAsset> scenes = new List<SceneAsset>();
}
#endif
