using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SequenceBackgroundManager : MonoBehaviour
{
    public static SequenceBackgroundManager Instance;

    [SerializeField] private Image primaryBackground;
    [SerializeField] private Image secondaryBackground;
    [SerializeField] private float transitionSpeed = 1f;

    private Coroutine backgroundChangeCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        DialogueManager.OnSequenceChange += ChangeBackground;
    }

    private void OnDisable()
    {
        DialogueManager.OnSequenceChange -= ChangeBackground;
    }

    public void ChangeBackground(DialogueSequence sequence)
    {
        Debug.Log("CHANGE BACKGROUND");
        Sprite newBackground = sequence.background;
        if (primaryBackground.sprite == newBackground) return;

        secondaryBackground.sprite = newBackground;
        if (backgroundChangeCoroutine != null) {
            StopCoroutine(backgroundChangeCoroutine);
        }
        backgroundChangeCoroutine = StartCoroutine(BackgroundTransition());
    }

    public void ChangeBackground(Sprite sprite)
    {
        if (primaryBackground.sprite == sprite) return;

        secondaryBackground.sprite = sprite;
        if (backgroundChangeCoroutine != null) {
            StopCoroutine(backgroundChangeCoroutine);
        }
        backgroundChangeCoroutine = StartCoroutine(BackgroundTransition());
    }

    private IEnumerator BackgroundTransition()
    {
        secondaryBackground.color = new Color(1, 1, 1, 0);
        primaryBackground.color = new Color(1, 1, 1, 1);

        float timer = 0;
        while (timer < transitionSpeed) {
            timer += Time.deltaTime;
            float normalizedTime = timer / transitionSpeed;
            primaryBackground.color = new Color(1, 1, 1, 1 - normalizedTime);
            secondaryBackground.color = new Color(1, 1, 1, normalizedTime);
            yield return null;
        }

        primaryBackground.sprite = secondaryBackground.sprite;
        primaryBackground.color = new Color(1, 1, 1, 1);
        secondaryBackground.color = new Color(1, 1, 1, 0);

        backgroundChangeCoroutine = null;
    }
}
