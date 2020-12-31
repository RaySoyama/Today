using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TodayManager))]
public class TodayManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TodayManager data = (TodayManager)target;
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Get Today Data"))
            {
                data.GetTodayData();
            }

            if (GUILayout.Button("Save Today Data"))
            {
                data.SaveTodayData();
            }
        }
        GUILayout.EndHorizontal();
    }
}
