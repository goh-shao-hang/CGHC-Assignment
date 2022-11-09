using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_MagicAttackState : MagicAttackState
{
    private Boss boss;

    public Boss_MagicAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MagicAttackState stateData) : base(entity, stateMachine, animBoolName, attackPosition, stateData)
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
