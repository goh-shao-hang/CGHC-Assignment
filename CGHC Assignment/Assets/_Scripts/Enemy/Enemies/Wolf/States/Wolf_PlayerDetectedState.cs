using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf_PlayerDetectedState : PlayerDetectedState
{
    private Wolf wolf;

    public Wolf_PlayerDetectedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetectedState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        wolf = entity as Wolf;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (performCloseRangeAction)
        {
            stateMachine.ChangeState(wolf.meleeAttackState);
        }
        else if (performLongRangeAction)
        {
            stateMachine.ChangeState(wolf.chargeState);
        }
        else if (!enemy.PlayerInMaxAggroRange)
        {
            wolf.idleState.SetFlipAfterIdle(false);
            stateMachine.ChangeState(wolf.idleState);
        }
        else if (enemy.LedgeDetected)
        {
            enemy.Flip();
            stateMachine.ChangeState(wolf.moveState);
        }
    }
}
