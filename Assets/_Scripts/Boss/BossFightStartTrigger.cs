using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightStartTrigger : MonoBehaviour
{
    [SerializeField] private GameObject restartBossFightButton;

    private void Start()
    {
        restartBossFightButton.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            // Trigger Boss Cutscene
            BossFightManager.Instance.BeginBossFight();
            restartBossFightButton.SetActive(true);
        }
    }
}
