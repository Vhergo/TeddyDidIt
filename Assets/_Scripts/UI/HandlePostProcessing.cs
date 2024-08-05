using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HandlePostProcessing : MonoBehaviour
{
    [SerializeField] private Volume ppVolume;

    private void Awake()
    {
        ppVolume = GetComponent<Volume>();
    }

    private void OnEnable()
    {
        DialogueManager.OnAutoSequenceStarted += EnablePostProcessing;
        DialogueManager.OnAutoSequenceEnded += DisablePostProcessing;
    }

    private void OnDisable()
    {
        DialogueManager.OnAutoSequenceStarted -= EnablePostProcessing;
        DialogueManager.OnAutoSequenceEnded -= DisablePostProcessing;
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
