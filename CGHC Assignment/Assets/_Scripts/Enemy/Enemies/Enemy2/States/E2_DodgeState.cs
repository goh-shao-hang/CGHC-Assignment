using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_DodgeState : DodgeState
{
    private Enemy2 enemy2;

    public E2_DodgeState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_DodgeState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        enemy2 = entity as Enemy2;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isDodgeOver)
        {
            if (performCloseRangeAction)
            {
                stateMachine.ChangeState(enemy2.meleeAttackState);
            }
            else if (isPlayerInMaxAggroRange && !performCloseRangeAction)
            {
                stateMachine.ChangeState(enemy2.rangedAttackState);
            }
            else if (isPlayerInMaxAggroRange)
            {
                stateMachine.ChangeState(enemy2.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(enemy2.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
