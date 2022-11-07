using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : Enemy
{
    public E2_MoveState moveState { get; private set; }
    public E2_IdleState idleState { get; private set; }
    public E2_PlayerDetectedState playerDetectedState { get; private set; }
    public E2_MeleeAttackState meleeAttackState { get; private set; }
    public E2_LookForPlayerState lookForPlayerState { get; private set; }
    public E2_StunState stunState { get; private set; }
    public E2_DeadState deadState { get; private set; }
    public E2_DodgeState dodgeState { get; private set; }
    public E2_RangedAttackState rangedAttackState { get; private set; }

    [Header("Scriptable Objects")]
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_PlayerDetectedState playerDetectedStateData;
    [SerializeField] private D_MeleeAttackState meleeAttackStateData;
    [SerializeField] private D_LookForPlayerState lookForPlayerStateData;
    [SerializeField] private D_StunState stunStateData;
    [SerializeField] private D_DeadState deadStateData;
    [SerializeField] private D_DodgeState dodgeStateData;
    [SerializeField] private D_RangedAttackState rangedAttackStateData;

    public D_DodgeState DodgeStateData => dodgeStateData;//To access dodge cd in other state which is readonly

    [Header("Assignables")]
    [SerializeField] private Transform meleeAttackPosition;
    [SerializeField] private Transform rangedAttackPosition;

    protected override void Start()
    {
        base.Start();

        moveState = new E2_MoveState(this, StateMachine, "move", moveStateData);
        idleState = new E2_IdleState(this, StateMachine, "idle", idleStateData);
        playerDetectedState = new E2_PlayerDetectedState(this, StateMachine, "playerDetected", playerDetectedStateData);
        meleeAttackState = new E2_MeleeAttackState(this, StateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData);
        lookForPlayerState = new E2_LookForPlayerState(this, StateMachine, "lookForPlayer", lookForPlayerStateData);
        stunState = new E2_StunState(this, StateMachine, "stun", stunStateData);
        deadState = new E2_DeadState(this, StateMachine, "dead", deadStateData);
        dodgeState = new E2_DodgeState(this, StateMachine, "dodge", dodgeStateData);
        rangedAttackState = new E2_RangedAttackState(this, StateMachine, "rangedAttack", rangedAttackPosition, rangedAttackStateData);

        StateMachine.Inititalize(moveState);
    }

    public override void TakeDamage(AttackDetails attackDetails)
    {
        base.TakeDamage(attackDetails);

        if (isDead)
        {
            StateMachine.ChangeState(deadState);
        }
        else if (isStunned && StateMachine.CurrentState != stunState)
        {
            StateMachine.ChangeState(stunState);
        }
        else if (CheckPlayerInMinAggroRange())
        {
            StateMachine.ChangeState(rangedAttackState);
        }
        else if (!CheckPlayerInMinAggroRange() && !isStunned) //If player hits from behind, go to look for player state
        {
                idleState.SetFlipAfterIdle(false); //if this is left true, the enemy will flip twice as a result
                lookForPlayerState.SetTurnImmediately(true);
                StateMachine.ChangeState(lookForPlayerState);
        }
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(meleeAttackPosition.position, meleeAttackStateData.attackRadius);
    }
}
