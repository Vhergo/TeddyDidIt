using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    [Space(10)]
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;

    [Header("Punch")]
    [SerializeField] private GameObject punchSimulator;
    [SerializeField] private float punchCooldown = 0.5f;
    [SerializeField] private float punchForce = 10f;
    [SerializeField] private float punchRadius = 2f;
    [SerializeField] private AudioSource punchSound;
    private bool punchOnCooldown;

    [Header("Grab and Throw"), Space(10)]
    [SerializeField] private Transform throwTarget;
    [SerializeField] private Transform grabPos;
    [SerializeField] private Transform throwPrepPos;
    [SerializeField] private Transform throwPos;
    [Space(10)]
    [SerializeField] private float grabRadius = 2f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float cooldownTime = 1f;
    [SerializeField] private AudioSource throwSound;
    private bool onCooldown;
    private bool hasGrabbed;

    private ObjectScoring objectScoringScript;

    private GameObject grabbedObject;

    private ProgressStage progressStage;

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
    }

    private void Update()
    {
        HandleCombatInput();
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
                GrabAndThrowInput();
                break;
            case ProgressStage.DoubleJump:
                GrabAndThrowInput();
                break;
            case ProgressStage.ChargeThrow:
                // Implement charged throw input handling if different from normal throw
                break;
        }
    }

    #region PUNCH
    private void PunchInput()
    {
        if (Input.GetMouseButtonDown(0) && !punchOnCooldown) {
            Punch();
        }
    }

    private void Punch()
    {
        Debug.Log("PUNCH");
        punchOnCooldown = true;
        punchSound.Play();

        Vector3 punchDirection = (throwTarget.position - grabPos.position).normalized;
        Vector3 punchPoint = grabPos.position + punchDirection * punchRadius;

        Collider[] colliders = Physics.OverlapSphere(punchPoint, punchRadius);
        foreach (Collider punchableObjects in colliders) {
            Rigidbody rb = punchableObjects.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddForce(punchDirection * punchForce, ForceMode.Impulse);
            }
        }

        // Punch Effects (punchSimulator should be a particle system)
        if (punchSimulator != null) {
            Instantiate(punchSimulator, punchPoint, Quaternion.identity);
        }

        Invoke("ResetPunchCooldown", punchCooldown);
    }
    #endregion

    #region GRAB AND THROW
    private void GrabAndThrowInput()
    {
        if (hasGrabbed) {
            if (Input.GetMouseButtonDown(0)) {
                ThrowObject();
            }
        }

        if (onCooldown) return;

        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Grab Input");
            TryGetObjectToThrow(out GameObject objectToThrow);
            grabbedObject = objectToThrow;
            if (grabbedObject != null)
            {
                objectScoringScript = grabbedObject.GetComponent<ObjectScoring>();
                GrabObject();
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

    private bool ClickIsInRange(Transform targetObject)
    {
        return Vector3.Distance(targetObject.position, transform.position) <= grabRadius;
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
            objectScoringScript.isGrabbed = true;
        }
        StartCoroutine(HandGrabAnimation());
    }

    private IEnumerator HandGrabAnimation()
    {
        float objectWidth = grabbedObject.GetComponent<Renderer>().bounds.size.x;
        Vector3 rightHandPos = new Vector3(objectWidth / 2, 0, 0);
        Vector3 leftHandPos = new Vector3(-objectWidth / 2, 0, 0);

        while (Vector3.Distance(rightHand.position, rightHandPos) > 0.01f) {
            rightHand.position = Vector3.Lerp(rightHand.position, rightHandPos, 0.1f);
            leftHand.position = Vector3.Lerp(leftHand.position, leftHandPos, 0.1f);
            yield return null;
        }
        rightHand.position = rightHandPos;
        leftHand.position = leftHandPos;
    }
    #endregion

    #region THROW
    private void ThrowObject()
    {
        hasGrabbed = false;
        onCooldown = true;

        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        Collider col = grabbedObject.GetComponent<Collider>();
        if (rb != null) {
            grabbedObject.transform.parent = null;
            rb.isKinematic = false;
            col.enabled = true;

            Vector3 throwDirection = throwTarget.position - grabPos.position;
            rb.AddForce(throwDirection.normalized * throwForce, ForceMode.Impulse);
            grabbedObject = null;
            throwSound.Play();
        }

        Invoke("ResetThrowCooldown", cooldownTime);
    }

    private void ResetThrowCooldown() => onCooldown = false;
    private void ResetPunchCooldown() => punchOnCooldown = false;

    //private IEnumerator HandThrowPrepAnimation()
    //{
    //    while (Vector3.Distance(grabPos.position, throwPrepPos.position) > 0.01f) {
    //        rightHand.position = Vector3.Lerp(rightHand.position, rightHandThrowPrepPosition.position, 0.1f);
    //        leftHand.position = Vector3.Lerp(leftHand.position, rightHandThrowPrepPosition.position, 0.1f);
    //        yield return null;
    //    }
    //    rightHand.position = rightHandThrowPrepPosition.position;
    //    leftHand.position = rightHandThrowPrepPosition.position;

    //    StartCoroutine(HandThrowAnimation());
    //}

    //private IEnumerator HandThrowAnimation()
    //{
    //    while (Vector3.Distance(rightHand.position, rightHandThrowPosition.position) > 0.01f) {
    //        rightHand.position = Vector3.Lerp(rightHand.position, rightHandThrowPosition.position, 0.1f);
    //        leftHand.position = Vector3.Lerp(leftHand.position, rightHandThrowPosition.position, 0.1f);
    //        yield return null;
    //    }
    //    rightHand.position = rightHandThrowPosition.position;
    //    leftHand.position = rightHandThrowPosition.position;
    //}

    #endregion

    #endregion

    private void UpdateProgressionStage()
    {
        progressStage = ProgressionSystem.Instance.GetCurrentStage();
    }
}
