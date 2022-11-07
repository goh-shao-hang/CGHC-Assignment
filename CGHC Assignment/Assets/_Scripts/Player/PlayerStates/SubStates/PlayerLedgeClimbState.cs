using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerState
{
    private Vector2 detectedPos;
    private Vector2 cornerPos;
    private Vector2 startPos;
    private Vector2 endPos;

    private bool isHanging;
    private bool isClimbing;
    private bool isTouchingCeiling; //Cast a ray from the ledge corner (not the player) to see if we need to stand or crouch after climbing a ledge. Different from the ceiling check function in the player

    private int xInput;
    private int yInput;
    private bool jumpInput;

    public PlayerLedgeClimbState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, PlayerData playerData) : base(entity, stateMachine, animBoolName, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocityZero();
        player.transform.position = detectedPos; //Lock player in this position before ledge climbing. We actually just want to use this position to do calculations.
        cornerPos = player.DetermineLedgeCornerPosition();

        //                substract here             because if we are facing right, we want to shift the xOffset to the left (by subtracting it) to get the startPos and vice versa. Reversed for endPos.
        startPos.Set(cornerPos.x - (player.FacingDirection * playerData.startOffset.x), cornerPos.y - playerData.startOffset.y);
        endPos.Set(cornerPos.x + (player.FacingDirection * playerData.startOffset.x), cornerPos.y + playerData.endOffset.y);

        player.transform.position = startPos;
    }
    
    public override void Exit()
    {
        base.Exit();

        isHanging = false;

        if (isClimbing)
        {
            player.transform.position = endPos;
            isClimbing = false;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            if (isTouchingCeiling)
            {
                stateMachine.ChangeState(player.CrouchIdleState); //Automatically crouches after a ledge climb if there is not enough space to stand
            }
            else
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
        else
        {
            xInput = player.InputHandler.NormalizedInputX;
            yInput = player.InputHandler.NormalizedInputY;
            jumpInput = player.InputHandler.JumpInput;

            player.SetVelocityZero();
            player.transform.position = startPos;

            if ((xInput == player.FacingDirection || yInput == 1) && isHanging && !isClimbing) //Start climbing if player inputs towards the ledge
            {
                CheckForCeilingAboveLedge();
                isClimbing = true;
                player.anim.SetBool("climbLedge", true);
            }
            else if ((yInput == -1 || xInput == -player.FacingDirection) && isHanging && !isClimbing) //Drop from ledge if player inputs down
            {
                stateMachine.ChangeState(player.InAirState);
            }
            else if (jumpInput && !isClimbing)
            {
                player.WallJumpState.DetermineWallJumpDirection(true);
                stateMachine.ChangeState(player.WallJumpState);
            }
        }
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

        isHanging = true;
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        player.anim.SetBool("climbLedge", false);
    }

    public void SetDetectedPosition(Vector2 pos) => detectedPos = pos;

                                                                                                    
    private void CheckForCeilingAboveLedge()
    {
                                                            //Offset upwards        Offset into ledge
        isTouchingCeiling = Physics2D.Raycast(cornerPos + (Vector2.up * 0.015f) + (Vector2.right * 0.015f * player.FacingDirection), Vector2.up, playerData.standColliderHeight, playerData.whatIsGround);
        player.anim.SetBool("isTouchingCeiling", isTouchingCeiling);
    }
}
