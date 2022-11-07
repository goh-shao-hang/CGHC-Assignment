using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected FiniteStateMachine stateMachine;
    protected Entity entity;

    protected string animBoolName;

    public float startTime { get; protected set; }
    
    public State(Entity entity, FiniteStateMachine stateMachine, string animBoolName) //constructor
    {
        this.entity = entity; //this. since both the variable and the parameter have the same name
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        startTime = Time.time;
        entity.anim.SetBool(animBoolName, true);
        DoChecks();
    }
    
    public virtual void Exit()
    {
        entity.anim.SetBool(animBoolName, false);
    }

    public virtual void LogicUpdate() //Called every frame
    {

    }

    public virtual void PhysicsUpdate() //Called in fixed update
    {
        DoChecks();
    }

    public virtual void DoChecks()
    {

    }
}
