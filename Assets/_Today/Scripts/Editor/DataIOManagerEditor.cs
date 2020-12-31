using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(DataIOManager))]
public class DataIOManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DataIOManager data = (DataIOManager)target;
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Open Settings Folder"))
        {
            data.OpenSettingsFolder();
        }

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Open Data Folder"))
            {
                data.OpenDataFolder();
            }
        }
        GUILayout.EndHorizontal();
    }
}
