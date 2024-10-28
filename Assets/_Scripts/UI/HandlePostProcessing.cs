using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HandlePostProcessing : MonoBehaviour
{
    public static HandlePostProcessing Instance { get; private set; }

    [SerializeField] private Volume blurVolume;
    [SerializeField] private Volume GrungeVolume;
    [SerializeField] private Volume darkenVolume;

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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        blurVolume.enabled = GrungeVolume.enabled = true;
    }

    public void EnablePostProcessing()
    {
        blurVolume.enabled = true;
    }

    public void DisablePostProcessing()
    {
        if (DialogueManager.Instance.IsAutoSequence) return;
        blurVolume.enabled = false;
    }

    public void ToggleDarkenVolume(bool value)
    {
        darkenVolume.enabled = value;
    }

}
