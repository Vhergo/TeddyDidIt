using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    [SerializeField] private Boss boss;

    public delegate void BossEvent();
    public BossEvent OnBossHit;

    public static Action OnBossDeath;

    [Header("Boss")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;
    [SerializeField] private Transform healthBar;
    [SerializeField] private GameObject healthPrefab;
    private bool isDead;

    [Header("Combat Cycle")]
    [SerializeField] private float firingDuration;
    [SerializeField] private float stompingDuration;

    [Header("Dialogue")]
    [SerializeField] private List<Dialogue> bossHit;
    [SerializeField] private Dialogue bossDefeated;

    [HideInInspector] public Emotes currentEmote;
    [SerializeField] private Emotes startingEmote = Emotes.blinkClosedMouth;
    [SerializeField] private GameObject bossFightRestartButton;
    [SerializeField] private PlayableAsset bossDeathCutscene;
    [SerializeField] private DialogueSequence postBossFightSequence;


    public enum Emotes
    {
        blinkClosedMouth, blinkOpenMouth, madClosedMouth, madOpenMouth, neutralClosedMouth, neutralOpenMouth
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    private void OnEnable()
    {
        BossFightManager.OnBossFightReset += ResetHealth;
        Player.OnPlayerDeath += PlayerDied;
    }
    private void OnDisable()
    {
        BossFightManager.OnBossFightReset -= ResetHealth;
        Player.OnPlayerDeath -= PlayerDied;
    }

    private void Start()
    {
        boss = GetComponent<Boss>();
        currentHealth = maxHealth;
        for (int i = 0; i < maxHealth; i++) {
            Instantiate(healthPrefab, healthBar);
        }
        healthBar.gameObject.SetActive(false);

        SetEmote(startingEmote);

        // StartCoroutine(BossCombatCycle());
    }

    public void ShowBossHealth()
    {
        healthBar.gameObject.SetActive(true);
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        
        foreach (Transform health in healthBar) {
            health.gameObject.SetActive(true);
        }
    }

    public void StartBossCombatCycle()
    {
        StartFiring();
        StartCoroutine(BossCombatCycle());
    }

    private IEnumerator BossCombatCycle()
    {
        while (boss.GetCombatState() != CombatState.Idle) {
            StartFiring();
            yield return new WaitForSeconds(firingDuration);

            StartStomping();
            yield return new WaitForSeconds(stompingDuration);
        }
    }

    //called by boss when hit by player projectile
    [ContextMenu("Boss Hit")]
    public void BossHit(int damage)
    {
        OnBossHit?.Invoke();

        int damageToApply = Mathf.Min(damage, currentHealth);
        currentHealth -= damageToApply;

        for (int i = 0; i < damageToApply; i++) {
            healthBar.GetChild(currentHealth + i).gameObject.SetActive(false);
        }

        // Check if the boss is dead
        if (currentHealth <= 0) {
            if (!isDead) BossDeath();
        } else {
            SetDialogue(BossHitDialogue());
        }
    }

    private void BossDeath()
    {
        isDead = true;
        // MySceneManager.Instance.PauseGame();
        StopAllCoroutines();
        StopAttacking();
        StopAllProjectiles();
        bossFightRestartButton.SetActive(false);
        TeddyMovement.Instance.Freeze();
        CutsceneManager.Instance.PlayCutscene(bossDeathCutscene);
        Debug.Log("Boss Defeated");
    }

    public void DestroyBoss()
    {
        OnBossDeath?.Invoke();
        SetDialogue(bossDefeated);
        Debug.Log("Boss Destroyed");
        DialogueManager.Instance.SkipSequence();
        //DialogueManager.Instance.UpdateSequence(postBossFightSequence);
        HandlePostProcessing.Instance.ToggleDarkenVolume(true);
        Destroy(gameObject);
    }

    private void StopAllProjectiles()
    {
        Projectile[] projectiles = FindObjectsOfType<Projectile>();
        foreach (Projectile projectile in projectiles) {
            Destroy(projectile.gameObject);
        }
    }

    private void SetDialogue(Dialogue dialogue)
    {
        DialogueManager.Instance.SetDialogue(dialogue);
    }

    private Dialogue BossHitDialogue()
    {
        int randomeIndex = UnityEngine.Random.Range(0, bossHit.Count);
        return bossHit[randomeIndex];
    }

    private void PlayerDied()
    {
        StopAllCoroutines();
        StopAttacking();
    }

    //set face emote of boss
    public void SetEmote(Emotes emote) => boss.SetEmote(emote);

    public void StartFiring() => boss.SetCombatState(CombatState.Firing);
    public void StartStomping() => boss.SetCombatState(CombatState.Stomping);
    public void StopAttacking() => boss.SetCombatState(CombatState.Idle);

    public void TriggerStompAttack() => boss.FinishedStopming();
    public void StartStompMovement() => boss.StartedStomping();
}
