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

        if (enemy.PlayerInMinAggroRange)
        {
            stateMachine.ChangeState(boss.playerDetectedState);
        }
        else if (enemy.WallDetected || enemy.LedgeDetected)
        {
            boss.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(boss.idleState);
        }
    }
}
