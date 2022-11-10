using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_PlayerDetectedState : PlayerDetectedState
{
    private Boss boss;

    public Boss_PlayerDetectedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetectedState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        boss = entity as Boss;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (performCloseRangeAction)
        {
            stateMachine.ChangeState(boss.meleeAttackState);
        }
        else if (performLongRangeAction)
        {
            if (Time.time >= boss.fadeState.startTime + boss.FadeStateData.fadeCooldown)
            {
                stateMachine.ChangeState(boss.fadeState);
            }
            else
            {
                stateMachine.ChangeState(boss.magicAttackState);
            }
        }
        else if (!enemy.PlayerInMaxAggroRange)
        {
            boss.idleState.SetFlipAfterIdle(false);
            stateMachine.ChangeState(boss.idleState);
        }
        else if (enemy.LedgeDetected)
        {
            enemy.Flip();
            stateMachine.ChangeState(boss.moveState);
        }
    }
}
