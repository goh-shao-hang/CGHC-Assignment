using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallGrabState : PlayerTouchingWallState
{
    private Vector2 holdPosition;

    public PlayerWallGrabState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, PlayerData playerData) : base(entity, stateMachine, animBoolName, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();

        holdPosition = player.transform.position;
        HoldPosition();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isExitingState) return;

        HoldPosition();

        if (yInput > 0)
        {
            stateMachine.ChangeState(player.WallClimbState);
        }
        if (yInput < 0 || !grabInput)
        {
            stateMachine.ChangeState(player.WallSlideState);
        }
    }

    private void HoldPosition()
    {
        player.transform.position = holdPosition; //Prevents gravity from causing player to slide
        player.SetVelocityX(0f);
        player.SetVelocityY(0f);
    }
}
