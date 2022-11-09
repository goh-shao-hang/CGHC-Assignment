using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf_ChargeState : ChargeState
{
    private Wolf wolf;

    public Wolf_ChargeState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData) : base(entity, stateMachine, animBoolName, stateData)
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
        else if (enemy.LedgeDetected || enemy.WallDetected)
        {
            wolf.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(wolf.idleState);
        }
        else if (isChargeTimeOver)
        {
            if (enemy.PlayerInMaxAggroRange)
            {
                stateMachine.ChangeState(wolf.playerDetectedState);
            }
            else
            {
                wolf.idleState.SetFlipAfterIdle(false);
                stateMachine.ChangeState(wolf.idleState);
            }
        }
    }
}
