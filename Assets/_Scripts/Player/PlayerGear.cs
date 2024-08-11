using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGear : MonoBehaviour
{
    [SerializeField] private GameObject belt;
    [SerializeField] private GameObject cape;
    [SerializeField] private GameObject gloveLeft;
    [SerializeField] private GameObject gloveRight;
    [SerializeField] private GameObject bootLeft;
    [SerializeField] private GameObject bootRight;

    private ProgressStage currentProgress;

    public static Action OnShoesEquipped;

    private void OnEnable()
    {
        ProgressionSystem.OnProgressionChanged += UpgradeGearBasedOnProgression;
    }

    private void OnDisable()
    {
        ProgressionSystem.OnProgressionChanged -= UpgradeGearBasedOnProgression;
    }

    public void Start()
    {
        belt.SetActive(false);
        cape.SetActive(false);
        gloveLeft.SetActive(false);
        gloveRight.SetActive(false);
        bootLeft.SetActive(false);
        bootRight.SetActive(false);
    }

    private void UpgradeGearBasedOnProgression()
    {
        currentProgress = ProgressionSystem.Instance.GetCurrentStage();
        UpdateGear(currentProgress);
    }

    private void UpdateGear(ProgressStage stage)
    {
        switch (stage) {
            case ProgressStage.Punch:
                gloveLeft.SetActive(true);
                gloveRight.SetActive(true);
                break;
            case ProgressStage.GrabAndThrow:
                belt.SetActive(true);
                break;
            case ProgressStage.DoubleJump:
                bootLeft.SetActive(true);
                bootRight.SetActive(true);
                OnShoesEquipped?.Invoke();
                break;
            case ProgressStage.ChargeThrow:
                cape.SetActive(true);
                break;
        }
    }
}
