using TMPro;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(TMP_Text))]
public class DisplayTextAnimation : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private int textLength;

    // Keyframe this for the timeline
    public int maxVisibleCharacters;

    void Awake()
    {
        displayText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        displayText.maxVisibleCharacters = maxVisibleCharacters;
    }

    private void OnValidate()
    {
        textLength = displayText.text.Length;
    }
}
