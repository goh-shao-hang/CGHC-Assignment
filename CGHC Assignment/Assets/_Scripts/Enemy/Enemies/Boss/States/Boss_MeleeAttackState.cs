using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_MeleeAttackState : MeleeAttackState
{
    private Boss boss;

    public Boss_MeleeAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttackState stateData) : base(entity, stateMachine, animBoolName, attackPosition, stateData)
    {
        boss = entity as Boss;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            if (enemy.PlayerInMaxAggroRange)
            {
                stateMachine.ChangeState(boss.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(boss.idleState);
            }
        }
    }
}
