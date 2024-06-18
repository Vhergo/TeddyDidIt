using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(TrailRenderer))]
public class Player3DPlatformerMovement : MonoBehaviour
{
    [SerializeField] private bool isDead;
    public bool IsDead { get; set; }

    #region INPUTS
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;

    [SerializeField] private PlayerControls playerControls;
    private InputAction move;
    private InputAction jump;
    private InputAction dash;
    #endregion

    #region VARIABLES
    [Header("Available Controls")]
    [Tooltip("This is a test description for myVariable.")][SerializeField] private bool toggleJumpOff;
    [SerializeField] private bool toggleCrouchOff;
    [SerializeField] private bool toggleDoubleJumpOff;
    [SerializeField] private bool toggleDashOff;
    [SerializeField] private bool toggleWallGrabOff;
    [SerializeField] private bool toggleWallSlideOff;
    [SerializeField] private bool toggleWallClimbOff;
    [SerializeField] private bool toggleWallJumpOff;

    [Header("Components")]
    [SerializeField] public Rigidbody rb; // Set Public for Knockback
    [SerializeField] private TrailRenderer tr;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 groundDetectionSize = new Vector3(1f, 0.2f, 1f);

    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallDetectionRadius = 0.2f;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float decceleration = 5f;
    [SerializeField] private bool conserveMomentum;
    private float originalMaxSpeed;
    private float accelForce;
    private float deccelForce;

    // In-air speed control
    [Range(0.01f, 1)][SerializeField] private float accelInAir = 1f;
    [Range(0.01f, 1)][SerializeField] private float deccelInAir = 1f;
    [SerializeField] private float airTime = 0.1f;
    private float airTimeCounter;

    // Facing direction
    [SerializeField] public bool isFacingRight = false;
    [SerializeField] public bool isMovingRight = false; // public for the weapon rotation
    private Vector3 moveDirection;

    public Vector3 MoveDirection { get; set; }

    [Header("Jump")]
    [SerializeField] private float jumpForce = 48f;
    [SerializeField] private float jumpHeight = 6f;
    [SerializeField] private float jumpTimeToApex = 0.25f;

    [Range(0, 1)][SerializeField] private float lowJumpMult = 0.55f; // percentage decrease
    [Range(0, 1)][SerializeField] private float fallGravityMult = 0.2f; // percentage increase
    [SerializeField] private bool isJumping;

    [SerializeField] private float gravityStrength;
    [SerializeField] private float gravityScale;
    [SerializeField] private float maxFallSpeed = 50f;

    [SerializeField] private int jumpLimit = 1;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float tempJumpLimit;
    private float jumpCounter;
    private float coyoteTimeCounter = 0;
    private float jumpBufferCounter = 0;
    private bool jumpInput;
    private bool isFalling;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeed = 7f;
    [Range(0, 1)][SerializeField] private float crouchScaleAmount = 0.6f;
    private Vector3 originalCrouchScale;
    private Vector3 newCrouchScale;
    private bool crouchInput;
    public bool isCrouching = false;

    [Header("Dash")]
    [SerializeField] private float dashForce = 4f;
    [SerializeField] private float dashTime = 0.08f;
    [SerializeField] private float dashCooldown = 0.5f;

    [SerializeField] private Vector3 dashDir;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isDashing = false;
    private float originalGravityScale;
    private bool dashInput;

    [Header("Wall Grab/Slide/Climb/Jump")]
    //Wall Grab
    [SerializeField] private float wallGrabTime = 4f;
    [SerializeField] private bool wallGrabInput = false;
    [SerializeField] private bool isWallGrabing = false;
    private float wallGrabCounter;
    private float tempWallGrabTime;

    //Wall Slide
    [SerializeField] private float wallSlideSpeed = 6f;
    [SerializeField] private bool wallSlideInput;
    [SerializeField] private bool isWallSliding = false;

    //Wall Climb
    [SerializeField] private float wallClimbSpeed = 6f;
    [SerializeField] private float wallClimbStaminaDrain = 1f;
    [SerializeField] private bool wallClimbInput = false;
    [SerializeField] private bool isWallClimbing = false;
    public float wallClimbDirection;

    //Wall Jump
    [SerializeField] private float wallJumpTime = 0.2f;
    [SerializeField] private float wallJumpDuration = 0.1f;
    [SerializeField] private Vector3 wallJumpForce = new Vector3(16, 20, 0);
    private float wallJumpCounter;
    private float wallJumpDirection;
    private bool wallJumpInput = false;

    #endregion

    private void Awake()
    {
        playerControls = new PlayerControls();
        ValidateAndInitialize();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Movement;
        jump = playerControls.Player.Jump;
        dash = playerControls.Player.Dash;

        move.Enable();
        jump.Enable();
        dash.Enable();

        jump.performed += JumpInput;
        dash.performed += DashInput;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        dash.Disable();
    }

    void Start()
    {
        originalGravityScale = rb.drag;

        originalCrouchScale = transform.localScale;
        originalMaxSpeed = maxSpeed;

        tempJumpLimit = jumpLimit;
        if (toggleDoubleJumpOff) jumpLimit = 1;
        jumpCounter = jumpLimit;

        tempWallGrabTime = wallGrabTime;
        if (toggleWallGrabOff) wallGrabTime = 0;
        wallGrabCounter = wallGrabTime;
    }

    void Update()
    {
        ProccessInput();
        Timers();
        Turn();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        if (!IsDead) Move();
        ControlToggles();
    }

    void JumpInput(InputAction.CallbackContext context) => jumpInput = true;
    void DashInput(InputAction.CallbackContext context) => dashInput = true;

    void ProccessInput()
    {
        moveDirection = move.ReadValue<Vector2>().normalized;

        wallClimbDirection = moveDirection.y;

        crouchInput = (Input.GetKey(KeyCode.LeftControl)) ? true : false;

        WallInteractions(moveDirection.x, moveDirection.y);

        isFalling = rb.velocity.y < 0 ? true : false;
        if (IsGrounded()) {
            if (isJumping) {
                if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(landSound);
            }

            isFalling = isJumping = false;
            jumpCounter = tempJumpLimit;
            airTimeCounter = airTime;
        }

        CoyoteTime();
        JumpBuffer();

        if (Input.GetButtonUp("Jump")) LowJump();
        if (rb.velocity.y < 0) FallMultiplier();
    }

    #region MOVEMENT
    void Move()
    {
        if (isCrouching) Mathf.Clamp(maxSpeed, 0, crouchSpeed);
        maxSpeed = isCrouching ? crouchSpeed : originalMaxSpeed;
        float targetSpeed = moveDirection.x * maxSpeed;

        float accelRate;
        if (airTimeCounter > 0) {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelForce : deccelForce;
        } else {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelForce * accelInAir : deccelForce * deccelInAir;
        }

        ConserveMomentum(targetSpeed, accelRate);

        float speedDif = targetSpeed - rb.velocity.x;
        float movement = speedDif * accelRate;

        rb.AddForce(Vector3.right * movement);
    }

    void Jump()
    {
        if ((jumpBufferCounter >= 0 && coyoteTimeCounter >= 0 && !isJumping) || (!IsGrounded() && isJumping && jumpCounter > 0)) {
            jumpInput = false;
            isJumping = true;
            jumpBufferCounter = 0;
            jumpCounter--;

            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(jumpSound);

            if (isCrouching) CrouchUp();
        }
    }

    void CrouchDown()
    {
        isCrouching = true;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * crouchScaleAmount, transform.localScale.z);
    }

    void CrouchUp()
    {
        isCrouching = false;
        transform.localScale = new Vector3(transform.localScale.x, originalCrouchScale.y, transform.localScale.z);
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        SetGravityScale(0);

        dashDir = moveDirection;
        if (dashDir == Vector3.zero) dashDir = new Vector3(-transform.localScale.x, 0, 0);
        rb.velocity = new Vector3(dashDir.x * maxSpeed * dashForce, 0, dashDir.z * maxSpeed * dashForce);
        tr.emitting = true;

        yield return new WaitForSeconds(dashTime);
        tr.emitting = false;
        SetGravityScale(originalGravityScale);
        isDashing = false;
        dashInput = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    #endregion

    #region WALL INTERACTIONS
    void WallGrab()
    {
        SetGravityScale(0);
        if (isWallClimbing && !toggleWallClimbOff) {
            WallClimb();
        } else {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }

    void WallSlide()
    {
        SetGravityScale(0);
        rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeed, rb.velocity.z);
    }

    void WallClimb()
    {
        SetGravityScale(0);
        if (wallClimbDirection != 0) {
            rb.velocity = new Vector3(rb.velocity.x, wallClimbDirection * wallClimbSpeed, rb.velocity.z);
        }
        wallGrabCounter -= wallClimbStaminaDrain * Time.deltaTime;
    }

    void ProcessWallJump()
    {
        if (isWallSliding) {
            if (wallJumpDuration < 0) wallJumpInput = false;
            wallJumpDirection = transform.localScale.x;
            wallJumpCounter = wallJumpTime;
        } else {
            wallJumpCounter -= Time.deltaTime;
        }

        if (jumpInput && wallJumpCounter > 0) {
            wallJumpInput = true;
            jumpInput = false;
        }
    }

    private IEnumerator WallJump()
    {
        rb.velocity = new Vector3(wallJumpDirection * wallJumpForce.x, wallJumpForce.y, rb.velocity.z);
        wallJumpCounter = 0;

        yield return new WaitForSeconds(wallJumpDuration);
        wallJumpInput = false;
    }
    #endregion

    #region HELPER FUNCTIONS

    void ControlToggles()
    {
        if (jumpInput && !toggleJumpOff) Jump();
        if (crouchInput && !toggleCrouchOff && !isCrouching && !isJumping) {
            CrouchDown();
        } else if (!crouchInput && !toggleCrouchOff && isCrouching) {
            CrouchUp();
        }
        if (dashInput && canDash && !toggleDashOff) StartCoroutine(Dash());
        if (isWallGrabing && !toggleWallGrabOff) WallGrab();
        if (isWallSliding && !toggleWallSlideOff) WallSlide();
        if (wallJumpInput && !toggleWallJumpOff) StartCoroutine(WallJump());
    }

    void ConserveMomentum(float targetSpeed, float accelRate)
    {
        bool fasterThanTargetSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed);
        bool checkDirection = Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed);
        bool targetSpeedAboveZero = Mathf.Abs(targetSpeed) > 0.01f;
        if (conserveMomentum && fasterThanTargetSpeed && checkDirection && targetSpeedAboveZero)
            accelRate = 0;
    }

    void WallInteractions(float horizontal, float vertical)
    {
        wallGrabInput = horizontal != 0 ? true : false;
        wallSlideInput = (wallGrabInput && toggleWallGrabOff) ? true : false;
        if (IsWalled() && !IsGrounded()) {
            if (wallGrabInput && wallGrabCounter > 0) {
                wallGrabCounter -= Time.deltaTime;
                isWallGrabing = true;
                isWallSliding = wallSlideInput ? true : false;
            } else {
                isWallSliding = true;
                isWallGrabing = false;
                if (!isDashing) SetGravityScale(originalGravityScale);
            }
        } else if (!IsWalled() && !IsGrounded()) {
            isWallSliding = false;
            isWallGrabing = false;
            if (!isDashing) SetGravityScale(originalGravityScale);
        } else {
            isWallSliding = false;
            isWallGrabing = false;
            wallGrabCounter = tempWallGrabTime;
            if (!isDashing) SetGravityScale(originalGravityScale);
        }
        wallClimbInput = vertical != 0 ? true : false;
        isWallClimbing = (IsWalled() && !IsGrounded() && wallClimbInput && wallGrabInput) ? true : false;
        ProcessWallJump();
    }

    void LowJump()
    {
        if (rb.velocity.y > 0) rb.AddForce(Vector3.down * rb.velocity.y * lowJumpMult, ForceMode.Impulse);
    }

    void FallMultiplier()
    {
        rb.AddForce(Vector3.up * rb.velocity.y * fallGravityMult / 10, ForceMode.Impulse);
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed), rb.velocity.z);
    }

    void CoyoteTime()
    {
        if (IsGrounded()) {
            coyoteTimeCounter = coyoteTime;
        } else {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    void JumpBuffer()
    {
        if (Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferTime;
        } else {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    void SetGravityScale(float gravityScale)
    {
        rb.drag = gravityScale;
    }

    private bool IsGrounded()
    {
        return Physics.CheckBox(groundCheck.position, groundDetectionSize / 2, Quaternion.identity, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics.CheckSphere(wallCheck.position, wallDetectionRadius, wallLayer);
    }

    void CheckFacingDirection()
    {
        isMovingRight = moveDirection.x > 0 ? true : false;
    }

    void Turn()
    {
        if (isDashing) return;
        if (MySceneManager.Instance != null && MySceneManager.Instance.gameState == GameState.Pause) return;

        if (moveDirection.x > 0 && !isFacingRight) {
            isFacingRight = true;
            Flip();
        } else if (moveDirection.x < 0 && isFacingRight) {
            isFacingRight = false;
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 playerScale = transform.localScale;
        playerScale.x *= -1;
        transform.localScale = playerScale;
    }

    void Timers()
    {
        airTimeCounter -= Time.deltaTime;
    }

    private void ValidateAndInitialize()
    {
        if (rb == null) {
            rb = GetComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Freeze X, Y, and Z rotation
            rb.constraints = RigidbodyConstraints.FreezePositionZ; // Freeze Z position
        }

        accelForce = (50 * acceleration) / maxSpeed;
        deccelForce = (50 * decceleration) / maxSpeed;

        acceleration = Mathf.Clamp(acceleration, 0.01f, maxSpeed);
        decceleration = Mathf.Clamp(decceleration, 0.01f, maxSpeed);

        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        gravityScale = gravityStrength / Physics.gravity.y;
        rb.drag = gravityScale;

        jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics.gravity.y * rb.drag));

        tempJumpLimit = toggleDoubleJumpOff ? 1 : jumpLimit;
        jumpCounter = tempJumpLimit;

        tempWallGrabTime = toggleWallGrabOff ? 0 : wallGrabTime;
        wallGrabCounter = tempWallGrabTime;
    }
    #endregion

    void OnValidate()
    {
        ValidateAndInitialize();
    }
}
