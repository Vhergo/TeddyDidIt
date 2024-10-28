using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossFightStartTrigger : MonoBehaviour
{
    [SerializeField] private GameObject backtrackBlock;
    [SerializeField] private PlayableAsset bossCutscene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            CutsceneManager.Instance.PlayCutscene(bossCutscene);
            backtrackBlock.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
