using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabAndThrowSystem : MonoBehaviour
{
    public static GrabAndThrowSystem Instance { get; private set; }

    [SerializeField] private Transform grabPos;
    [SerializeField] private Transform throwTarget;
    [SerializeField] private float grabRadius = 2f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float cooldownTime = 1f;
    private bool onCooldown;
    private bool hasGrabbed;

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
        ProgressionSystem.OnGrabAndThrowEnabled += EnableGrabAndThrow;
        ProgressionSystem.OnChargeThrowEnabled += EnableChargeThrow;
    }

    private void OnDisable()
    {
        ProgressionSystem.OnGrabAndThrowEnabled -= EnableGrabAndThrow;
        ProgressionSystem.OnChargeThrowEnabled -= EnableChargeThrow;
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
            case ProgressStage.GrabAndThrow:
            case ProgressStage.DoubleJump:
                GrabAndThrowInput();
                break;
            case ProgressStage.ChargeThrow:
                // Implement charged throw input handling if different from normal throw
                break;
        }
    }

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
            if (grabbedObject != null) GrabObject();
        }
    }

    private bool TryGetObjectToThrow(out GameObject objectToThrow)
    {
        Debug.Log("Trying to get object to throw");
        objectToThrow = null;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, LayerMask.GetMask("Grabbable"))) {
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
        }
    }

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
        }

        Invoke("ResetCooldown", cooldownTime);
    }

    private void ResetCooldown() => onCooldown = false;

    private void EnableGrabAndThrow()
    {
        progressStage = ProgressionSystem.Instance.GetCurrentStage();
    }

    private void EnableChargeThrow()
    {
        progressStage = ProgressionSystem.Instance.GetCurrentStage();
    }
}
