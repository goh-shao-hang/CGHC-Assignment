using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_StunState : StunState
{
    private Boss boss;

    public Boss_StunState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        boss = entity as Boss;
    }

    
}
