using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Explosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionForce;
    [SerializeField] private float explosionForcePlayer;
    [SerializeField] private bool destroyOnTrigger;
    [SerializeField] private bool triggerByAnything;
    [SerializeField] private bool triggerOnSpawn;

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
        if (affectedObjects.Count == 0) {
            // Get all objects within the explosion radius
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider col in colliders) {
                affectedObjects.Add(col.gameObject);
            }
        }

        if (affectedObjects.Count > 0) {
            foreach (GameObject obj in affectedObjects) {
                foreach (Rigidbody rb in obj.GetComponentsInChildren<Rigidbody>()) {
                    if (!rb.gameObject.CompareTag("Player"))
                        rb.isKinematic = true;
                }
            }
        }

        if (triggerOnSpawn) TriggerExplosion();
    }

    private void TriggerExplosion()
    {
        foreach (GameObject obj in affectedObjects) {
            if (obj == null) continue;

            if (obj.CompareTag("Player")) {
                Rigidbody playerRb = Player.Instance.GetComponent<Rigidbody>();
                playerRb.isKinematic = false;
                playerRb.AddForce(new Vector3(-1, 1, 0) * explosionForcePlayer, ForceMode.Impulse);
            }

            foreach (Rigidbody rb in obj.GetComponentsInChildren<Rigidbody>()) {
                rb.isKinematic = false;
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        AddScore();
        if (destroyOnTrigger) Destroy(gameObject, 1f);
    }

    private void AddScore()
    {
        ScoreSystem.Instance.AddScore("Explosion");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            if (triggerByAnything) Player.Instance.TakeDamage();
            TriggerExplosion();
        }
    }
}
