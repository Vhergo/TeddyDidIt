using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;

public class DialogueManager : MonoBehaviour
{
    // Singleton for quick access
    public static DialogueManager Instance { get; private set; }

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

    [SerializeField] private DialogueSequence currentSequence;
    private bool isAutoSequence = false;
    public bool IsAutoSequence => isAutoSequence;

    private bool allTextIsDisplayed = false;

    // Dialogue
    [SerializeField] private Dialogue currentDialogue;
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private float textSkipSpeed = 0.001f;
    [SerializeField] private int dialogueIndex = 0;
    private float originalTextSpeed;
    private Coroutine dialogueCoroutine;

    [SerializeField] private List<Dialogue> dialogues = new List<Dialogue>();

    [Header("Speaker UI")]
    [SerializeField] private List<SpeakerInfo> speakerInfo;

    [SerializeField] private GameObject speakerIcon;
    [SerializeField] private RectTransform speakerRect;
    [SerializeField] private RectTransform speakerFull;
    [SerializeField] private PositionInfo leftSide;
    [SerializeField] private PositionInfo rightSide;

    [SerializeField] private TMP_Text speakerName;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text continueDialogueText;
    [SerializeField] private GameObject skipAutoSequenceDisplay;

    [Space(10)]
    [SerializeField] private AudioSource typingSource;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private AudioClip outroMusic;

    public static Action<Speaker> OnSpeakerChanged;
    public static Action<DialogueSequence> OnSequenceChange;
    public static Action OnAutoSequenceStarted;
    public static Action OnAutoSequenceEnded;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        originalTextSpeed = textSpeed;
        InitializeSpeakers();

        typingSource = GetComponent<AudioSource>();
        typingSource.clip = typingSound;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.K)) NextDialogue();
        //if (Input.GetKeyDown(KeyCode.L)) PreviousDialogue();

        //if (Input.GetKeyDown(KeyCode.J)) SkipSequence();

        if (Input.GetKeyDown(KeyCode.Space) && !allTextIsDisplayed && currentSequence.autoplay) StartCoroutine(SkipTyping());
    }

    #region DIALOGUE SYSTEM
    private void InitializeSpeakers()
    {
        Debug.Log("Initializing speakers");

        speakerName.text = "";
        dialogueText.text = "";
        dialogueText.horizontalAlignment = HorizontalAlignmentOptions.Center;
        continueDialogueText.gameObject.SetActive(false);
    }

    #region SEQUENCES
    // Call this from the timeline
    public void StartDialogueSystem()
    {
        Debug.Log("STARTING DIALOGUE SYSTEM");
        UpdateSequence(kitchenSequence);
    }

    public void SkipSequence()
    {
        if (currentSequence.nextSequence == null) {
            Debug.Log("ALL SEQUENCES ARE COMPLETE");
            SoundManager.Instance.TriggerSwitchMusic(outroMusic);
            GameManager.Instance.GameOver();
            return;
        }

        StopAllCoroutines();
        if (!currentSequence.nextSequence.autoplay) {
            OnAutoSequenceEnded?.Invoke();
            UseFullVisual(false);
        }
        UpdateSequence(currentSequence.nextSequence);
    }

    private void UpdateSequence(DialogueSequence sequence)
    {
        if (sequence == null) {
            Debug.Log("ALL SEQUENCES ARE COMPLETE");
            return;
        }

        currentSequence = sequence;
        isAutoSequence = sequence.autoplay;

        dialogues = sequence.dialogueSequence;
        dialogueIndex = 0;

        OnSequenceChange?.Invoke(sequence);
        if (isAutoSequence) {
            // Disable player input HERE
            StartCoroutine(StartAutoSequence());
            TeddyMovement.Instance.Freeze();
        } else {
            // Enable player input HERE
            StartGameplaySequence();
            TeddyMovement.Instance.Unfreeze();
        }
    }

    private IEnumerator StartAutoSequence()
    {
        OnAutoSequenceStarted?.Invoke();
        UseFullVisual(true);

        while (dialogueIndex < dialogues.Count) {
            Dialogue dialogue = GetDialogue(dialogueIndex);
            SetDialogue(dialogue);
            // OnSpeakerChanged?.Invoke(dialogue.speaker); // Obsolete
            yield return new WaitUntil(() => allTextIsDisplayed);

            // Have some UI indication to press continue here
            continueDialogueText.gameObject.SetActive(true);

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

            dialogueIndex++;
            continueDialogueText.gameObject.SetActive(false);
        }
        Debug.Log("Sequence is over");

        if (currentSequence.nextSequence != null) {
            if (!currentSequence.nextSequence.autoplay) {
                OnAutoSequenceEnded?.Invoke();
                UseFullVisual(false);
            }
            UpdateSequence(currentSequence.nextSequence);
        }else {
            Debug.Log("ALL SEQUENCES ARE COMPLETE");
            SoundManager.Instance.TriggerSwitchMusic(outroMusic);
            GameManager.Instance.GameOver();
        }
    }

    private void UseFullVisual(bool fullVisual)
    {
        speakerFull.gameObject.SetActive(fullVisual);
        speakerIcon.gameObject.SetActive(!fullVisual);
    }

    private void StartGameplaySequence()
    {
        SetDialogue(GetDialogue(dialogueIndex));
        UpdateDialogueUI(currentDialogue);

        skipAutoSequenceDisplay.SetActive(false);
    }
    #endregion

    #region DIALOGUE

    public void SetDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        UpdateDialogueUI(currentDialogue);
    }

    public IEnumerator SetDelayedDialogue(Dialogue dialogue, float bonusDelay = 0.5f)
    {
        yield return new WaitUntil(() => allTextIsDisplayed);
        yield return new WaitForSeconds(bonusDelay);
        SetDialogue(dialogue);
    }

    public void NextDialogue(bool delay = false)
    {
        if (dialogueIndex < dialogues.Count - 1) {
            dialogueIndex++;
            if (delay) StartCoroutine(SetDelayedDialogue(GetDialogue(dialogueIndex), 1f));
            else SetDialogue(GetDialogue(dialogueIndex));
        } else {
            Debug.Log("End of Sequence");
            UpdateSequence(currentSequence.nextSequence);
        }
    }

    public void PreviousDialogue()
    {
        if (dialogueIndex > 0) {
            dialogueIndex--;
            SetDialogue(GetDialogue(dialogueIndex));
        } else {
            Debug.Log("Start of Sequence");
        }
    }

    private Dialogue GetDialogue(int index) => dialogues[index];

    private void SetSpeaker(Speaker speaker)
    {
        SpeakerInfo info = speakerInfo.Find(s => s.speaker == speaker);

        speakerName.text = info.speaker.ToString();
        speakerName.horizontalAlignment = info.horizontalAlignment;
        dialogueText.horizontalAlignment = info.horizontalAlignment;
        continueDialogueText.horizontalAlignment = info.horizontalAlignment;

        UpdateSpeakerIcon(info, GetPositionInfo(info.horizontalAlignment));
    }
    #endregion

    #region UI
    public void UpdateDialogueUI(Dialogue dialogue)
    {
        SetSpeaker(dialogue.speaker);
        dialogueText.text = dialogue.dialogueText;

        if (dialogueCoroutine != null) StopCoroutine(dialogueCoroutine);
        dialogueCoroutine = StartCoroutine(AniamteDialogueText());
    }

    private IEnumerator AniamteDialogueText()
    {
        dialogueText.maxVisibleCharacters = 0;
        int totalVisibleCharacters = dialogueText.text.Length;
        int counter = 0;

        allTextIsDisplayed = false;
        while (counter <= totalVisibleCharacters) {
            dialogueText.maxVisibleCharacters = counter;
            counter++;
            PlayTypingSound();
            yield return new WaitForSeconds(textSpeed);
        }

        allTextIsDisplayed = true;
    }

    private void PlayTypingSound()
    {
        typingSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        typingSource.Play();
    }

    private IEnumerator SkipTyping()
    {
        textSpeed = textSkipSpeed;
        yield return new WaitUntil(() => allTextIsDisplayed);
        textSpeed = originalTextSpeed;
    }

    private void UpdateSpeakerIcon(SpeakerInfo info, PositionInfo positionInfo)
    {
        Vector3 newPos = new Vector3(positionInfo.iconXPos, speakerRect.localPosition.y, 0);
        speakerRect.localPosition = newPos;

        Image speakerImage = speakerRect.GetComponent<Image>();
        speakerImage.sprite = info.speakerIcon;

        if (currentSequence.autoplay) UpdateSpeakerFullVisual(info, positionInfo);
    }

    private void UpdateSpeakerFullVisual(SpeakerInfo info, PositionInfo positionInfo)
    {
        // Update full speaker visual sprite
        Image speakerImage = speakerFull.GetComponent<Image>();
        speakerImage.sprite = info.fullSprite;

        // Set dimensions for full speaker visual
        Vector3 newSize = new Vector3(info.fullSprite.rect.width, info.fullSprite.rect.height, 0);
        speakerFull.sizeDelta = newSize;

        // Set position for full speaker visual
        Vector3 newPos = new Vector3(positionInfo.visualXPos, positionInfo.visualYPos, 0);
        speakerFull.anchoredPosition = newPos;

        if (info.showFullVisual) speakerFull.gameObject.SetActive(true);
        else speakerFull.gameObject.SetActive(false);
    }

    private PositionInfo GetPositionInfo(HorizontalAlignmentOptions option)
    {
        if (option == HorizontalAlignmentOptions.Left) {
            speakerFull.anchorMin = new Vector2(0, 0);
            speakerFull.anchorMax = new Vector2(0, 0);
            speakerFull.pivot = new Vector2(0, 0);
            return leftSide;
        }else {
            speakerFull.anchorMin = new Vector2(1, 0);
            speakerFull.anchorMax = new Vector2(1, 0);
            speakerFull.pivot = new Vector2(1, 0);
            return rightSide;
        }
    }
    #endregion

    #endregion
}

[Serializable]
public class SpeakerInfo
{
    public string speakerName;
    public Speaker speaker;
    public Sprite speakerIcon;
    public Sprite fullSprite;
    public HorizontalAlignmentOptions horizontalAlignment;
    public bool showFullVisual;
}

[Serializable]
public class PositionInfo
{
    public float iconXPos;
    public float visualXPos;
    public float visualYPos;
}
