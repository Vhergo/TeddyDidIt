using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CombatAnimations : MonoBehaviour
{
    public static CombatAnimations Instance { get; private set; }

    [SerializeField] private Animator anim;
    [SerializeField] private Rig rig;

    [Header("Animation Clips")]
    [SerializeField] private AnimationClip rigIdle;
    [SerializeField] private AnimationClip punchClip;
    [SerializeField] private AnimationClip grabClip;
    [SerializeField] private AnimationClip throwClip;
    private bool punchLeft = false;

    private void Awake()
    {
        if (Instance == null)  Instance = this;
        else  Destroy(gameObject);
    }

    private void OnEnable()
    {
        CombatSystem.OnPunch += PunchAnimation;
        CombatSystem.OnGrab += GrabAnimation;
        CombatSystem.OnThrow += ThrowAnimation;
    }

    private void OnDisable()
    {
        CombatSystem.OnPunch -= PunchAnimation;
        CombatSystem.OnGrab -= GrabAnimation;
        CombatSystem.OnThrow -= ThrowAnimation;
    }

    public void PunchAnimation()
    {
        anim.SetBool("PunchLeft", punchLeft);
        anim.SetTrigger("Punch");

        punchLeft = !punchLeft;
    }

    public void GrabAnimation()
    {
        Debug.Log("GRAB");
        anim.Play(grabClip.name);
    }

    public void ThrowAnimation()
    {
        Debug.Log("THROW");
        anim.Play(throwClip.name);
    }
}
