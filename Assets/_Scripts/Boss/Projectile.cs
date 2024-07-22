using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
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
        rb.velocity = (target.position - transform.position).normalized * strength;
    }

    private void DestroyProjectile()
    {
        rb.useGravity = false; //to ensure gravity is not on when this projectle is reused
        ObjectPoolManager.Instance.ReturnPoolObject(gameObject); //return to pool
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.useGravity = true; //let projectile fall once it hits something
    }
}
