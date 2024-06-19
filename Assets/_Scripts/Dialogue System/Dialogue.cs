using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    public Speaker speakerName;
    [TextArea(3, 10)]
    public string dialogueText;
}

[Serializable]
public enum Speaker
{
    Mom,
    Dad,
    Kid
}