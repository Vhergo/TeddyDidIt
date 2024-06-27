using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class DialogueImporter : EditorWindow
{
    private string csvFilePath = "Assets/Narrative/DialogueList.csv"; // Adjusted path

    [MenuItem("Tools/Dialogue Importer")]
    public static void ShowWindow()
    {
        GetWindow<DialogueImporter>("Dialogue Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Import Dialogue from CSV", EditorStyles.boldLabel);

        if (GUILayout.Button("Import Dialogues")) {
            ImportDialogues();
        }
    }

    private void ImportDialogues()
    {
        // Ensure the file exists
        if (!File.Exists(csvFilePath)) {
            Debug.LogError($"File not found at {csvFilePath}");
            return;
        }

        var lines = File.ReadAllLines(csvFilePath);
        for (int i = 0; i < lines.Length; i++)
        {
            var fields = ParseCsvLine(lines[i]);
            if (fields.Length < 3) continue;

            int orderNumber = int.Parse(fields[0]);
            string speakerName = fields[1];
            string dialogueText = fields[2];

            UpdateOrCreateDialogue(orderNumber, speakerName, dialogueText);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Dialogue import completed!");
    }

    private string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;

        foreach (var c in line) {
            if (c == ',' && !inQuotes) {
                result.Add(current.ToString());
                current.Clear();
            } else if (c == '"') {
                inQuotes = !inQuotes;
            } else {
                current.Append(c);
            }
        }

        result.Add(current.ToString());
        return result.ToArray();
    }

    private void UpdateOrCreateDialogue(int orderNumber, string speakerName, string dialogueText)
    {
        string assetDirectory = "Assets/Narrative/Dialogues";
        string assetPath = $"{assetDirectory}/Dialogue {orderNumber}.asset";

        Dialogue existingDialogue = AssetDatabase.LoadAssetAtPath<Dialogue>(assetPath);

        if (existingDialogue != null) {
            existingDialogue.speaker = Enum.TryParse(speakerName, out Speaker speaker) ? speaker : Speaker.Narration;
            existingDialogue.dialogueText = dialogueText;
            EditorUtility.SetDirty(existingDialogue);
        } else {
            Dialogue newDialogue = ScriptableObject.CreateInstance<Dialogue>();
            newDialogue.orderNumber = orderNumber;
            newDialogue.speaker = Enum.TryParse(speakerName, out Speaker speaker) ? speaker : Speaker.Narration;
            newDialogue.dialogueText = dialogueText;

            if (!Directory.Exists(assetDirectory)) {
                Directory.CreateDirectory(assetDirectory);
            }

            AssetDatabase.CreateAsset(newDialogue, assetPath);
        }
    }
}
