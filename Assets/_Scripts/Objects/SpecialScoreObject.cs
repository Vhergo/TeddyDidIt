using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialScoreObject : MonoBehaviour
{
    [SerializeField] private List<Dialogue> specialScoreDialogue;

    private void SetSpecialScoreDialogue()
    {
        int randomIndex = Random.Range(0, specialScoreDialogue.Count);
        StartCoroutine(DialogueManager.Instance.SetDelayedDialogue(specialScoreDialogue[randomIndex]));
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player") {
            Debug.Log("SPECIAL SCORE PICKED UP");
            SetSpecialScoreDialogue();
            ScoreSystem.Instance.SpecialPickup();
            Destroy(gameObject);
        }
    }
}
