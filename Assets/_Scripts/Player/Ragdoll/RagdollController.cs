using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private PlayerControls playerControls;
    private InputAction move;
    private InputAction jump;

    [Header("Components")]
    [SerializeField] private Rigidbody hips; //from ragdoll model
    [SerializeField] private Animator ani; //from animation modle

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 20f;
    private Vector3 moveDirection;
    private bool facingLeft = false;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 48f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 groundDetectionSize = new(1f, 0.2f, 1f);
    private bool canJump = true;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
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

        jump.performed -= JumpInput;
    }

    void Update()
    {
        ProccessInput();
    }

    void FixedUpdate()
    {
        Move();
    }

    void ProccessInput()
    {
        moveDirection = move.ReadValue<Vector2>().normalized;
    }

    void JumpInput(InputAction.CallbackContext context) => Jump();

    void Move()
    {
        float targetSpeed = moveDirection.x * maxSpeed; //calculate speed
        AnimateMovement(targetSpeed); //animate
        hips.velocity = new(0, hips.velocity.y, targetSpeed); //move player
    }

    void AnimateMovement(float speed)
    {
        //not moving
        if (speed == 0)
        {
            //animate idle in direction
            if (!facingLeft)
            {
                ani.Play("TeddyIdleRight", 0);
            }
            else
            {
                ani.Play("TeddyIdleLeft", 0);
            }
            hips.velocity = new(0, hips.velocity.y); // freeze x to prevent sliding
            return;
        }

        //player is moving
        //check if direction is changing
        if (speed < 0 && !facingLeft)
        {     
            ani.Play("TeddyTurnLeft", 1);           
            facingLeft = true;
        }
        //changing direction to right
        else if (speed > 0 && facingLeft)
        {          
            ani.Play("TeddyTurnRight", 1);   
            facingLeft = false;
        }

        //animate walk in direction
        if (facingLeft)
        {
            ani.Play("TeddyWalkLeft", 0);
        }
        else
        {
            ani.Play("TeddyWalkRight", 0);
        }
    }

    void Jump()
    {
        if (CanJump())
        {
            StartCoroutine(JumpCD());
            hips.velocity = new(hips.velocity.x, jumpForce);
        }
    }

    private bool CanJump()
    {
        if (!IsGrounded() || canJump == false) return false;

        return true;
    }

    private bool IsGrounded()
    {
        return Physics.CheckBox(groundCheck.position, groundDetectionSize / 2, Quaternion.identity, groundLayer);
    }

    private IEnumerator JumpCD()
    {
        canJump = false;
        yield return new WaitForSeconds(0.5f);
        canJump = true;
    }
}
