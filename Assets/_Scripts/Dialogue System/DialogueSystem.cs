using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    // Singleton for quick access
    public static DialogueSystem Instance { get; private set; }

    public static Action OnDialogueChange;

    [SerializeField] private List<Dialogue> dialogues = new List<Dialogue>();

    public enum Side { Left, Right }

    [Header("Dialogue UI")]
    [SerializeField] private GameObject mom;
    [SerializeField] private GameObject dad;
    [SerializeField] private GameObject timmy;
    private GameObject currentSpeaker;

    [SerializeField] private TMP_Text dialogueText;

    [Header("Characters")]
    [SerializeField] private string momName;
    [SerializeField] private string dadName;
    [SerializeField] private string kidName;

    private Dialogue currentDialogue;
    private int currentDialogueIndex = 0;
    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeSpeakers();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5)) NextDialogue();
        if (Input.GetKeyDown(KeyCode.Alpha6)) PreviousDialogue();
        if (Input.GetKeyDown(KeyCode.Alpha7)) StartDialogue();
    }

    private void InitializeSpeakers()
    {
        print("Initializing speakers");
        mom.SetActive(false);
        dad.SetActive(false);
        timmy.SetActive(false);
        dialogueText.text = "...";
        dialogueText.horizontalAlignment = HorizontalAlignmentOptions.Center;
    }

    private void UpdateDialogueUI(Dialogue dialogue)
    {
        SetSpeakerName(dialogue.speaker);
        dialogueText.text = dialogue.dialogueText;
    }

    public void StartDialogue()
    {
        SetDialogue(GetDialogue(currentDialogueIndex));
        UpdateDialogueUI(currentDialogue);
    }

    public void NextDialogue()
    {
        if (currentDialogueIndex < dialogues.Count - 1) {
            SetDialogue(GetDialogue(++currentDialogueIndex));
        }else {
            Debug.Log("End of dialogue");
        }
    }

    public void PreviousDialogue()
    {
        if (currentDialogueIndex > 0) {
            SetDialogue(GetDialogue(--currentDialogueIndex));
        }else {
            Debug.Log("Start of dialogue");
        }
    }

    public void SetDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        UpdateDialogueUI(currentDialogue);
    }

    private Dialogue GetDialogue(int index) => dialogues[index];

    private void SetSpeakerName(Speaker speaker)
    {
        switch (speaker) {
            case Speaker.Mother:
                dad.SetActive(false);
                timmy.SetActive(false);

                mom.SetActive(true);
                mom.transform.GetChild(0).GetComponent<TMP_Text>().text = momName;
                dialogueText.horizontalAlignment = HorizontalAlignmentOptions.Right;
                break;
            case Speaker.Father:
                mom.SetActive(false);
                timmy.SetActive(false);

                dad.SetActive(true);
                dad.transform.GetChild(0).GetComponent<TMP_Text>().text = dadName;
                dialogueText.horizontalAlignment = HorizontalAlignmentOptions.Right;
                break;
            case Speaker.Timmy:
                mom.SetActive(false);
                dad.SetActive(false);

                timmy.SetActive(true);
                timmy.transform.GetChild(0).GetComponent<TMP_Text>().text = kidName;
                dialogueText.horizontalAlignment = HorizontalAlignmentOptions.Left;
                break;
        }
    }
}
