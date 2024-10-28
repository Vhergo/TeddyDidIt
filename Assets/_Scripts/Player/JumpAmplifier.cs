using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAmplifier : MonoBehaviour
{
    [SerializeField] private float jumpAmplifier;
    private TeddyMovement player;

    private void Start()
    {
        player = TeddyMovement.Instance;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player")) {
            player.SetJumpAmplifier(jumpAmplifier);
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Player")) {
            player.SetJumpAmplifier(1f);
        }
    }
}
