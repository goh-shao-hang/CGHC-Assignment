using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_StunState : StunState
{
    private Archer archer;

    public Archer_StunState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        archer = entity as Archer;
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

        if (isStunTimeOver)
        {
            if (enemy.PlayerInMinAggroRange)
            {
                stateMachine.ChangeState(archer.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(archer.idleState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
