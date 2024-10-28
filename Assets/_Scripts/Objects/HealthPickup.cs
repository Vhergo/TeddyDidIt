using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour, ICollectible
{
    [SerializeField] private AudioClip healthPickupSound;

    public static Action OnHealthPickup;

    [ContextMenu("Collect")]
    public void Collect()
    {
        Debug.Log("Health Pickup Collected");
        OnHealthPickup?.Invoke();
        SoundManager.Instance.PlaySound(healthPickupSound);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player")) {
            Collect();
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Grabbable")) {
            ObjectInteraction objectInteraction = col.GetComponent<ObjectInteraction>();
            if (objectInteraction != null) {
                if (objectInteraction.CheckCurrentState(ObjectInteractionState.Thrown) ||
                    objectInteraction.CheckCurrentState(ObjectInteractionState.Charged)) {
                    Collect();
                }
            }
        }
    }
}
