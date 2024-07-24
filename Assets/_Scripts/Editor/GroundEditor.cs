using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ground))]
public class GroundEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Ground ground = (Ground)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Update Planks")) {
            ground.UpdatePlanks();
        }

        if (GUILayout.Button("Remove Planks")) {
            ground.RemovePlanks();
        }
    }
}
