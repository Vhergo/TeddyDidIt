using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public enum Emotes
    {
        blinkClosedMouth, blinkOpenMouth, madClosedMouth, madOpenMouth, neutralClosedMouth, neutralOpenMouth
    }

    private void Awake()
    {
        Instance = this;
    }


    private void OnEnable() => BossFightManager.OnBossFightReset += ResetHealth;
    private void OnDisable() => BossFightManager.OnBossFightReset -= ResetHealth;

    private void Start()
    {
        boss = GetComponent<Boss>();
        currentHealth = maxHealth;

        SetEmote(startingEmote);

        // StartCoroutine(BossCombatCycle());
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
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
    public void BossHit()
    {
        OnBossHit?.Invoke();
        currentHealth--;

        if (currentHealth <= 0) {
            if (!isDead) BossDeath();
        }else {
            SetDialogue(BossHitDialogue());
        }
    }

    private void BossDeath()
    {
        OnBossDeath?.Invoke();

        isDead = true;
        SetDialogue(bossDefeated);
        // MySceneManager.Instance.PauseGame();
        StopAllCoroutines();
        StopAttacking();
        bossFightRestartButton.SetActive(false);
        TeddyMovement.Instance.Freeze();
        DialogueManager.Instance.SkipSequence();
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


    //set face emote of boss
    public void SetEmote(Emotes emote) => boss.SetEmote(emote);

    public void StartFiring() => boss.SetCombatState(CombatState.Firing);
    public void StartStomping() => boss.SetCombatState(CombatState.Stomping);
    public void StopAttacking() => boss.SetCombatState(CombatState.Idle);

    public void TriggerStompAttack() => boss.FinishedStopming();
    public void StartStompMovement() => boss.StartedStomping();
}
