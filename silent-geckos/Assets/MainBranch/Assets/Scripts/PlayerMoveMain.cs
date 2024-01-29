﻿using UnityEngine;

public class PlayerMoveMain : MonoBehaviour
{
    [SerializeField] private AudioClip clip;

    private Rigidbody2D PlayerBody;

    [SerializeField] private Animator animator;

    [Header("Set Player Forces")]

    [SerializeField] private float movementSpeed = 6.36f;
    [SerializeField] private float jumpForce;

    [Header("Player Collision Checks")]
    //[SerializeField] private Transform ceilingCheck; // -- State not used ! -- //
    //[SerializeField] private Transform groundCheck;

    [Header("Define Collision Layer to Check")]
    [SerializeField] private LayerMask jumpableObjects;

    [Header("Player Jump Information")]
    [SerializeField] private int maxJumpCount = 0;
    [SerializeField] private int jumpsAvailable;
    [Tooltip("Makes A circle with Radius : __ :  From   groundCheck  and check if Radius Inside or Touching jumpableObjects ")]
    [SerializeField] private float checkRadius;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isMovementEnabled;
    [Header("Unlocks")] [SerializeField] private Unlock movementSpeed1;
    [SerializeField] private Unlock movementSpeed2;
    [SerializeField] private Unlock doubleJump;

    //[Header("Player Direction")]
    private bool facingRight = true;
    private float moveDirection;
    private bool gravityState; // Gets GravityisFlipped State
    private float value;

    // [SerializeField]  - See Private Values Within The Inspector Editor
    // [Header("Text")]  - Organizes Inspector into a list seperating other values where nessecary
    // [Tooltip("text")] - Hover Over an Element Such as Variable, States Its use as A TextBox Tooltip on top of the Value.
    // Debug.Log("text"); - Displays Information Within our Console

    private void Awake()
    {
        PlayerBody = GetComponent<Rigidbody2D>();
        jumpsAvailable = maxJumpCount;
    }

    int verticalScale(bool Bool)
    {
        if (Bool) return -1;
        if (!Bool) return 1;
        else return 0;
    }

    void Update()
    {
        if (doubleJump.isUnlocked) maxJumpCount = 1;
        MovementSpeedUnlock();
        Processinputs();
        Animate();
        if (PlayerBody.velocity.x != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
        if (PlayerBody.velocity.y * verticalScale(gravityState) > 0)
        {
            animator.SetBool("isGoingUp", true);
        }
        else
        {
            animator.SetBool("isGoingUp", false);
        }
        if (PlayerBody.velocity.y * verticalScale(gravityState) < 0)
        {
            animator.SetBool("isGoingDown", true);
        }
        else
        {
            animator.SetBool("isGoingDown", false);
        }
    }

    private void MovementSpeedUnlock()
    {
        if (movementSpeed1.isUnlocked) movementSpeed = 7f;
        if (movementSpeed2.isUnlocked) movementSpeed = 7.7f;
    }

    private void Processinputs()
    {
        isMovementEnabled = Dash.isMovementEnabled;
        gravityState = FlipScript.GravityIsFlipped;
        moveDirection = Input.GetAxis("Horizontal"); // Scale of -1 to 1
        if (Input.GetButtonDown("Jump") && (jumpsAvailable > 0 || isGrounded))
        {
            isJumping = true;
        }
    }

    private void FixedUpdate() //Called multiples time per frame
    {
        GameObject groundCheck = GameObject.FindWithTag("GroundCheck");
        isGrounded = Physics2D.IsTouchingLayers(groundCheck.GetComponentInChildren<BoxCollider2D>(), jumpableObjects);
        if (isGrounded && !isJumping)
        {
            jumpsAvailable = maxJumpCount;
        }
        Move();
    }

    private void Move()
    {
        if (isMovementEnabled)
        {
            PlayerBody.velocity = new Vector2(moveDirection * movementSpeed, PlayerBody.velocity.y);
            if (isJumping && (jumpsAvailable > 0 || isGrounded))
            {
                if (AudioController.instance != null)
                {
                    // Should be fixed in the future currently has issues
                    AudioController.instance.PlaySFX(clip);
                }

                PlayerBody.velocity = new Vector3(PlayerBody.velocity.x, 0, 0);
                if (gravityState) value = -jumpForce; else value = jumpForce;
                PlayerBody.AddForce(new Vector2(0f, value), ForceMode2D.Impulse);
                if (!isGrounded) jumpsAvailable--;
                isJumping = false;
            }
        }
    }



    private void Animate()
    {
        if (moveDirection > 0 && !facingRight)
        {
            FlipPlayerDirection();
        }
        else if (moveDirection < 0 && facingRight)
        {
            FlipPlayerDirection();
        }
    }

    private void FlipPlayerDirection()
    {
        facingRight = !facingRight; // Inverse bool
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, 1);
    }
}
