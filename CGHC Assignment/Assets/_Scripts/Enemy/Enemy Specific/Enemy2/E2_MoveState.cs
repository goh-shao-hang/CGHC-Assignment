using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_MoveState : MoveState
{
    private Enemy2 enemy2;

    public E2_MoveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData) : base(entity, stateMachine, animBoolName, stateData)
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

        if (isPlayerInMinAggroRange)
        {
            stateMachine.ChangeState(enemy2.playerDetectedState);
        }
        else if (isDetectingWall || !isDetectingLedge)
        {
            enemy2.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(enemy2.idleState);
        }
            
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
