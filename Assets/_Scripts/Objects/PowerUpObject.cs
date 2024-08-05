using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpObject : MonoBehaviour
{
    [SerializeField] private float amplification = 10f;
    [SerializeField] private List<Dialogue> powerUpDialogue;

    private void SetPowerUpDialogue()
    {
        int randomIndex = Random.Range(0, powerUpDialogue.Count);
        StartCoroutine(DialogueManager.Instance.SetDelayedDialogue(powerUpDialogue[randomIndex]));
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player") {
            Debug.Log("POWER UP PICKED UP");
            SetPowerUpDialogue();
            CombatSystem.Instance.PowerUp(amplification);
            Destroy(gameObject);
        }
    }
}
