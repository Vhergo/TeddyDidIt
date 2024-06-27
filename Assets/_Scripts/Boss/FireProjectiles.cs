using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectiles : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform firePoint;

    [Header("Projectile Variants")]
    [SerializeField] private GameObject[] projectilePrefabs;

    [Header ("Interval Setting")]
    public float minInterval = 2f;
    public float maxInterval = 5f;

    [Header("Strength Multiplier Setting")]
    public float minMultiplier = 1f;
    public float maxMultiplier = 1.5f;

    private ObjectPoolManager PoolManager => ObjectPoolManager.Instance;

    private bool canFire;

    private void Start()
    {
        StartCoroutine(FiringProcess());
        StartFiring();
    }

    //allow firing process
    public void StartFiring()
    {
        canFire = true;
    }

    //pause firing process
    public void StopFiring()
    {
        canFire = false;
    }

    //fire projectiles at a set interval
    private IEnumerator FiringProcess()
    {
        //keep firing unless stopped
        while (true)
        {   
            if (canFire)
            {
                //fire then wait for random interval
                Fire();
                yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            }
            else
            {
                yield return null;
            }
        }     
    }  

    //fire a projectile
    private void Fire()
    {
        //choose random projectile variant
        GameObject chosenVariant = projectilePrefabs[Random.Range(0, projectilePrefabs.Length)];
        //get chosen projectile from pool
        GameObject projectile = PoolManager.GetPoolObject(chosenVariant, firePoint.position, transform.rotation);

        //initialize projectile
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.Fire(target, Random.Range(minMultiplier, maxMultiplier));   
    }
}
