using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScoreSystem))]
public class ScoreSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ScoreSystem scoreSystem = (ScoreSystem)target;
        if (GUILayout.Button("Add Score")) {
            scoreSystem.AddScore();
        }
    }
}
