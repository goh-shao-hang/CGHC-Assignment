using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectedState : EnemyState
{
    protected D_PlayerDetectedState stateData;

    public Transform player;

    protected bool performCloseRangeAction;
    protected bool performLongRangeAction;
 
    public PlayerDetectedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetectedState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        base.Enter();

        player = GameObject.Find("Player").transform;
        performLongRangeAction = false;
        enemy.SetVelocityX(0f);
    }

    public override void Exit()
    {
        base.Exit();

        player = null;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.transform.position.x > player.position.x && enemy.FacingDirection != -1)
        {
            enemy.Flip();
        }
        else if (enemy.transform.position.x < player.position.x && enemy.FacingDirection != 1)
        {
            enemy.Flip();
        }

        performCloseRangeAction = Time.time >= startTime + stateData.closeRangeActionTime && enemy.PlayerInCloseRangeAction;
        performLongRangeAction = Time.time >= startTime + stateData.longRangeActionTime;

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

}
