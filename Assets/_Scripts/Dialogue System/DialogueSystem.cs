using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    // Singleton for quick access
    public static DialogueSystem Instance { get; private set; }

    public static Action OnDialogueChange;

    [Header("Sequences")]
    // Gameplay Sequences
    [SerializeField] private DialogueSequence gameplaySequence; // Order: 4
    [SerializeField] private DialogueSequence castleSequence; // Order: 5

    // Auto Sequences
    [Space(10)]
    [SerializeField] private DialogueSequence kitchenSequence; // Order: 1
    [SerializeField] private DialogueSequence bedroomSequence; // Order: 2
    [SerializeField] private DialogueSequence introSequence; // Order: 3
    [SerializeField] private DialogueSequence postBossSequence; // Order: 6
    [SerializeField] private DialogueSequence endingSequence; // Order: 7

    private DialogueSequence currentSequence;
    private bool isAutoSequence = false;

    [SerializeField] private List<Dialogue> dialogues = new List<Dialogue>();

    [Header("Speaker UI")]
    [SerializeField] private List<SpeakerInfo> speakerInfo;

    [SerializeField] private GameObject speakerIcon;
    [SerializeField] private RectTransform speakerRect;

    [SerializeField] private TMP_Text speakerName;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject currentSpeaker;
    [SerializeField] Dialogue currentDialogue;
    private int dialogueIndex = 0;
    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Debug.Log("ORIGINAL POS: "+ speakerRect.localPosition);
        InitializeSpeakers();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) NextDialogue();
        if (Input.GetKeyDown(KeyCode.K)) PreviousDialogue();
        if (Input.GetKeyDown(KeyCode.L)) StartDialogue();
    }

    private void InitializeSpeakers()
    {
        Debug.Log("Initializing speakers");

        speakerName.text = "...";
        dialogueText.text = "...";
        dialogueText.horizontalAlignment = HorizontalAlignmentOptions.Center;

        UpdateSequence(kitchenSequence);
    }

    private void UpdateSequence(DialogueSequence sequence)
    {
        if (sequence == null) {
            Debug.Log("ALL SEQUENCES ARE COMPLETE");
            return;
        }

        currentSequence = sequence;
        isAutoSequence = sequence.autoplay;

        dialogues.Clear();
        dialogues = sequence.dialogueSequence;
        dialogueIndex = 0;
        Debug.Log("DIALOGUE COUNT: " + dialogues.Count);

        if (isAutoSequence) StartCoroutine(StartAutoSequence());
    }

    private IEnumerator StartAutoSequence()
    {
        Debug.Log("DIALOGUE COUNT: " + dialogues.Count);
        while (dialogueIndex < dialogues.Count) {
            Debug.Log("PLAY DIALOGUE");
            Dialogue dialogue = GetDialogue(dialogueIndex);
            SetDialogue(dialogue);
            yield return new WaitForSeconds(dialogue.autoplayDuration);
            dialogueIndex++;
        }
        Debug.Log("Sequence is over");
        UpdateSequence(currentSequence.nextSequence);
    }

    public void StartDialogue()
    {
        SetDialogue(GetDialogue(dialogueIndex));
        UpdateDialogueUI(currentDialogue);
    }

    public void UpdateDialogueUI(Dialogue dialogue)
    {
        SetSpeaker(dialogue.speaker);
        dialogueText.text = dialogue.dialogueText;
    }

    public void NextDialogue()
    {
        if (dialogueIndex < dialogues.Count - 1) {
            SetDialogue(GetDialogue(++dialogueIndex));
        }else {
            Debug.Log("End of dialogue");
        }
    }

    public void PreviousDialogue()
    {
        if (dialogueIndex > 0) {
            SetDialogue(GetDialogue(--dialogueIndex));
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

    private void SetSpeaker(Speaker speaker)
    {
        SpeakerInfo info = speakerInfo.Find(s => s.speaker == speaker);

        speakerName.text = info.speaker.ToString();
        speakerName.horizontalAlignment = info.horizontalAlignment;
        dialogueText.horizontalAlignment = info.horizontalAlignment;

        UpdateSpeakerIcon(info);
    }

    private void UpdateSpeakerIcon(SpeakerInfo info)
    {
        Debug.Log("OLD POS: " + speakerRect.localPosition);
        Vector3 newPos = new Vector3(info.xPos, speakerRect.localPosition.y, 0);
        Debug.Log("NEW POS: " + newPos);
        speakerRect.localPosition = newPos;

        Image speakerImage = speakerRect.GetComponent<Image>();
        speakerImage.sprite = info.speakerImage;
    }
}

[Serializable]
public class SpeakerInfo
{
    public string speakerName;
    public Speaker speaker;
    public Sprite speakerImage;
    public HorizontalAlignmentOptions horizontalAlignment;
    public float xPos;
}
