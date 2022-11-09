using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf_IdleState : IdleState
{
    private Wolf wolf;

    public Wolf_IdleState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        wolf = entity as Wolf;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.PlayerInMinAggroRange)
        {
            stateMachine.ChangeState(wolf.playerDetectedState);
        }
        else if (isIdleTimeOver)
        {
            stateMachine.ChangeState(wolf.moveState);
        }
    }
}
