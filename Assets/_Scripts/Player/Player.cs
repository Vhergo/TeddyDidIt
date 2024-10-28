using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;
    [SerializeField] private float healthRegenDelay = 10f;
    [SerializeField] private Transform healthBar;
    [SerializeField] private GameObject healthPrefab;
    [SerializeField] private List<AudioClip> teddySqueaks;
    private float healthRegenTimer;

    [Header("Dialogue")]
    [SerializeField] private List<Dialogue> teddyHit;
    [SerializeField] private Dialogue teddyHit2Left;
    [SerializeField] private Dialogue teddyHit1Left;
    [SerializeField] private Dialogue teddyDeath;
    [SerializeField] private float hitDialogueChance = 0.5f;
    [Space(10)]
    [SerializeField] private List<Dialogue> healthRegen;
    [SerializeField] private Dialogue healthRegenFull;
    [Space(10)]
    [SerializeField] private List<Dialogue> bossTaunts;
    [SerializeField] private float bonusDialogueDelay = 1f;

    private bool bossIsDead;

    public static Action OnPlayerDeath;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        BossFightManager.OnBossFightReset += ResetHealth;
        BossManager.OnBossDeath += () => bossIsDead = true;
        HealthPickup.OnHealthPickup += IncreaseHealth;
    }
    private void OnDisable()
    {
        BossFightManager.OnBossFightReset -= ResetHealth;
        BossManager.OnBossDeath -= () => bossIsDead = true;
        HealthPickup.OnHealthPickup -= IncreaseHealth;
    }

    private void Start()
    {
        // currentHealth = maxHealth;
        currentHealth = 0;
        healthRegenTimer = healthRegenDelay;
    }

    private void Update()
    {
        //Timer();
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        foreach (Transform health in healthBar) {
            health.gameObject.SetActive(true);
        }
    }

    #region HEALTH

    public void IncreaseHealth()
    {
        maxHealth++;
        currentHealth++;
        Instantiate(healthPrefab, healthBar);
    }


    [ContextMenu("Take Damage")]
    // Take Damage
    public void TakeDamage()
    {
        if (currentHealth <= 3) {
            switch (currentHealth) {
                case 3:
                    SetDialogue(teddyHit2Left);
                    break;
                case 2:
                    SetDialogue(teddyHit1Left);
                    break;
                case 1:
                    SetDialogue(teddyDeath);
                    break;
            }
        }else {
            RandomHitDialogue();
        }

        currentHealth--;
        if (currentHealth >= 0)
            healthBar.GetChild(currentHealth).gameObject.SetActive(false);
        healthRegenTimer = healthRegenDelay;
        if (currentHealth <= 0) {
            PlayerDeath();
        }else {
            SoundManager.Instance.PlaySound(teddySqueaks[UnityEngine.Random.Range(0, teddySqueaks.Count)], true);
        }
    }

    private void PlayerDeath()
    {
        Debug.Log("TEDDY HAS DIED");
        OnPlayerDeath?.Invoke();
        TeddyMovement.Instance.Freeze();

        if (MySceneManager.Instance != null) {
            MySceneManager.Instance.PauseGame();
        }
    }

    private void RandomHitDialogue()
    {
        // Chance for dialogue upon hit
        float randomChance = UnityEngine.Random.Range(0f, 1f);
        if (randomChance <= hitDialogueChance * 1.5f) {

            // Choose between dialogue from Teddy or King Broc
            randomChance = UnityEngine.Random.Range(0f, 1f);
            if (randomChance <= hitDialogueChance) {
                SetDialogue(TeddyHitDialogue());

                // Chance for bonus King Broc dialogue
                randomChance = UnityEngine.Random.Range(0f, 1f);
                if (randomChance <= hitDialogueChance) {
                    SetDelayedDialogue(BossTauntDialogue());
                }
            } else {
                SetDialogue(BossTauntDialogue());
            }
        }   
    }
    #endregion

    #region DIALOGUE
    private void SetDialogue(Dialogue dialogue)
    {
        if (bossIsDead) return;
        DialogueManager.Instance.SetDialogue(dialogue);
    }

    private void SetDelayedDialogue(Dialogue dialogue)
    {
        if (bossIsDead) return;
        StartCoroutine(DialogueManager.Instance.SetDelayedDialogue(dialogue, bonusDialogueDelay));
    }

    private Dialogue GetRegenDialogue()
    {
        int randomeIndex = UnityEngine.Random.Range(0, healthRegen.Count);
        return healthRegen[randomeIndex];
    }

    private Dialogue BossTauntDialogue()
    {
        int randomeIndex = UnityEngine.Random.Range(0, bossTaunts.Count);
        return bossTaunts[randomeIndex];
    }

    private Dialogue TeddyHitDialogue()
    {
        int randomeIndex = UnityEngine.Random.Range(0, teddyHit.Count);
        return teddyHit[randomeIndex];
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (ProgressionSystem.Instance.GetCurrentStage() == ProgressStage.Base) {
            if (collision.gameObject.layer == 8) {
                ScoreSystem.Instance.AddScorePrePunch();

                ObjectScoring objScoring = collision.gameObject.GetComponent<ObjectScoring>();
                if (objScoring != null) objScoring.isInteracted = true;
            }
        }
    }
}
