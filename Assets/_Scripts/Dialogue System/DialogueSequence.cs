using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Sequence", menuName = "Dialogue System/Dialogue Sequence")]
public class DialogueSequence : ScriptableObject
{
    public string sequenceName = string.Empty;
    public bool autoplay = false;
    public List<Dialogue> dialogueSequence = new List<Dialogue>();
    public DialogueSequence nextSequence;
    public Sprite background;
    public AudioClip backgroundMusic;
}