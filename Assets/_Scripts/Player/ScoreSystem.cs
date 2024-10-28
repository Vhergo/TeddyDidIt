using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    // (REMOVED) [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int punchScore;
    [SerializeField] private int grabAndThrowScore;
    [SerializeField] private int doubleJumpScore;
    [SerializeField] private int chargeThrowScore;
    [SerializeField] private int maxScore;
    [SerializeField] private int score = 0;

    //[SerializeField] private AudioSource destroySound;

    [Header("Score Book")]
    [SerializeField] private int legoScore = 5;
    [SerializeField] private int foodScore = 5;
    [SerializeField] private int clothingScore = 10;
    [SerializeField] private int officeSuppliesScore = 5;
    [SerializeField] private int toysScore = 15;
    [SerializeField] private int sportsEquipmentScore = 15;
    [Space(10)]
    [SerializeField] private int BossDefeatedScore = 7777;
    [SerializeField] private int specialScore = 100;
    [SerializeField] private int prePunchScore = 25;

    [Header("UI")]
    [SerializeField] private Image progressionFill;

    private Dictionary<string,int> score_book = new Dictionary<string,int>();

    public static ScoreSystem Instance;

    private bool punchCheck = false;
    private bool grabAndThrowCheck = false;
    private bool doubleJumpCheck = false;
    private bool chargeThrowCheck = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // private void OnEnable() => BossManager.Instance.OnBossHit += BossDefeated;
    // private void OnDisable() => BossManager.Instance.OnBossHit -= BossDefeated;

    private void Start()
    {
        score_book.Add("Legos", legoScore);
        score_book.Add("Food", foodScore);
        score_book.Add("Clothing", clothingScore);
        score_book.Add("OfficeSupplies", officeSuppliesScore);
        score_book.Add("Toys", toysScore);
        score_book.Add("SportsEquipment", sportsEquipmentScore);
        score_book.Add("Explosion", 100);
        score_book.Add("Projectile", 5);

        maxScore = chargeThrowScore;
        progressionFill.fillAmount = 0;
        ProgressionSystem.Instance.SetProgressIndicatorPositions(punchScore, grabAndThrowScore, doubleJumpScore, chargeThrowScore);
    }

    public void AddScore(String tag1)
    {
        if (!score_book.ContainsKey(tag1)) return;

        int scoreToAdd = score_book[tag1];
        score = score + scoreToAdd;
        CheckThreshhold();
    }

    public void AddScore(float scalar, String tag1, String tag2)
    {
        if (!score_book.ContainsKey(tag1) || !score_book.ContainsKey(tag2)) return;

        float scoreToAdd = (float) score_book[tag1] + score_book[tag2];
        scoreToAdd = Mathf.Floor(scalar * scoreToAdd);
        score = score + (int) scoreToAdd;
        // SoundManager.Instance.PlaySound(destroySound.clip);
        CheckThreshhold();
    }

    [ContextMenu("Add Score")]
    public void AddScore()
    {
        score += 100;
        CheckThreshhold();
    }

    public void AddScorePrePunch()
    {
        score += prePunchScore;
        CheckThreshhold();
    }

    public void BossDefeated()
    {
        score += BossDefeatedScore;
        CheckThreshhold();
    }

    public void SpecialPickup()
    {
        score += specialScore;
        CheckThreshhold();
    }

    private void CheckThreshhold()
    {
        if (score >= chargeThrowScore && !chargeThrowCheck)
        {
            ProgressionSystem.Instance.SetProgressionStage(ProgressStage.ChargeThrow);
            chargeThrowCheck = true;
        }
        else if (score >= doubleJumpScore && !doubleJumpCheck)
        {
            ProgressionSystem.Instance.SetProgressionStage(ProgressStage.DoubleJump);
            doubleJumpCheck = true;
        }else if(score >= grabAndThrowScore && !grabAndThrowCheck)
        {
            ProgressionSystem.Instance.SetProgressionStage(ProgressStage.GrabAndThrow);
            grabAndThrowCheck = true;
        }else if (score >= punchScore && !punchCheck) 
        {
            ProgressionSystem.Instance.SetProgressionStage(ProgressStage.Punch);
            punchCheck = true;
        }

        UpdatedProgressionBar();
    }

    private void UpdatedProgressionBar()
    {
        float progress = (float)score / (float)maxScore;
        progressionFill.fillAmount = progress;
    }
}
