using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGear : MonoBehaviour
{
    [SerializeField] private GameObject hat;
    [SerializeField] private GameObject cape;
    [SerializeField] private GameObject gloveLeft;
    [SerializeField] private GameObject gloveRight;

    private ProgressStage currentProgress;

    private void OnEnable()
    {
        ProgressionSystem.OnProgressionChanged += GetCurrentProgression;
    }

    private void OnDisable()
    {
        ProgressionSystem.OnProgressionChanged -= GetCurrentProgression;
    }

    public void Start()
    {
        hat.SetActive(false);
        cape.SetActive(false);
        gloveLeft.SetActive(false);
        gloveRight.SetActive(false);
    }

    private void GetCurrentProgression()
    {
        currentProgress = ProgressionSystem.Instance.GetCurrentStage();
        UpdateGear(currentProgress);
    }

    private void UpdateGear(ProgressStage stage)
    {
        switch (stage) {
            case ProgressStage.GrabAndThrow:
                gloveLeft.SetActive(true);
                gloveRight.SetActive(true);
                break;
            case ProgressStage.DoubleJump:
                cape.SetActive(true);
                break;
            case ProgressStage.ChargeThrow:
                hat.SetActive(true);
                break;
        }
    }
}
