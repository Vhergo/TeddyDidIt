using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimEvent : MonoBehaviour
{
    [HideInInspector] public bool onAttack = false;

    public void OnAttackAnimation()
    {
        onAttack = true;
    }

    public void ResetAttack()
    {
        onAttack = false;
    }

    public void OnStompAttack()
    {
        BossManager.Instance.TriggerStompAttack();
    }

    public void OnStompStarted()
    {
        BossManager.Instance.StartStompMovement();
    }
}
