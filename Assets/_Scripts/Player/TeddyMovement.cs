using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeddyMovement : MonoBehaviour
{
    [SerializeField] private PlayerControls playerControls;
    private InputAction move;
    private InputAction jump;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator anim;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 10f;
    private Vector3 moveDirection;
    private bool facingLeft = false;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 60f; 
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 groundDetectionSize = new(1f, 0.2f, 1f);
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool inAir = false;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource landSound;
    private bool inJumpAnim = false;

    public bool allowDoubleJump = false;
    [SerializeField] private float doubleJumpForce = 50f;
    [SerializeField] private int jumpsLeft = 2;
    [SerializeField] private float walkedOffEdgeBufferTime = 0.25f;

    [Header("Gravity")]
    [SerializeField] private float gravityScale = 1.0f;
    [SerializeField] private ForceMode forceMode = ForceMode.Force;

    private static float globalGravity = -9.81f;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        rb.useGravity = false; //so custom gravity can be used

        move = playerControls.Player.Movement;
        jump = playerControls.Player.Jump;

        move.Enable();
        jump.Enable();

        jump.performed += JumpInput;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();

        jump.performed -= JumpInput; //read jump input
    }

    void Update()
    {
        MoveInput(); //read movement input
        AnimateMovement(); //animate teddy
        CheckIfLanded();
        CheckIfWalkedOffEdge();
    }

    void FixedUpdate()
    {
        //apply gravity with custom scale
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, forceMode);

        Move();
    }

    #region MOVEMENT
    void MoveInput()
    {
        moveDirection = move.ReadValue<Vector2>().normalized;
    }


    void Move()
    {
        float targetSpeed = moveDirection.x * maxSpeed; //calculate speed     
        rb.velocity = new(targetSpeed, rb.velocity.y, 0); //move player
    }

    void AnimateMovement()
    {
        //player is not jumping
        float speed = moveDirection.x;

        //check if player direction is changing to play turn around anim
        if (speed < 0 && !facingLeft)
        {
            anim.Play("TeddyTurnLeft", 1);
            facingLeft = true;
        }
        //changing direction to right
        else if (speed > 0 && facingLeft)
        {
            anim.Play("TeddyTurnRight", 1);
            facingLeft = false;
        }

        if (isJumping) return; //to prevent walk and idle anims overriding jump anim

        //play idle anim if player is not moving
        if (speed == 0)
        {
            anim.Play("TeddyIdle", 0); //play idle animation
            rb.velocity = new(0, rb.velocity.y); // freeze x to prevent sliding
            return;
        }

        //player is moving
        anim.Play("TeddyWalk", 0); //play walk animation
    }
    #endregion

    #region JUMP
    void JumpInput(InputAction.CallbackContext context) => TryJump();

    void TryJump()
    {
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
<<<<<<< Updated upstream
        rb.velocity = new(rb.velocity.x, jumpForce); //add jump force    
=======
        rb.velocity = new(rb.velocity.x, jumpForce); //add jump force
        SoundManager.Instance.PlaySound(jumpSound.clip);
>>>>>>> Stashed changes
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

        return false;
    }

    private bool CanDoubleJump()
    {
        if (allowDoubleJump && jumpsLeft > 0 && (inAir || isJumping)) return true;

        return false;
    }

    private bool IsGrounded()
    {
        return Physics.CheckBox(groundCheck.position, groundDetectionSize / 2, Quaternion.identity, groundLayer);
    }
    
    //check if teddy has landed after jumping so player can jump again
    private void CheckIfLanded()
    {
        if (inAir && IsGrounded())
        {
            inAir = false;
            isJumping = false;
            jumpsLeft = 2; //reset double jump
<<<<<<< Updated upstream
=======
            SoundManager.Instance.PlaySound(landSound.clip);
>>>>>>> Stashed changes
        }
    }

    //to ensure only one jump is allowed in the air
    //checks if teddy walks off a ledge without jumping
    private void CheckIfWalkedOffEdge()
    {
        if (jumpsLeft >= 2 && isJumping == false && IsGrounded() == false)
        {
            inAir = true;
            Invoke("WalkedOffEdgeBuffer", walkedOffEdgeBufferTime);
        }
    }

    private void WalkedOffEdgeBuffer()
    {
        if (jumpsLeft >= 2 && isJumping == false && IsGrounded() == false) {
            jumpsLeft--;
        }
    }
    #endregion
}
