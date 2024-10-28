using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private const string PROJECTILE_TAG = "Projectile";
    private List<string> throwableTags = new List<string> {
        "Legos",
        "Food",
        "Clothing",
        "OfficeSupplies",
        "SportsEquipment",
        "Toys"
    };

    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform firePointLeft;
    [SerializeField] private Transform firePointRight;
    private Transform firePoint;

    [Space(10)]
    [SerializeField] private SkinnedMeshRenderer faceMesh;
    [SerializeField] private Animator anim;
    [SerializeField] private BossAnimEvent animEvent;
    [SerializeField] private AnimationClip takenDamageAnimation;
    [SerializeField] private AnimationClip turnLeft;
    [SerializeField] private AnimationClip turnRight;
    private bool isFacingLeft = true;

    [Header("Stomping")]
    [SerializeField] private AnimationClip stompAnimation;
    [SerializeField] private AudioClip stompSound;
    [SerializeField] private Transform stompPoint;
    [SerializeField] private float stompMoveSpeed;
    [SerializeField] private float stompDelay = 4f;
    [SerializeField] private float stompDuration = 1f;
    [SerializeField] private float stompRadius = 5f;
    [SerializeField] private float stompForce = 25f;
    private bool isStomping;
    private bool stompStarted;

    [Space(10)]
    [SerializeField] private AnimationClip deathAnimation;

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

    [SerializeField] private AudioClip shootSound;

    private ObjectPoolManager PoolManager => ObjectPoolManager.Instance;

    private bool canFire = false;
    private int slowPeasFired = 0;

    private CombatState currentCombatState = CombatState.Idle;
    private Coroutine currentCombatCoroutine;

    private void Start()
    {
        // SetCombatState(CombatState.Default);
        firePoint = firePointLeft;
    }

    private void Update()
    {
        BossTurn();
    }

    public void SetCombatState(CombatState state)
    {
        if (currentCombatState == state) return;
        currentCombatState = state;

        switch (state) {
            case CombatState.Idle:
                // Do nothing
                StopCoroutine(currentCombatCoroutine);
                break;
            case CombatState.Firing:
                StartFiring();
                if (currentCombatCoroutine != null) StopCoroutine(currentCombatCoroutine);
                currentCombatCoroutine = StartCoroutine(FiringProcess());
                break;
            case CombatState.Stomping:
                StopFiring();
                isStomping = stompStarted = false;
                if (currentCombatCoroutine != null) StopCoroutine(currentCombatCoroutine);
                currentCombatCoroutine = StartCoroutine(StompingProcess());
                break;
        }
    }

    public CombatState GetCombatState() => currentCombatState;

    #region FIRING PROCESS
    // Fire projectiles at a set interval
    private IEnumerator FiringProcess()
    {
        while (currentCombatState == CombatState.Firing) {   
            if (canFire) {
                // Fire fast pea every 3 slow peas fired
                if (slowPeasFired < 3) {
                    yield return new WaitForSeconds(UnityEngine.Random.Range(minIntervalForSlow, maxIntervalForSlow));
                    slowPeasFired++; //increment counter
                    StartCoroutine("FireProcess", slowPea); //fire slow pea
                }else {
                    yield return new WaitForSeconds(UnityEngine.Random.Range(minIntervalForFast, maxIntervalForFast));
                    slowPeasFired = 0; //reset counter
                    StartCoroutine("FireProcess", fastPea); //fire fast pea
                }
            } else {
                yield return null;
            }
        }     
    }  

    //play attack animation and fire projectile
    private IEnumerator FireProcess(GameObject peaVariant)
    {
        //play attack animation
        anim.SetTrigger("fire");

        //wait for animation event before firing projectile at right timing
        while (animEvent.onAttack == false)
            yield return null;

        //reset event trigger
        animEvent.ResetAttack();

        Fire(peaVariant);
    }

    private void Fire(GameObject peaVariant)
    {
        GameObject projectile = PoolManager.GetPoolObject(peaVariant, firePoint.position, transform.rotation);

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.Fire(target);

        SoundManager.Instance.PlaySound(shootSound, true);
    }
    public void StartFiring() => canFire = true;
    public void StopFiring() => canFire = false;
    #endregion

    #region STOMPING PROCESS

    private IEnumerator StompingProcess()
    {
        Debug.Log("STOOOOOOOMPING");
        while (currentCombatState == CombatState.Stomping) {
            // isStomping = true;
            StartCoroutine(StartStomp());
            Debug.Log("JUMP");
            yield return new WaitUntil(() => !isStomping && stompStarted);
            
            EndStomp();
            Debug.Log("LAND");
            yield return new WaitForSeconds(stompDelay);
        }
    }

    public IEnumerator StartStomp()
    {
        Vector3 stompDirection = isFacingLeft ? Vector3.left : Vector3.right;
        Vector3 targetPosition = transform.position + stompDirection * 100f;

        anim.Play(stompAnimation.name);
        yield return new WaitUntil(() => isStomping);
        stompStarted = true;

        float stompTimer = 0;
        while (stompTimer < stompDuration) {
            stompTimer += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, stompMoveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void EndStomp()
    {
        Collider[] colliders = Physics.OverlapSphere(stompPoint.position, stompRadius);
        foreach (Collider col in colliders) {
            Debug.Log("COLLIDED WITH " + col.gameObject.name);
            Rigidbody rb;
            if (col.CompareTag("Player")) {
                rb = Player.Instance.GetComponent<Rigidbody>();
                if (TeddyMovement.Instance.IsGrounded()) Player.Instance.TakeDamage();
                Debug.Log("PLAYER STOMPED");
            } else {
                rb = col.GetComponent<Rigidbody>();
                Debug.Log("OBJECT STOMPED" + col.gameObject.name);
            }
            
            if (rb != null)
                rb.AddForce(Vector3.up * stompForce, ForceMode.Impulse);

            SoundManager.Instance.PlaySound(stompSound, true);
            CameraShake.Instance.TriggerCameraShake();
        }

        stompStarted = false;
    }

    public void StartedStomping() => isStomping = true;
    public void FinishedStopming() => isStomping = false;

    #endregion

    #region OTHER ANIMATIONS
    private void BossTurn()
    {
        if (target.position.x < transform.position.x && !isFacingLeft) {
            Debug.Log("TURN LEFT");
            anim.Play(turnLeft.name, 1);
            isFacingLeft = true;
            firePoint = firePointLeft;
        } else if (target.position.x > transform.position.x && isFacingLeft) {
            Debug.Log("TURN RIGHT");
            anim.Play(turnRight.name, 1);
            isFacingLeft = false;
            firePoint = firePointRight;
        }
    }

    #endregion

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

    #region TAKE DAMAGE

    [ContextMenu("Take Damage")]
    public void TakeDamage(int damage)
    {
        BossManager.Instance.BossHit(damage);

        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip != stompAnimation)
            anim.Play(takenDamageAnimation.name);


    }

    public void PlayDeathAnimation()
    {
        Debug.Log("BOSS DEATH ANIMATION");
        anim.Play(deathAnimation.name, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > bossHitThreshold) {
            if (CheckCollisionTag(collision.gameObject.tag)) {
                ObjectInteraction obj = collision.gameObject.GetComponent<ObjectInteraction>();
                if (obj != null) {
                    if (obj.CheckCurrentState(ObjectInteractionState.Thrown)) {
                        TakeDamage(1);
                        obj.ResetNow();
                    } else if (obj.CheckCurrentState(ObjectInteractionState.Charged)) {
                        TakeDamage(2);
                        obj.ResetNow();
                    }
                }
            }
        }
    }

    private bool CheckCollisionTag(string tag)
    {
        return throwableTags.Contains(tag);
    }
    #endregion
}

[Serializable]
public enum CombatState
{
    Idle,
    Firing,
    Stomping
}
