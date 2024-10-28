using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class HazardRespawn : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private void Respawn()
    {
        Player.Instance.transform.position = respawnPoint.position;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player")) {
            Respawn();
        }
    }
}
