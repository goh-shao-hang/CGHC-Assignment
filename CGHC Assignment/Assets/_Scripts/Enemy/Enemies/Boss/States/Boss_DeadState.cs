using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_DeadState : DeadState
{
    private Boss boss;

    public Boss_DeadState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        boss = entity as Boss;
    }
}
