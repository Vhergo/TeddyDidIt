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
    [SerializeField] private AudioSource destroySound;


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
        score_book.Add("Legos", 5);
        score_book.Add("Food", 5);
        score_book.Add("Clothing", 10);
        score_book.Add("OfficeSupplies", 5);
        score_book.Add("Toys", 15);
        score_book.Add("SportsEquipment", 15);
    }

    public void addScore(float scalar, String tag1, String tag2)
    {
        float scoreToAdd = (float) score_book[tag1] + score_book[tag2];
        scoreToAdd = Mathf.Floor(scalar * scoreToAdd);
        score = score + (int) scoreToAdd;
        scoreText.text = score.ToString("#,#");
        destroySound.Play();
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
