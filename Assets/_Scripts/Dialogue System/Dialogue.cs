using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    public int orderNumber;
    public Speaker speaker;
    [TextArea(3, 10)]
    public string dialogueText;
}