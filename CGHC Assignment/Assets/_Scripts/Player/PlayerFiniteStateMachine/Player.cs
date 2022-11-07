using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    #region Assignables
    [Header("Base Data")]
    [SerializeField] private PlayerData playerData;

    [Header("Assignables")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private Transform dashDirectionIndicator;

    public PlayerInputHandler InputHandler { get; private set; }
    public BoxCollider2D MovementCollider { get; private set; }
    public Transform DashDirectionIndicator => dashDirectionIndicator;
    #endregion

    #region States
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallGrabState WallGrabState { get; private set; }
    public PlayerWallClimbState WallClimbState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerLedgeClimbState LedgeClimbState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerCrouchIdleState CrouchIdleState { get; private set; }
    public PlayerCrouchMoveState CrouchMoveState { get; private set; }
    #endregion

    #region Unity Callback Functions
    protected override void Awake()
    {
        base.Awake();

        IdleState = new PlayerIdleState(this, StateMachine, "idle", playerData);
        MoveState = new PlayerMoveState(this, StateMachine, "move", playerData);
        JumpState = new PlayerJumpState(this, StateMachine, "inAir", playerData); //inAir as animBoolName to share same blend tree with InAirState
        InAirState = new PlayerInAirState(this, StateMachine, "inAir", playerData);
        LandState = new PlayerLandState(this, StateMachine, "land", playerData);
        WallSlideState = new PlayerWallSlideState(this, StateMachine, "wallSlide", playerData);
        WallGrabState = new PlayerWallGrabState(this, StateMachine, "wallGrab", playerData);
        WallClimbState = new PlayerWallClimbState(this, StateMachine, "wallClimb", playerData);
        WallJumpState = new PlayerWallJumpState(this, StateMachine, "inAir", playerData);
        LedgeClimbState = new PlayerLedgeClimbState(this, StateMachine, "ledgeClimbState", playerData);
        DashState = new PlayerDashState(this, StateMachine, "inAir", playerData);
        CrouchIdleState = new PlayerCrouchIdleState(this, StateMachine, "crouchIdle", playerData);
        CrouchMoveState = new PlayerCrouchMoveState(this, StateMachine, "crouchMove", playerData);
    }

    protected override void Start()
    {
        base.Start();

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        InputHandler = GetComponent<PlayerInputHandler>();
        MovementCollider = GetComponent<BoxCollider2D>();

        StateMachine.Inititalize(IdleState);
    }
    #endregion

    #region Check Functions

    public void CheckIfShouldFlip(int xInput)
    {
        if (xInput != 0 && xInput != FacingDirection)
        {
            Flip();
        }
    }

    public bool CheckIfGrounded() => Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);

    public bool CheckIfTouchingWall() => Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);

    public bool CheckIfTouchingWallBack() => Physics2D.Raycast(wallCheck.position, Vector2.right * -FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround); //Touching a wall from behind

    public bool CheckForLedge() => Physics2D.Raycast(ledgeCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);

    public bool CheckForCeiling() => Physics2D.OverlapCircle(ceilingCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);

    #endregion

    #region Other Functions

    public void SetColliderHeight(float height)
    {
        Vector2 center = MovementCollider.offset;
        vector2Workspace.Set(MovementCollider.size.x, height);

        center.y += (height - MovementCollider.size.y) / 2; //Shift the offset so that the bottom of the collider remains on the ground. Every 1 unit the collider is shrinked, the offset needs to be lowered by 0.5 unit. The minus sign handles if the collider should be shrinked or growed.

        MovementCollider.size = vector2Workspace;
        MovementCollider.offset = center;
    }

    public Vector2 DetermineLedgeCornerPosition()
    {
        RaycastHit2D xHit = Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
        float xDist = xHit.distance; //Distance of the raycast hit position from the raycast origin used to determine the position of the ledge corner

        vector2Workspace.Set((xDist + 0.015f) * FacingDirection, 0f);
        RaycastHit2D yHit = Physics2D.Raycast(ledgeCheck.position + (Vector3)(vector2Workspace), Vector2.down, ledgeCheck.position.y - wallCheck.position.y + 0.015f, playerData.whatIsGround);
        //By offsetting our y raycast with the x distance, we ensure that we can fire a vertical raycast to hit the ledge corner and determine the y position of the ledge
        float yDist = yHit.distance;

        vector2Workspace.Set(wallCheck.position.x + (xDist * FacingDirection), ledgeCheck.position.y - yDist); //Final determined corner position
        return vector2Workspace;
    }

    private void AnimationTrigger() => ((PlayerState)StateMachine.CurrentState).AnimationTrigger(); //Casting required to call this function since it is exclusive to the child class of State (PlayerState)

    private void AnimationFinishTrigger() => ((PlayerState)StateMachine.CurrentState).AnimationFinishTrigger();

    #endregion

    public override void Flip()
    {
        base.Flip();
        transform.Rotate(0f, 180f, 0f);
    }
}
