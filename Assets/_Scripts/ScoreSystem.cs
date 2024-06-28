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

    private Dictionary<string,int> score_book = new Dictionary<string,int>();

    public static ScoreSystem Instance;
    private int score = 0;

    private bool GrabAndThrowCheck = false;
    private bool DoubleJumpCheck = false;
    private bool ChargeThrowCheck = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        score_book.Add("Food", 15);
        score_book.Add("Clothing", 20);
        score_book.Add("OfficeSupplies", 10);
        score_book.Add("Toys", 25);
        score_book.Add("SportsEquipment", 30);

    }

    public void addScore(String tag1, String tag2)
    {
        score = score + score_book[tag1] + score_book[tag2];
        scoreText.text = score.ToString("#,#");
        checkThreshhold();
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
