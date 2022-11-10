using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_FadeState : FadeState
{
    private Boss boss;

    public Boss_FadeState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_FadeState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        boss = entity as Boss;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (fadeFinished)
        {
            stateMachine.ChangeState(boss.meleeAttackState);
        }
    }
}
