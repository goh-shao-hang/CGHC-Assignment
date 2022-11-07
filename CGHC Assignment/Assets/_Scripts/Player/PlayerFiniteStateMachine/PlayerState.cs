using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : State
{
    protected Player player;
    protected PlayerData playerData;

    protected bool isAnimationFinished;
    protected bool isExitingState;

    public PlayerState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, PlayerData playerData) : base(entity, stateMachine, animBoolName)
    {
        player = entity as Player;
        this.playerData = playerData;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log(animBoolName);
        isAnimationFinished = false;
        isExitingState = false;
    }

    public override void Exit()
    {
        base.Exit();

        isExitingState = true;
    }

    public virtual void AnimationTrigger() { }

    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;
}
