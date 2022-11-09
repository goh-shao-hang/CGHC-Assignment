using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_IdleState : IdleState
{
    private Archer enemy2;

    public Archer_IdleState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData) : base(entity, stateMachine, animBoolName, stateData)
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
        else if (isIdleTimeOver)
        {
            stateMachine.ChangeState(enemy2.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
