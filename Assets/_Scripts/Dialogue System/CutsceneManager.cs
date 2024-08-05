using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    private PlayableDirector playableDirector;
    private DialogueManager dialogueSystem;
    private bool cutscenePlaying = true;
    public bool CutscenePlaying {
        get => cutscenePlaying;
        set => cutscenePlaying = value;
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
}
