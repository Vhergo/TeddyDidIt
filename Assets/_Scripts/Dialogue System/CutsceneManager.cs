using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }

    [SerializeField] private PlayableAsset introCutscene;

    private PlayableDirector playableDirector;
    private DialogueManager dialogueSystem;
    private bool cutscenePlaying = true;

    public bool CutscenePlaying {
        get => cutscenePlaying;
        set => cutscenePlaying = value;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        // BossManager.OnBossDeath += PlayBossCutscene;
    }

    private void OnDisable()
    {
        // BossManager.OnBossDeath -= PlayBossCutscene;
    }   

    private void Start()
    {
        playableDirector = FindObjectOfType<PlayableDirector>();
        dialogueSystem = DialogueManager.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            if (cutscenePlaying) {
                SkipCutscene();
                cutscenePlaying = false;
            }else {
                SkipAutoSequence();
            }
        }
    }

    public void UpdateCutscene(PlayableAsset cutscene)
    {
        playableDirector.playableAsset = cutscene;
        playableDirector.Play();
    }

    public void SkipCutscene()
    {
        playableDirector.time = playableDirector.duration;
        playableDirector.Evaluate();
        playableDirector.Stop();
    }

    public void SkipAutoSequence()
    {
        if (dialogueSystem.IsAutoSequence) {
            dialogueSystem.SkipSequence();
        }
    }

    public void PlayCutscene(PlayableAsset cutscene)
    {
        if (cutscene != null)
            UpdateCutscene(cutscene);
    }
}
