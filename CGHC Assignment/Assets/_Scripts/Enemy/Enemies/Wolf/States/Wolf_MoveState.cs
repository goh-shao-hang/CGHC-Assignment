using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf_MoveState : MoveState
{
    private Wolf wolf;

    public Wolf_MoveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        wolf = entity as Wolf;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (enemy.PlayerInMinAggroRange && !enemy.LedgeDetected)
        {
            stateMachine.ChangeState(wolf.playerDetectedState);
        }
        else if (enemy.WallDetected || enemy.LedgeDetected)
        {
            wolf.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(wolf.idleState);
        }
    }
}
