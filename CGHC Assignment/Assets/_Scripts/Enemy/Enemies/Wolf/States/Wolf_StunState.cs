using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf_StunState : StunState
{
    private Wolf wolf;

    public Wolf_StunState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        wolf = entity as Wolf;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isStunTimeOver)
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
