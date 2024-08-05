using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionSystem : MonoBehaviour
{
    public static ProgressionSystem Instance { get; private set; }

    [Tooltip("Add to list in order of progression")]
    [SerializeField] private List<GameObject> progressionIndicators = new List<GameObject>();
    [SerializeField] private AudioSource progressionSound01;
    [SerializeField] private AudioSource progressionSound02;
    [SerializeField] private AudioSource progressionSound03;
    [SerializeField] private RectTransform progressionBar;

    private ProgressStage currentState;

    public static Action OnProgressionChanged;
    public static Action OnPunchEnabled;
    public static Action OnGrabAndThrowEnabled;
    public static Action OnDoubleJumpEnabled;
    public static Action OnChargeThrowEnabled;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeUI();
        SetProgressionStage(ProgressStage.Base);
        SetProgressionStage(ProgressStage.Punch);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SetProgressionStage(ProgressStage.Base);
        }else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SetProgressionStage(ProgressStage.Punch);
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SetProgressionStage(ProgressStage.GrabAndThrow);
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            SetProgressionStage(ProgressStage.DoubleJump);
        } else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            SetProgressionStage(ProgressStage.ChargeThrow);
        }
    }

    private void InitializeUI()
    {
        foreach (GameObject indicator in progressionIndicators) {
            indicator.transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
    }

    public void SetProgressionStage(ProgressStage stage)
    {
        Debug.Log("Entering Stage: " + stage.ToString());
        switch (stage) {
            case ProgressStage.Base:
                currentState = ProgressStage.Base;
                ActivateUIIndicator(0);
                break;
            case ProgressStage.Punch:
                currentState = ProgressStage.Punch;
                OnPunchEnabled?.Invoke();
                ActivateUIIndicator(1);
                break;
            case ProgressStage.GrabAndThrow:
                currentState = ProgressStage.GrabAndThrow;
                OnGrabAndThrowEnabled?.Invoke();
                ActivateUIIndicator(2);
                SoundManager.Instance.PlayNext(SoundManager.Instance.track02);
                SoundManager.Instance.PlaySound(progressionSound01.clip);
                break;
            case ProgressStage.DoubleJump:
                currentState = ProgressStage.DoubleJump;
                OnDoubleJumpEnabled?.Invoke();
                ActivateUIIndicator(3);
                SoundManager.Instance.PlayNext(SoundManager.Instance.track03);
                SoundManager.Instance.PlaySound(progressionSound02.clip);
                break;
            case ProgressStage.ChargeThrow:
                currentState = ProgressStage.ChargeThrow;
                OnChargeThrowEnabled?.Invoke();
                ActivateUIIndicator(4);
                SoundManager.Instance.PlayNext(SoundManager.Instance.track04);
                SoundManager.Instance.PlaySound(progressionSound03.clip);
                break;
        }
    }

    public ProgressStage GetCurrentStage()
    {
        return currentState;
    }

    private void ActivateUIIndicator(int index)
    {
        OnProgressionChanged?.Invoke();
        progressionIndicators[index].transform.GetChild(0).GetComponent<Image>().enabled = true;
    }

    public void SetProgressIndicatorPositions(int punch, int grab, int jump, int charge)
    {
        float barWidth = progressionBar.rect.width;

        float punchPercent = (float)punch / charge;
        float grabPercent = (float)grab / charge;
        float jumpPercent = (float)jump / charge;
        float chargePercent = (float)charge / charge;
        List<float> percentages = new List<float> { 0, punchPercent, grabPercent, jumpPercent, chargePercent };

        float startX = progressionIndicators[0].GetComponent<RectTransform>().anchoredPosition.x;

        for (int i = 0; i < progressionIndicators.Count; i++) {
            Debug.Log(percentages[i]);
            progressionIndicators[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(startX + (barWidth * percentages[i]), 0, 0);
        }
    }
}

[Serializable]
public enum ProgressStage
{
    Base,
    Punch,
    GrabAndThrow,
    DoubleJump,
    ChargeThrow
}
