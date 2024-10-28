using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleGate : MonoBehaviour
{
    [SerializeField] private float gateBreakForce;

    private void TriggerCastleGate()
    {
        GetComponent<Collider>().enabled = false;

        foreach (Transform child in transform) {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(ForceCalculation(child), ForceMode.Impulse);
        }
    }

    private Vector3 ForceCalculation(Transform obj)
    {
        Vector3 direction = obj.position - transform.position;
        return direction.normalized * gateBreakForce;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Grabbable")) {
            ObjectInteraction obj = col.GetComponent<ObjectInteraction>();
            if (obj != null) {
                if (obj.CheckCurrentState(ObjectInteractionState.Charged)) {
                    TriggerCastleGate();
                }
            }
        }
    }
}
