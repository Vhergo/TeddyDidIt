using TMPro;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(TMP_Text))]
public class DisplayTextAnimation : MonoBehaviour
{
    [SerializeField] private AudioClip typeSound;
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private int textLength;

    // Keyframe this for the timeline
    public int maxVisibleCharacters;
    private int previousVisibleCharacters;

    void Awake()
    {
        displayText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        maxVisibleCharacters = 0;
        previousVisibleCharacters = 0;
    }

    private void Update()
    {
        displayText.maxVisibleCharacters = maxVisibleCharacters;
        PlaySoundOnCharacterIncrease();
    }

    private void PlaySoundOnCharacterIncrease()
    {
        if (maxVisibleCharacters > previousVisibleCharacters) {
            SoundManager.Instance.PlaySound(typeSound);
            previousVisibleCharacters = maxVisibleCharacters;
        }
    }

    private void OnValidate()
    {
        textLength = displayText.text.Length;
    }
}
