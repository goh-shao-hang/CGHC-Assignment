using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_MoveState : MoveState
{
    private Archer enemy2;

    public Archer_MoveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        enemy2 = entity as Archer;
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

        if (enemy.PlayerInMinAggroRange)
        {
            stateMachine.ChangeState(enemy2.playerDetectedState);
        }
        else if (enemy.WallDetected || enemy.LedgeDetected)
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
