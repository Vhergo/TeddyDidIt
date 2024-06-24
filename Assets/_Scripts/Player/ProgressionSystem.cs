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

    private ProgressStage currentState;

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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SetProgressionStage(ProgressStage.Base);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SetProgressionStage(ProgressStage.GrabAndThrow);
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SetProgressionStage(ProgressStage.DoubleJump);
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
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
        switch(stage) {
            case ProgressStage.Base:
                currentState = ProgressStage.Base;
                ActivateUIIndicator(0);
                break;
            case ProgressStage.GrabAndThrow:
                currentState = ProgressStage.GrabAndThrow;
                OnGrabAndThrowEnabled?.Invoke();
                ActivateUIIndicator(1);
                break;
            case ProgressStage.DoubleJump:
                currentState = ProgressStage.DoubleJump;
                OnDoubleJumpEnabled?.Invoke();
                ActivateUIIndicator(2);
                break;
            case ProgressStage.ChargeThrow:
                currentState = ProgressStage.ChargeThrow;
                OnChargeThrowEnabled?.Invoke();
                ActivateUIIndicator(3);
                break;
        }
    }

    public ProgressStage GetCurrentStage()
    {
        return currentState;
    }

    private void ActivateUIIndicator(int index)
    {
        progressionIndicators[index].transform.GetChild(0).GetComponent<Image>().enabled = true;
    }
}

[Serializable]
public enum ProgressStage
{
    Base,
    GrabAndThrow,
    DoubleJump,
    ChargeThrow
}
