using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState //Not a superstate but is not part of any superstate
{
    //Input
    private int xInput;
    private bool jumpInput;
    private bool jumpInputStop;
    private bool grabInput;
    private bool dashInput;

    //Checks
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isTouchingWallBack;
    private bool isTouchingLedge;
    private bool isInCoyoteTime;
    private bool isInWallJumpCoyoteTime;
    private bool isTouchingWallPreviousFrame; //These previous frame checks are used to determine if we should start the wall jump coyote time
    private bool isTouchingWallBackPreviousFrame;
    private float wallJumpCoyoteTimeStart;
    private bool isJumping;

    public PlayerInAirState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, PlayerData playerData) : base(entity, stateMachine, animBoolName, playerData)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTouchingWallPreviousFrame = isTouchingWall;
        isTouchingWallBackPreviousFrame = isTouchingWallBack;

        isGrounded = player.CheckIfGrounded();
        isTouchingWall = player.CheckIfTouchingWall();
        isTouchingWallBack = player.CheckIfTouchingWallBack();
        isTouchingLedge = player.CheckForLedge();

        if (isTouchingWall && !isTouchingLedge)
        {
            player.LedgeClimbState.SetDetectedPosition(player.transform.position);
        }

        if (!isInWallJumpCoyoteTime && !isTouchingWall && !isTouchingWallBack && (isTouchingWallPreviousFrame || isTouchingWallBackPreviousFrame)) //If we are not touching any walls but were touching at least one wall last frame, start wall jump coyote time
            StartWallJumpCoyoteTime();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        //These 4 lines are to make sure the player doesn't activate inAirCoyoteTime right after performing a wall jump.
        //If these bools weren't reset, we risk activating coyote time by accident in the dochecks function called in Enter() right after performing a wall jump
        isTouchingWallPreviousFrame = false;
        isTouchingWallBackPreviousFrame = false;
        isTouchingWall = false;
        isTouchingWallBack = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CheckCoyoteTime();
        CheckWallJumpCoyoteTime();

        xInput = player.InputHandler.NormalizedInputX;
        jumpInput = player.InputHandler.JumpInput;
        jumpInputStop = player.InputHandler.JumpInputStop;
        grabInput = player.InputHandler.GrabInput;
        dashInput = player.InputHandler.DashInput;

        CheckJumpMultiplier();

        if (isGrounded && player.CurrentVelocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.LandState);
        }
        else if (isTouchingWall && !isTouchingLedge && !isGrounded) //Ledge climb
        {

            stateMachine.ChangeState(player.LedgeClimbState);
        }
        else if (jumpInput && (isTouchingWall || isTouchingWallBack || isInWallJumpCoyoteTime)) //Wall jump
        {
            StopWallJumpCoyoteTime();
            isTouchingWall = player.CheckIfTouchingWall(); //We need to manually set this again since isTouchingWall is normally checked in fixed update but this code is ran in update. Without this, isTouchingWall might sometimes not be updated, causing the player to wall jump towards the wrong direction.
            player.WallJumpState.DetermineWallJumpDirection(isTouchingWall);
            stateMachine.ChangeState(player.WallJumpState);
        }
        else if (jumpInput && player.JumpState.CanJump) //Jumping in air (double jump etc.)
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if (isTouchingWall && xInput == player.FacingDirection && player.CurrentVelocity.y <= 0) //If player input towards wall, wall slide (also make sure player is falling before entering wall slide)
        {
            stateMachine.ChangeState(player.WallSlideState);
        }
        else if (dashInput && player.DashState.CheckIfCanDash)
        {
            stateMachine.ChangeState(player.DashState);
        }
        else
        {
            player.CheckIfShouldFlip(xInput);
            player.SetVelocityX(playerData.moveSpeed * xInput);
            player.anim.SetFloat("yVelocity", player.CurrentVelocity.y);
            player.anim.SetFloat("xVelocity", Mathf.Abs(player.CurrentVelocity.x));
        }
    }

    private void CheckJumpMultiplier()
    {
        if (isJumping)
        {
            if (jumpInputStop)
            {
                isJumping = false;
                player.SetVelocityY(player.CurrentVelocity.y * playerData.variableJumpHeightMultiplier);
            }
            else if (player.CurrentVelocity.y <= 0)
                isJumping = false;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void StartCoyoteTime() => isInCoyoteTime = true;

    private void CheckCoyoteTime()
    {
        if (isInCoyoteTime && Time.time > startTime + playerData.coyoteTime)
        {
            isInCoyoteTime = false;
            player.JumpState.DecreaseAmountOfJumpsLeft();
        }
    }

    public void StartWallJumpCoyoteTime()
    {
        isInWallJumpCoyoteTime = true;
        wallJumpCoyoteTimeStart = Time.time;
    }

    public void StopWallJumpCoyoteTime() => isInWallJumpCoyoteTime = false;

    private void CheckWallJumpCoyoteTime()
    {
        if (isInWallJumpCoyoteTime && Time.time > wallJumpCoyoteTimeStart + playerData.coyoteTime)
        {
            StopWallJumpCoyoteTime();
        }
    }

    public void SetIsJumping() => isJumping = true;
}
