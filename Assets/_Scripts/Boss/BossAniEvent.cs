using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAniEvent : MonoBehaviour
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
}
