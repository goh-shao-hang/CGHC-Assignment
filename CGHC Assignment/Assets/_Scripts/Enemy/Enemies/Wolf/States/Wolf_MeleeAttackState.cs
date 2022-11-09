using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf_MeleeAttackState : MeleeAttackState
{
    private Wolf wolf;

    public Wolf_MeleeAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttackState stateData) : base(entity, stateMachine, animBoolName, attackPosition, stateData)
    {
        wolf = entity as Wolf;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.SetVelocity(5f, new Vector2(2, 1), enemy.FacingDirection);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            if (enemy.PlayerInMaxAggroRange)
            {
                stateMachine.ChangeState(wolf.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(wolf.idleState);
            }
        }
    }
}
