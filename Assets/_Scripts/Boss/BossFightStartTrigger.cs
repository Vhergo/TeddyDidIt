using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightStartTrigger : MonoBehaviour
{
    [SerializeField] private GameObject restartBossFightButton;
    [SerializeField] private GameObject backtrackBlock;

    private void Start()
    {
        restartBossFightButton.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            CutsceneManager.Instance.PlayBossCutscene();
            restartBossFightButton.SetActive(true);
            backtrackBlock.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
