using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchMoveState : PlayerGroundedState
{
    public PlayerCrouchMoveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, PlayerData playerData) : base(entity, stateMachine, animBoolName, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetColliderHeight(playerData.crouchColliderHeight);
    }

    public override void Exit()
    {
        base.Exit();

        player.SetColliderHeight(playerData.standColliderHeight);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isExitingState) return;

        player.SetVelocityX(playerData.crouchMoveSpeed * player.FacingDirection);
        player.CheckIfShouldFlip(xInput);

        if (xInput == 0)
        {
            stateMachine.ChangeState(player.CrouchIdleState);
        }
        else if (yInput != -1 && !isTouchingCeiling)
        {
            stateMachine.ChangeState(player.MoveState);
        }
    }

}
