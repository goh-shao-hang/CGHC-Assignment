using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_MoveState : MoveState
{
    private Boss boss;

    public Boss_MoveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        boss = enemy as Boss;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isPlayerInMinAggroRange)
        {
            stateMachine.ChangeState(boss.playerDetectedState);
        }
        else if (isDetectingWall || !isDetectingLedge)
        {
            boss.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(boss.idleState);
        }
    }
}
