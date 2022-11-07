using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerTouchingWallState
{
    public PlayerWallSlideState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, PlayerData playerData) : base(entity, stateMachine, animBoolName, playerData)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isExitingState) return; //We don't want to set velocity or change state again if we triggered a wall jump in the superstate

        player.SetVelocityY(-playerData.wallSlideVelocity);
        
        if (grabInput && yInput == 0)
        {
            stateMachine.ChangeState(player.WallGrabState);
        }
    }
}
