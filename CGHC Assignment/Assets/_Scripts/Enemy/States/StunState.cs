using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : EnemyState
{
    protected D_StunState stateData;

    protected bool isStunTimeOver;
    protected bool isGrounded;
    protected bool isMovementStopped;
    protected bool performCloseRangeAction;
    protected bool isPlayerInMinAggroRange;

    public StunState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = enemy.CheckGround();
        performCloseRangeAction = enemy.CheckPlayerInCloseRangeAction();
        isPlayerInMinAggroRange = enemy.CheckPlayerInMinAggroRange();
    }

    public override void Enter()
    {
        base.Enter();

        isStunTimeOver = false;
        isMovementStopped = false;
        enemy.SetVelocity(stateData.stunKnockbackSpeed, stateData.stunKnockbackAngle, enemy.lastDamageDirection);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.ResetStunResistance();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= startTime + stateData.stunTime)
            isStunTimeOver = true;

        if (isGrounded && Time.time >= startTime + stateData.stunKnockbackTime && !isMovementStopped) //Make sure the velocity can only be set to 0 if the enemy hits the ground after being knocked back after a certain amount of time
        {
            isMovementStopped = true; //only get into this if statement once, so that the enemy is not continuously stopped and we can make a bounce effect or other stuff otherwards
            enemy.SetVelocityX(0f);
        }
    }
            

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
