using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleGateBreakers : MonoBehaviour
{
    private void DisableChildKinematic()
    {
        foreach (Transform child in transform) {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            rb.isKinematic = false;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player")) {
            DisableChildKinematic();
        }
    }
}
