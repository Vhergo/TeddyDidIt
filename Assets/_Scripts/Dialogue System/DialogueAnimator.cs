using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAnimator : MonoBehaviour
{
    public static DialogueAnimator Instance { get; private set; }

    [SerializeField] private Animator timmy;
    [SerializeField] private Animator mother;
    [SerializeField] private Animator father;
    [SerializeField] private Animator teddy;
    [SerializeField] private Animator king;
    [SerializeField] private Animator narrator;

    private Speaker currentSpeaker;

    private Dictionary<Speaker, Animator> speakerDict;

    public enum AnimationState
    {
        IsSpeaking,
        SpeakingTransition,
        NotSpeaking,
        NotSpeakingTransition
    }

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        speakerDict = new Dictionary<Speaker, Animator> {
            { Speaker.Timmy, timmy },
            { Speaker.Mother, mother },
            { Speaker.Father, father },
            { Speaker.Teddy, teddy },
            { Speaker.King, king },
            { Speaker.Narrator, narrator }
        };
    }

    private void OnEnable()
    {
        DialogueManager.OnSpeakerChanged += TriggerSpeakerChange;
        DialogueManager.OnAutoSequenceEnded += StopCurrentSpeaker;
    }

    private void OnDisable()
    {
        DialogueManager.OnSpeakerChanged -= TriggerSpeakerChange;
        DialogueManager.OnAutoSequenceEnded -= StopCurrentSpeaker;
    }

    private void TriggerSpeakerChange(Speaker speaker)
    {
        Debug.Log("TRIGGERED SPEAKER CHANGE");
        StartCoroutine(ChangeSpeaker(speaker));
    }

    private IEnumerator ChangeSpeaker(Speaker speaker)
    {
        if (currentSpeaker == speaker) yield break;
        else speakerDict[currentSpeaker].SetTrigger("StopSpeaking");

        speakerDict[speaker].SetTrigger("StartSpeaking");
        currentSpeaker = speaker;
    }

    private void StopCurrentSpeaker()
    {
        Debug.Log("THIS IS CAUSING THE TIMMY ISSUE");
        speakerDict[currentSpeaker].SetTrigger("StopSpeaking");
    }
}
