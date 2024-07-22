using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int GrabAndThrowScore;
    [SerializeField] private int DoubleJumpScore;
    [SerializeField] private int ChargeThrowScore;

    public static ScoreSystem Instance;
    private int score = 0;

    private bool GrabAndThrowCheck = false;
    private bool DoubleJumpCheck = false;
    private bool ChargeThrowCheck = false;

    private void Awake()
    {
        Instance = this;
    }

    [ContextMenu("Add Score")]
    public void addScore()
    {
        score += 100;
        scoreText.text = score.ToString("#,#");
        checkThreshhold();
    }

    private void checkThreshhold()
    {
        if (score >= ChargeThrowScore && !ChargeThrowCheck)
        {
            ProgressionSystem.Instance.SetProgressionStage(ProgressStage.ChargeThrow);
            ChargeThrowCheck = true;
        }
        else if (score >= DoubleJumpScore && !DoubleJumpCheck)
        {
            ProgressionSystem.Instance.SetProgressionStage(ProgressStage.DoubleJump);
            DoubleJumpCheck = true;
        }else if(score >= GrabAndThrowScore && !GrabAndThrowCheck)
        {
            ProgressionSystem.Instance.SetProgressionStage(ProgressStage.GrabAndThrow);
            GrabAndThrowCheck = true;
        }
    }
}
