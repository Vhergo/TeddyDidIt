using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject explosion;

    [Header("Settings")]
    [SerializeField] private bool selfDestroy = true;
    [SerializeField] private float timeToSelfDestroy = 10;
    [SerializeField] private float strength = 10f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (selfDestroy)
            Invoke("DestroyProjectile", timeToSelfDestroy);
    }

    public void Fire(Transform target)
    {
        //move towards player
        Vector3 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * strength;
    }

    private void DestroyProjectile()
    {
        rb.useGravity = false; //to ensure gravity is not on when this projectle is reused
        ObjectPoolManager.Instance.ReturnPoolObject(gameObject); //return to pool
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player")) {
            Debug.Log("BOSS PROJECITLE COLLIDED WITH PLAYER");
            ObjectPoolManager.Instance.GetPoolObject(explosion, transform.position, Quaternion.identity);
            ObjectPoolManager.Instance.ReturnPoolObject(gameObject);
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Grabbable")) {
            ObjectInteraction obj = col.GetComponent<ObjectInteraction>();
            if (obj != null) {
                if (obj.CheckCurrentState(ObjectInteractionState.Thrown) ||
                    obj.CheckCurrentState(ObjectInteractionState.Charged)) {
                    ObjectPoolManager.Instance.GetPoolObject(explosion, transform.position, Quaternion.identity);
                    ObjectPoolManager.Instance.ReturnPoolObject(gameObject);
                }
            }
        }
    }
}
