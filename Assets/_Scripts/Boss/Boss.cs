using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private const string PROJECTILE_TAG = "Projectile";

    [Header ("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform firePoint;
    [SerializeField] private SkinnedMeshRenderer faceMesh;
    [SerializeField] private Animator ani;
    [SerializeField] private BossAniEvent aniEvent;

    [Header("Projectile Variants")]
    [SerializeField] private GameObject slowPea;
    [SerializeField] private GameObject fastPea;

    [Header ("Interval Setting For Slow And Fast Peas")]
    public float minIntervalForSlow = 2f;
    public float maxIntervalForSlow = 3f;
    public float minIntervalForFast = 4f;
    public float maxIntervalForFast = 5f;

    [Header("Force Threshold For Boss To Register Hit From Player")]
    [SerializeField] private float bossHitThreshold = 5;

    [Header("Emote Materials")]
    [SerializeField] private Material blinkClosed;
    [SerializeField] private Material blinkOpen;
    [SerializeField] private Material madClosed;
    [SerializeField] private Material madOpen;
    [SerializeField] private Material neutralClosed;
    [SerializeField] private Material neutralOpen;

    private ObjectPoolManager PoolManager => ObjectPoolManager.Instance;

    private bool canFire = false;
    private int slowPeasFired = 0;


    private void Start()
    {
        StartCoroutine(FiringProcess());
        StartFiring();
    }

    #region PROJECTILE FIRING
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
                //fire fast pea every 3 slow peas fired
                if (slowPeasFired < 3)
                {
                    yield return new WaitForSeconds(Random.Range(minIntervalForSlow, maxIntervalForSlow));
                    slowPeasFired++; //increment counter
                    StartCoroutine("FireProcess", slowPea); //fire slow pea
                }
                else
                {
                    yield return new WaitForSeconds(Random.Range(minIntervalForFast, maxIntervalForFast));
                    slowPeasFired = 0; //reset counter
                    StartCoroutine("FireProcess", fastPea); //fire fast pea
                }
                
            }
            else
            {
                yield return null;
            }
        }     
    }  

    //play attack animation and fire projectile
    private IEnumerator FireProcess(GameObject peaVariant)
    {
        //play attack animation
        ani.SetTrigger("fire");

        //wait for animation event before firing projectile at right timing
        while (aniEvent.onAttack == false)
            yield return null;

        //reset event trigger
        aniEvent.ResetAttack();

        //fire projectile
        Fire(peaVariant);
    }

    //fire projectile
    private void Fire(GameObject peaVariant)
    {
        //get chosen projectile from pool
        GameObject projectile = PoolManager.GetPoolObject(peaVariant, firePoint.position, transform.rotation);

        //initialize projectile
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.Fire(target);
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        //register hit if impact higher than threshold
        if (collision.impulse.magnitude > bossHitThreshold)
        {
            if (!collision.gameObject.CompareTag("Projectile"))
                BossManager.Instance.OnBossHit();
        }
    }

    #region EMOTE
    public void SetEmote(BossManager.Emotes emote)
    {
        switch (emote)
        {
            case BossManager.Emotes.blinkClosedMouth:
                faceMesh.material = blinkClosed;
                break;
            case BossManager.Emotes.blinkOpenMouth:
                faceMesh.material = blinkOpen;
                break;
            case BossManager.Emotes.madClosedMouth:
                faceMesh.material = madClosed;
                break;
            case BossManager.Emotes.madOpenMouth:
                faceMesh.material = madOpen;
                break;
            case BossManager.Emotes.neutralClosedMouth:
                faceMesh.material = neutralClosed;
                break;
            case BossManager.Emotes.neutralOpenMouth:
                faceMesh.material = neutralOpen;
                break;
            default:
                break;
        }

        BossManager.Instance.currentEmote = emote;
    }
    #endregion
}
