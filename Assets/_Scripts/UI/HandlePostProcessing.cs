using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HandlePostProcessing : MonoBehaviour
{
    [SerializeField] private Volume ppVolume;
    [SerializeField] private Volume ppVolume2;

    private void OnEnable()
    {
        DialogueManager.OnAutoSequenceStarted += EnablePostProcessing;
        DialogueManager.OnAutoSequenceEnded += DisablePostProcessing;

        GameUIManager.OnMenuOpen += EnablePostProcessing;
        GameUIManager.OnMenuClose += DisablePostProcessing;
    }

    private void OnDisable()
    {
        DialogueManager.OnAutoSequenceStarted -= EnablePostProcessing;
        DialogueManager.OnAutoSequenceEnded -= DisablePostProcessing;

        GameUIManager.OnMenuOpen -= EnablePostProcessing;
        GameUIManager.OnMenuClose -= DisablePostProcessing;
    }

    private void Start()
    {
        ppVolume.enabled = ppVolume2.enabled = true;
    }

    private void EnablePostProcessing()
    {
        ppVolume.enabled = true;
    }

    private void DisablePostProcessing()
    {
        ppVolume.enabled = false;
    }

}
