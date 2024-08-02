using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LegoColors))]
public class LegoColorsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LegoColors legoColors = (LegoColors)target;

        if (GUILayout.Button("Set Random Lego Colors")) {
            legoColors.SetRandomLegoColors();
        }

        if (GUILayout.Button("Remove All Colors")) {
            legoColors.RemoveAllColors();
        }
    }
}
