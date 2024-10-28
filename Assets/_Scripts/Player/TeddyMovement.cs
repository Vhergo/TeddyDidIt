using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeddyMovement : MonoBehaviour
{
    public static TeddyMovement Instance { get; private set; }

    [SerializeField] private PlayerControls playerControls;
    private InputAction move;
    private InputAction jump;
    private InputAction sprint;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator anim;

    [Header("Movement")]
    [SerializeField] private float maxWalkSpeed = 12f;
    [SerializeField] private float maxSprintSpeed = 16f;
    [SerializeField] private bool turnWithMovement;
    [SerializeField] private Transform mousePos;
    [SerializeField] private Cloth cape;
    [SerializeField] private float capeWind = 15f;
    private float moveSpeed;
    private Vector3 moveDirection;
    private bool facingLeft;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 40f; 
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 groundDetectionSize = new(1f, 0.2f, 1f);
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool inAir = false;
    [SerializeField] private float jumpAmplifier = 1f;
    [Space(10)]
    [SerializeField] private AudioClip jumpSoundNormal;
    [SerializeField] private AudioClip jumpSoundOnBed;
    private AudioClip jumpSound;
    [SerializeField] private AudioClip landSoundNormal;
    [SerializeField] private AudioClip landSoundShoesOn;
    private AudioClip landSound;
    [SerializeField] private ParticleSystem dustParticles;
    private bool disableMovement;


    public bool allowDoubleJump = false;
    [SerializeField] private float doubleJumpForce = 45f;
    [SerializeField] private int jumpLimit = 2;
    [SerializeField] private float coyoteTime = 0.22f;
    [SerializeField] private float jumpBuffer = 0.2f;
    private int jumpsLeft;
    private float jumpBufferTimer = 0;


    [Header("Gravity")]
    [SerializeField] private float gravityScale = 1.0f;
    [SerializeField] private ForceMode forceMode = ForceMode.Force;
    private bool disableGravity;

    private static float globalGravity = -9.81f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        rb.useGravity = false; //so custom gravity can be used

        move = playerControls.Player.Movement;
        jump = playerControls.Player.Jump;
        sprint = playerControls.Player.Sprint;

        move.Enable();
        jump.Enable();
        sprint.Enable();

        jump.performed += JumpInput;

        CombatSystem.OnCharge += Freeze;
        CombatSystem.OnChargedThrow += Unfreeze;
        PlayerGear.OnShoesEquipped += UpdateLandSound;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        sprint.Disable();

        jump.performed -= JumpInput; //read jump input

        CombatSystem.OnCharge -= Freeze;
        CombatSystem.OnChargedThrow -= Unfreeze;
        PlayerGear.OnShoesEquipped -= UpdateLandSound;
    }

    private void Start()
    {
        jumpsLeft = jumpLimit;
        jumpBufferTimer = jumpBuffer;
        jumpSound = jumpSoundNormal;
        landSound = landSoundNormal;

        disableMovement = true;
    }

    private void Update()
    {
        MoveInput(); //read movement input
        SprintInput();
        AnimateMovement(); //animate teddy

        CheckIfLanded();
        CheckIfWalkedOffEdge();
        Timers();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        Move();
    }

    #region MOVEMENT
    private void MoveInput() => moveDirection = move.ReadValue<Vector2>().normalized;
    private void SprintInput() => moveSpeed = sprint.ReadValue<float>() == 1 ? maxSprintSpeed : maxWalkSpeed;

    private void Move()
    {
        if (disableMovement) return;

        float targetSpeed = moveDirection.x * moveSpeed; //calculate speed     
        rb.velocity = new(targetSpeed, rb.velocity.y, 0); //move player
    }

    private void AnimateMovement()
    {
        if (disableMovement) return;

        if (turnWithMovement) TurnWithMovement();
        else TurnWithMouse();

        if (isJumping) return; // Prevent walk and idle anims overriding jump anim

        WalkAnimation();
        CapeAnimation();
    }

    private void WalkAnimation()
    {
        // Play idle anim if player is not moving
        if (moveDirection.x == 0) {
            anim.Play("TeddyIdle", 0); // Play idle animation
            rb.velocity = new(0, rb.velocity.y); // Freeze x to prevent sliding
            return;
        }

        // Check if player is walking forwards or backwards
        if ((facingLeft && moveDirection.x > 0) || (!facingLeft && moveDirection.x < 0)) {
            // Choose between walking backwards or sprinting backwards
            anim.Play(moveSpeed == maxWalkSpeed ? "TeddyWalkBackwards" : "TeddySprintBackwards", 0);
        } else {
            anim.Play(moveSpeed == maxWalkSpeed ? "TeddyWalk" : "TeddySprint", 0);
        }
    }

    private void TurnWithMovement()
    {
        //check if player direction is changing to play turn around anim
        if (moveDirection.x < 0 && !facingLeft) {
            anim.Play("TeddyTurnLeft", 1);
            facingLeft = true;
        }
        //changing direction to right
        else if (moveDirection.x > 0 && facingLeft) {
            anim.Play("TeddyTurnRight", 1);
            facingLeft = false;
        }
    }

    private void TurnWithMouse()
    {
        if (mousePos.position.x < transform.position.x && !facingLeft) {
            anim.Play("TeddyTurnLeft", 1);
            facingLeft = true;
        } else if (mousePos.position.x > transform.position.x && facingLeft) {
            anim.Play("TeddyTurnRight", 1);
            facingLeft = false;
        }
    }

    private void CapeAnimation()
    {
        if (facingLeft) cape.externalAcceleration = new Vector3(capeWind, 0, 0);
        else cape.externalAcceleration = new Vector3(-capeWind, 0, 0);
    }
    #endregion

    #region JUMP
    void JumpInput(InputAction.CallbackContext context) => TryJump();

    void TryJump()
    {
        if (disableMovement) return;

        if (CanJump())
        {
            Jump(jumpForce);
        }
        else if (CanDoubleJump())
        {
            Jump(doubleJumpForce);
        }
    }

    private void Jump(float jumpForce)
    {
        jumpsLeft--;
        isJumping = true; //ensure player can't jump until landed
        StartCoroutine(JumpTrigger()); //start cooldown for jump input
        anim.Play("TeddyJump", 0); //play jump animation
        rb.velocity = new(rb.velocity.x, jumpForce * jumpAmplifier); //add jump force

        TriggerJumpSound();
    }

    private void TriggerJumpSound()
    {
        SoundManager.Instance.PlaySound(jumpSound);
    }


    //wait to give time for player to jump so ground check doens't think player has landed immediately
    private IEnumerator JumpTrigger()
    {
        yield return new WaitForSeconds(0.5f);
        inAir = true;
    }

    private bool CanJump()
    {
        if (IsGrounded() || isJumping == false) return true;
        else if (isJumping == true) {
            jumpBufferTimer = jumpBuffer;
        }
        return false;
    }

    private bool CanDoubleJump()
    {
        if (allowDoubleJump && jumpsLeft > 0 && (inAir || isJumping) && !CombatSystem.Instance.HasGrabbed) return true;
        else return false;
    }

    public bool IsGrounded()
    {
        return Physics.CheckBox(groundCheck.position, groundDetectionSize / 2, Quaternion.identity, groundLayer);
    }
    
    // Check if teddy has landed after jumping so player can jump again
    private void CheckIfLanded()
    {
        if (inAir && IsGrounded())
        {
            JumpBuffer();
            inAir = false;
            isJumping = false;
            jumpsLeft = jumpLimit; //reset double jump

            JustLanded();
            CheckIfOnBed();
        }
    }

    private void CheckIfOnBed()
    {
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, 1f);
        foreach (Collider col in colliders) {
            if (col.CompareTag("Bed")) {
                SoundManager.Instance.PlaySound(jumpSoundOnBed);
            }
        }
    }

    private void JustLanded()
    {
        SoundManager.Instance.PlaySound(landSound);
        dustParticles.Play();
    }

    private void UpdateLandSound()
    {
        landSound = landSoundShoesOn;
    }

    //to ensure only one jump is allowed in the air
    //checks if teddy walks off a ledge without jumping
    private void CheckIfWalkedOffEdge()
    {
        if (jumpsLeft >= jumpLimit && isJumping == false && IsGrounded() == false)
        {
            inAir = true;
            Invoke("CoyoteTime", coyoteTime);
        }
    }

    private void CoyoteTime()
    {
        if (jumpsLeft >= jumpLimit && isJumping == false && IsGrounded() == false) {
            jumpsLeft--;
        }
    }

    private void JumpBuffer()
    {
        if (jumpBufferTimer > 0) Jump(jumpForce);
    }

    public void SetJumpAmplifier(float value) => jumpAmplifier = value;

    private void Timers() => jumpBufferTimer -= Time.deltaTime;

    #endregion
    
    private void ApplyGravity()
    {
        if (disableGravity) return;

        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, forceMode);
    }

    public void Unfreeze()
    {
        disableGravity = false;
        disableMovement = false;
    }

    public void Freeze()
    {
        disableGravity = true;
        disableMovement = true;
        rb.velocity = Vector3.zero;
        anim.Play("TeddyIdle", 0);
    }

    public float GetMovementAmplification()
    {
        if (moveDirection.x == 0) return 1f; // No amplification if player is not moving
        else {
            if (moveSpeed == maxSprintSpeed) return 1.25f;
            else return 1.1f;
        }
    }
}
