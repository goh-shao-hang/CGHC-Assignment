using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    [Header("Assignables")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int amountOfJumps = 2;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float movementForceInAir;
    [SerializeField] private float airDragMultiplier;
    [SerializeField] private float variableJumpHeightMultiplier = 0.5f;
    [SerializeField] private float wallHopForce;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private Vector2 wallHopDirection; //Not used
    [SerializeField] private Vector2 wallJumpDirection;
    [SerializeField] private float jumpBufferTime = 0.15f;
    [SerializeField] private float turnBufferTime = 0.1f;
    [SerializeField] private float wallJumpTimer = 0.5f;
    [SerializeField] private float ledgeClimbXOffset1;
    [SerializeField] private float ledgeClimbYOffset1;
    [SerializeField] private float ledgeClimbXOffset2;
    [SerializeField] private float ledgeClimbYOffset2;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float distanceBetweenImages;
    [SerializeField] private float dashCD;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private Vector2 knockbackSpeed;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float wallCheckDistance;

    private float movementInputDirection;
    private bool isFacingRight = true;
    private int facingDirection = 1; //-1 for left and 1 for right
    private bool isWalking;
    private bool isGrounded;
    private bool canJump;
    private bool canWallJump;
    private bool isTouchingWall;
    private bool isWallSliding;
    private int jumpsLeft;
    private float jumpBufferTimeLeft;
    private bool isAttemptingToJump = false;
    private bool checkJumpMultiplier; //Check if need to start responding to variable jump
    private bool canMove = true;
    private bool canFlip = true;
    private bool hasWallJumped;
    private int lastWallJumpDirection;
    private float turnBufferTimeLeft;
    private float wallJumpTimerLeft;
    private bool isTouchingLedge = false;
    private bool canClimbLedge = false;
    private bool ledgeDetected;
    private Vector2 ledgePosBot;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;
    private bool isDashing = false;
    private float dashTimeLeft;
    private float lastImageXPos;
    private float lastDashTime = -100f; //set to a negative value so that we can dash when the game starts
    private bool knockback;
    private float knockbackStartTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        jumpsLeft = amountOfJumps;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    private void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckJumpBuffer();
        CheckLedgeClimb();
        CheckDash();
        CheckKnockback();
    }

    private void FixedUpdate()
    {
        Move();
        CheckSurroundings();
    }

    private void CheckSurroundings() //Check for ground or walls
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);

        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, whatIsGround);
        if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.001f)
        {
            jumpsLeft = amountOfJumps;
        }

        if (isTouchingWall)
        {
            canWallJump = true;
        }

        if (jumpsLeft <= 0)
            canJump = false;
        else
            canJump = true;
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGrounded && movementInputDirection == facingDirection && rb.velocity.y < 0 && !canClimbLedge)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CheckLedgeClimb()
    {
        if (ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;

            if (isFacingRight) //Finds 2 point near the ledge. Pos 1 is the position to hold the player at while animation is playing while Pos 2 is the destination after the animation finishes
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }

            canMove = false;
            canFlip = false;

            anim.SetBool("canClimbLedge", canClimbLedge);
        }

        if (canClimbLedge)
        {
            transform.position = ledgePos1;
        }
    }

    public void FinishLedgeClimb()
    {
        canClimbLedge = false;
        transform.position = ledgePos2;
        canMove = true;
        canFlip = true;
        ledgeDetected = false;
        anim.SetBool("canClimbLedge", canClimbLedge);
    }

    public void Knockback(int direction) //public to call from combat controller
    {
        knockback = true;
        knockbackStartTime = Time.time;
        rb.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
    }

    private void CheckKnockback()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration && knockback)
        {
            knockback = false;
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (Mathf.Abs(rb.velocity.x) > 0.001f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || (jumpsLeft > 0 && !isTouchingWall))
                Jump();
            else
            {
                jumpBufferTimeLeft = jumpBufferTime;
                isAttemptingToJump = true;
            }
        }

        if (movementInputDirection != facingDirection && isWallSliding && !isGrounded) //If the character is currently wall sliding and tries to move away from the wall, briefly lock movement and flipping to give them time to try a walljump
        {
            canMove = false;
            canFlip = false;

            turnBufferTimeLeft = turnBufferTime;
        }

        if (turnBufferTimeLeft > 0)
        {
            turnBufferTimeLeft -= Time.deltaTime;

            if (turnBufferTimeLeft <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier); //Immediately cut current y velocity if the player lets go of jump key to create variable jump height. The earlier they cut, the lesser the velocity.
        }

        if (Input.GetButtonDown("Dash"))
        {
            if (Time.time > lastDashTime + dashCD)
                AttemptToDash();
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void Move()
    {
        if (!isGrounded && !isWallSliding && movementInputDirection == 0 && !knockback) //If player is in air and there is no input, slowly bring the character to a stop as if there is air resistance.
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y); //Multiply the xspeed by some value like 0.95 every frame to slow down gradually
        }
        else if (canMove && !knockback)
        {
            rb.velocity = new Vector2(movementInputDirection * speed, rb.velocity.y);
        }

        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void CheckJumpBuffer()
    {
        if (jumpBufferTimeLeft > 0)
        {
            if (!isGrounded && isTouchingWall && movementInputDirection != 0 && movementInputDirection != facingDirection)//Wall Jump
            {
                WallJump();
            }
            else if (isGrounded)
            {
                Jump();
            }
        }
        
        if (isAttemptingToJump)
            jumpBufferTimeLeft -= Time.deltaTime;

        if (wallJumpTimerLeft > 0)
        {
            if (hasWallJumped && movementInputDirection == -lastWallJumpDirection) //If the player attempts to return the direction they wall jumped from, cut their y velocity
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                hasWallJumped = false;
                wallJumpTimerLeft -= Time.deltaTime;
            }
        }
        else
        {
            hasWallJumped = false;
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); //Set xspeed to 0 if we want to use air force to move the player in x direction
            jumpsLeft--;
            jumpBufferTimeLeft = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
        }
    }

    private void WallJump()
    {
        if (canWallJump)
        {
            isWallSliding = false;
            jumpsLeft = amountOfJumps;
            jumpsLeft--;
            rb.velocity = new Vector2(rb.velocity.x, 0); //Reset y velocity before adding force so that the jump speed isn't affected by current y speed
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            jumpBufferTimeLeft = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            turnBufferTimeLeft = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimerLeft = wallJumpTimer;
            lastWallJumpDirection = -facingDirection;
        }
    }

    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDashTime = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXPos = transform.position.x;
    }

    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0 && !isTouchingWall)
            {
                canMove = false;
                canFlip = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, 0);
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXPos) >= distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXPos = transform.position.x;
                }
            }
            else
            {
                rb.velocity = new Vector2(speed * facingDirection, rb.velocity.y); //Optional, used to clamp to max speed right after dashing
                isDashing = false;
                canMove = true;
                canFlip = true;
            }
        }
    }

    public bool GetDashStatus()
    {
        return isDashing;
    }

    private void Flip()
    {
        if (!isWallSliding && canFlip && !knockback)
        {
            facingDirection *= -1; //flip sign of facing direction variable
            isFacingRight = !isFacingRight;
            transform.Rotate(0, 180f, 0);
        }
    }

    private void EnableFlip()
    {
        canFlip = true;
        canMove = true;
    }

    private void DisableFlip()
    {
        canFlip = false;
        canMove = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); //Ground check indicator

        Vector3 wallCheckDestination = wallCheck.position;
        if (isFacingRight)
            wallCheckDestination.x += wallCheckDistance;
        else
            wallCheckDestination.x -= wallCheckDistance;
        Gizmos.DrawLine(wallCheck.position, wallCheckDestination); //Wall check indicator

        Vector3 ledgeCheckDestination = ledgeCheck.position;
        if (isFacingRight)
            ledgeCheckDestination.x += wallCheckDistance;
        else
            ledgeCheckDestination.x -= wallCheckDistance;
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheckDestination); //Ledge check indicator
    }
}
