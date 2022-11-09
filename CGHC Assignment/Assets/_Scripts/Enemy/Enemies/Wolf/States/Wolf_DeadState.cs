using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf_DeadState : DeadState
{
    private Wolf wolf;

    public Wolf_DeadState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        wolf = entity as Wolf;
    }

    
}
