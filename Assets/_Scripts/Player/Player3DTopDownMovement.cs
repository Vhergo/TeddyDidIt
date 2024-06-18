using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Player3DTopDownMovement : MonoBehaviour
{
    public static Player3DTopDownMovement Instance { get; private set; }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float deceleration = 1f;
    private Vector3 moveDirection;
    private Vector3 mousePos;

    private bool isMoving = false;
    public bool IsMoving { get { return isMoving; } }

    [HideInInspector] public bool canMove;


    [SerializeField] private float dashForce = 12f;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private Slider dashIndicator;
    private bool isDashing;
    private bool isDead;
    public bool IsDashing { get { return isDashing; } }

    private Rigidbody rb;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        canMove = true;
        isDead = false;
    }

    private void Update()
    {
        ProcessInput();
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        if (canMove) Move();
    }

    private void ProcessInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        isMoving = Equals(moveDirection, Vector3.zero) ? false : true;

        if (Input.GetKey(KeyCode.Space) && !isDashing && !isDead) {
            StartCoroutine(Dash());
        }
    }

    private void Move()
    {
        Vector3 targetVelocity = moveDirection * moveSpeed;
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        if (moveDirection == Vector3.zero) {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        StartCoroutine(DashCooldown());

        Vector3 mouseDirection = GetMouseDirection();
        Vector3 dashDirection = (moveDirection == Vector3.zero) ? mouseDirection : moveDirection;
        rb.velocity = dashDirection * dashForce;

        yield return new WaitForSeconds(dashDuration);
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(dashCooldown - dashDuration);
        isDashing = false;
    }

    private IEnumerator DashCooldown()
    {
        float cooldownTimer = 0;

        while (cooldownTimer < dashCooldown) {
            cooldownTimer += Time.deltaTime;
            if (dashIndicator != null) dashIndicator.value = cooldownTimer / dashCooldown;
            yield return new WaitForEndOfFrame();
        }
        cooldownTimer = dashCooldown;
        if (dashIndicator != null) dashIndicator.value = cooldownTimer / dashCooldown;
    }

    private Vector3 GetMouseDirection()
    {
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
        return (mousePos - transform.position).normalized;
    }

    private void PlayerDeath()
    {
        rb.velocity = Vector3.zero;
        isDead = true;
    }
}
