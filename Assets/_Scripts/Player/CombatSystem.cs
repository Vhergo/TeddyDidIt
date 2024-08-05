using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    [Header("Punch")]
    [SerializeField] private GameObject punchSimulator;
    [SerializeField] private float punchCooldown = 0.5f;
    [SerializeField] private float punchForce = 10f;
    [SerializeField] private float punchRadius = 2f;
    [SerializeField] private AudioSource punchSound;
    private bool punchOnCooldown;

    [Header("Grab and Throw"), Space(10)]
    [SerializeField] private Transform grabPos;
    [SerializeField] private Transform throwTarget;
    [SerializeField] private Transform playerCore;
    [Space(10)]
    [SerializeField] private float grabRadius = 2f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float throwCooldown = 1f;
    [SerializeField] private AudioSource throwSound;
    private GameObject grabbedObject;
    private bool grabOnCooldown;
    private bool hasGrabbed;

    [Header("Charged Throw")]
    [SerializeField] private float chargedThrowForce = 20f;
    [SerializeField] private float chargedThrowCooldown = 3f;
    [SerializeField] private float chargeTime = 2f;
    private float chargeTimer;
    private bool chargeOnCooldown;
    private bool isCharging;

    public bool HasGrabbed => hasGrabbed;

    private ObjectScoring objectScoringScript;

    private ProgressStage progressStage;
    private CameraShake cameraShake;

    public static Action OnPunch;
    public static Action OnGrab;
    public static Action OnThrow;
    public static Action OnCharge;
    public static Action OnChargedThrow;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        ProgressionSystem.OnProgressionChanged += UpdateProgressionStage;
    }

    private void OnDisable()
    {
        ProgressionSystem.OnProgressionChanged -= UpdateProgressionStage;
    }

    private void Start()
    {
        progressStage = ProgressionSystem.Instance.GetCurrentStage();
        cameraShake = CameraShake.Instance;
        chargeTimer = chargeTime;
    }

    private void Update()
    {
        HandleCombatInput();
        Timers();
    }

    private void HandleCombatInput()
    {
        switch (progressStage) {
            case ProgressStage.Base:
                break;
            case ProgressStage.Punch:
                PunchInput();
                break;
            case ProgressStage.GrabAndThrow:
                PunchInput();
                GrabAndThrowInput();
                break;
            case ProgressStage.DoubleJump:
                EnableDoubleJump();
                PunchInput();
                GrabAndThrowInput();
                break;
            case ProgressStage.ChargeThrow:
                PunchInput();
                GrabAndThrowInput();
                ChargedThrowInput();
                break;
        }
    }

    #region PUNCH
    private void PunchInput()
    {
        if (Input.GetMouseButtonDown(0) && !punchOnCooldown && !hasGrabbed) {
            OnPunch?.Invoke();
        }
    }

    public void Punch()
    {
        Debug.Log("PUNCH");
        punchOnCooldown = true;

        Vector3 punchDirection = (throwTarget.position - playerCore.position).normalized;
        Vector3 punchPoint = playerCore.position + punchDirection * punchRadius;

        Collider[] colliders = Physics.OverlapSphere(punchPoint, punchRadius);
        foreach (Collider punchableObjects in colliders) {
            Rigidbody rb = punchableObjects.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddForce(punchDirection * punchForce, ForceMode.Impulse);
                ScoreSystem.Instance.AddScore(punchableObjects.gameObject.tag);
                SoundManager.Instance.PlaySound(punchSound.clip);
            }
        }

        PunchEffect();

        Invoke("ResetPunchCooldown", punchCooldown);
    }

    private void PunchEffect()
    {
        //// Punch Effects (punchSimulator should be a particle system)
        //if (punchSimulator != null) {
        //    Instantiate(punchSimulator, punchPoint, Quaternion.identity);
        //}
    }
    #endregion

    #region GRAB AND THROW
    private void GrabAndThrowInput()
    {
        if (isCharging) return;

        if (hasGrabbed) {
            if (Input.GetMouseButtonDown(1)) {
                OnThrow?.Invoke();
            }
        }else {
            if (grabOnCooldown) return;

            if (Input.GetMouseButtonDown(1)) {
                Debug.Log("Grab Input");
                TryGetObjectToThrow(out GameObject objectToThrow);
                grabbedObject = objectToThrow;
                if (grabbedObject != null) {
                    objectScoringScript = grabbedObject.GetComponent<ObjectScoring>();
                    GrabObject();
                }
            }
        }
    }

    #region GRAB
    private bool TryGetObjectToThrow(out GameObject objectToThrow)
    {
        Debug.Log("Trying to get object to throw");
        objectToThrow = null;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Grabbable"))) {
            Debug.Log("Found object to throw: " + hit.collider.gameObject.name);
            if (!ClickIsInRange(hit.collider.transform)) return false;
            objectToThrow = hit.collider.gameObject;
            return true;
        }
        Debug.Log("No object to throw");
        objectToThrow = null;
        return false;
    }

    private void GrabObject()
    {
        hasGrabbed = true;
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        Collider col = grabbedObject.GetComponent<Collider>();
        if (rb != null) {
            rb.isKinematic = true;
            col.enabled = false;
            grabbedObject.transform.position = grabPos.position;
            grabbedObject.transform.parent = grabPos;
            // objectScoringScript.isGrabbed = true;

            OnGrab?.Invoke();
        }
    }

    private bool ClickIsInRange(Transform targetObject)
    {
        return Vector3.Distance(targetObject.position, transform.position) <= grabRadius;
    }
    #endregion

    #region THROW
    public void ThrowObject()
    {
        hasGrabbed = false;
        grabOnCooldown = true;

        Debug.Log("GRABBED OBJECT: " + grabbedObject.name);
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        Collider col = grabbedObject.GetComponent<Collider>();
        if (rb != null) {
            grabbedObject.transform.parent = null;
            rb.isKinematic = false;
            col.enabled = true;

            Vector3 throwDirection = throwTarget.position - grabPos.position;
            rb.AddForce(throwDirection.normalized * throwForce, ForceMode.Impulse);
            grabbedObject = null;
            SoundManager.Instance.PlaySound(throwSound.clip);
        }

        ThrowEffect();

        Invoke("ResetThrowCooldown", throwCooldown);
    }

    private void ThrowEffect()
    {

    }

    #endregion

    #endregion

    #region CHARGED THROW

    private void ChargedThrowInput()
    {
        if (Input.GetKeyDown(KeyCode.X) && !chargeOnCooldown && hasGrabbed) {
            if (!isCharging) {
                isCharging = true;
                chargeTimer = chargeTime;
                OnCharge?.Invoke();
            }else {
                isCharging = false;
                OnChargedThrow?.Invoke();
            }
        }
    }

    public void ChargedThrow()
    {
        hasGrabbed = false;
        chargeOnCooldown = true;

        Debug.Log("GRABBED OBJECT: " + grabbedObject.name);
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        Collider col = grabbedObject.GetComponent<Collider>();
        if (rb != null) {
            grabbedObject.transform.parent = null;
            rb.isKinematic = false;
            col.enabled = true;

            Vector3 throwDirection = throwTarget.position - grabPos.position;
            rb.AddForce(throwDirection.normalized * chargedThrowForce, ForceMode.Impulse);
            grabbedObject = null;
        }

        Invoke("ResetChargedThrowCooldown", chargedThrowCooldown);
    }

    #endregion


    private void ResetPunchCooldown() => punchOnCooldown = false;
    private void ResetThrowCooldown() => grabOnCooldown = false;
    private void ResetChargedThrowCooldown() => chargeOnCooldown = false;
    private void ChargeHoldTimer()
    {
        if (isCharging) chargeTimer -= Time.deltaTime;
        if (chargeTimer <= 0) {
            isCharging = false;
            chargeTimer = chargeTime;
            OnChargedThrow?.Invoke();
        }
    }

    private void EnableDoubleJump()
    {
        if (!TeddyMovement.Instance.allowDoubleJump)
            TeddyMovement.Instance.allowDoubleJump = true;
    }

    private void Timers()
    {
        ChargeHoldTimer();
    }

    private void UpdateProgressionStage()
    {
        progressStage = ProgressionSystem.Instance.GetCurrentStage();
    }
}
