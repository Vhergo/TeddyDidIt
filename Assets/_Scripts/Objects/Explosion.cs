using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Explosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionForce;
    [SerializeField] private float zAxisAmplifier;
    [SerializeField] private bool destroyOnTrigger;

    [Space(10)]
    [SerializeField] private int awardedScore;

    [Space(10)]
    [SerializeField] private List<GameObject> affectedObjects;

    private void Start()
    {
        SetAffectedObjectsToKinematic();
    }

    private void SetAffectedObjectsToKinematic()
    {
        foreach (GameObject obj in affectedObjects) {
            foreach (Rigidbody rb in obj.GetComponentsInChildren<Rigidbody>()) {
                rb.isKinematic = true;
            }
        }
    }

    private void TriggerExplosion()
    {
        foreach (GameObject obj in affectedObjects) {
            foreach (Rigidbody rb in obj.GetComponentsInChildren<Rigidbody>()) {
                rb.isKinematic = false;
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

                Vector3 explosionDir = rb.transform.position - transform.position;
                explosionDir.y *= zAxisAmplifier;
                rb.AddForce(explosionDir.normalized, ForceMode.Impulse);
            }
        }
        AddScore();
        if (destroyOnTrigger) Destroy(gameObject, 1f);
    }

    private void AddScore()
    {
        ScoreSystem.Instance.AddScore(10, "Explosion", "Explosion");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            TriggerExplosion();
        }
    }
}
