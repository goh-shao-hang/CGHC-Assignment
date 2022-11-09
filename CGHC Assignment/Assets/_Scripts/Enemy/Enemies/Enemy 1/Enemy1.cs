using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Enemy
{
    public E1_IdleState idleState { get; private set; }
    public E1_MoveState moveState { get; private set; }
    public E1_PlayerDetectedState playerDetectedState { get; private set; }
    public E1_ChargeState chargeState { get; private set; }
    public E1_LookForPlayerState lookForPlayerState { get; private set; }
    public E1_MeleeAttackState meleeAttackState { get; private set; }
    public E1_StunState stunState { get; private set; }
    public E1_DeadState deadState { get; private set; }

    [Header("Scriptable Objects")]
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_PlayerDetectedState playerDetectedStateData;
    [SerializeField] private D_ChargeState chargeStateData;
    [SerializeField] private D_LookForPlayerState lookForPlayerStateData;
    [SerializeField] private D_MeleeAttackState meleeAttackStateData;
    [SerializeField] private D_StunState stunStateData;
    [SerializeField] private D_DeadState deadStateData;

    [Header("Assignables")]
    [SerializeField] private Transform meleeAttackPosition;

    protected override void Start()
    {
        base.Start();

        idleState = new E1_IdleState(this, StateMachine, "idle", idleStateData);
        moveState = new E1_MoveState(this, StateMachine, "move", moveStateData);
        playerDetectedState = new E1_PlayerDetectedState(this, StateMachine, "playerDetected", playerDetectedStateData);
        chargeState = new E1_ChargeState(this, StateMachine, "charge", chargeStateData);
        lookForPlayerState = new E1_LookForPlayerState(this, StateMachine, "lookForPlayer", lookForPlayerStateData);
        meleeAttackState = new E1_MeleeAttackState(this, StateMachine, "meleeAttack", meleeAttackPosition , meleeAttackStateData);
        stunState = new E1_StunState(this, StateMachine, "stun", stunStateData);
        deadState = new E1_DeadState(this, StateMachine, "dead", deadStateData);

        StateMachine.Inititalize(moveState);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(meleeAttackPosition.position, meleeAttackStateData.attackRadius);
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
        
    }
}
