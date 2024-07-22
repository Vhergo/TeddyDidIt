using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    [SerializeField] private Boss boss;

    public delegate void BossEvent();
    public BossEvent onBossHit;

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

    private void Start()
    {
        //set stating emote
        SetEmote(startingEmote);
    }

    //called by boss when hit by player projectile
    public void OnBossHit()
    {
        onBossHit?.Invoke();
    }

    //set face emote of boss
    public void SetEmote(Emotes emote) => boss.SetEmote(emote);
}
