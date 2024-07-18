using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;

    private void OnValidate()
    {
        if (dialogue != null) gameObject.name = dialogue.name + " Trigger";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            DialogueSystem.Instance.SetDialogue(dialogue);
            Destroy(gameObject);
        }
    }
}
