using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    [SerializeField] private Boss boss;

    public delegate void BossEvent();
    public BossEvent OnBossHit;

    [Header("Boss")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;

    [Header("Dialogue")]
    [SerializeField] private List<Dialogue> bossHit;
    [SerializeField] private Dialogue bossDefeated;

    [HideInInspector] public Emotes currentEmote;
    [SerializeField] private Emotes startingEmote = Emotes.blinkClosedMouth;
    public enum Emotes
    {
        blinkClosedMouth, blinkOpenMouth, madClosedMouth, madOpenMouth, neutralClosedMouth, neutralOpenMouth
    }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Player.OnPlayerDeath += StopFiring;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= StopFiring;
    }

    private void Start()
    {
        boss = GetComponent<Boss>();
        currentHealth = maxHealth;

        SetEmote(startingEmote);

    }

    //called by boss when hit by player projectile
    public void BossHit()
    {
        OnBossHit?.Invoke();
        currentHealth--;

        if (currentHealth <= 0) {
            BossDeath();
        }else {
            SetDialogue(BossHitDialogue());
        }
    }

    private void BossDeath()
    {
        SetDialogue(bossDefeated);
    }

    private void SetDialogue(Dialogue dialogue)
    {
        DialogueManager.Instance.SetDialogue(dialogue);
    }

    private Dialogue BossHitDialogue()
    {
        int randomeIndex = Random.Range(0, bossHit.Count);
        return bossHit[randomeIndex];
    }


    //set face emote of boss
    public void SetEmote(Emotes emote) => boss.SetEmote(emote);

    public void StartFiring() => boss.StartFiring();
    public void StopFiring() => boss.StopFiring();
}
