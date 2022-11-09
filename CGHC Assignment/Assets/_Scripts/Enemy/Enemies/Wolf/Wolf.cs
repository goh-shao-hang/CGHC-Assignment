using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Enemy
{
    public Wolf_IdleState idleState { get; private set; }
    public Wolf_MoveState moveState { get; private set; }
    public Wolf_PlayerDetectedState playerDetectedState { get; private set; }
    public Wolf_ChargeState chargeState { get; private set; }
    public Wolf_MeleeAttackState meleeAttackState { get; private set; }
    public Wolf_StunState stunState { get; private set; }
    public Wolf_DeadState deadState { get; private set; }

    [Header("State Data")]
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_PlayerDetectedState playerDetectedStateData;
    [SerializeField] private D_ChargeState chargeStateData;
    [SerializeField] private D_MeleeAttackState meleeAttackStateData;
    [SerializeField] private D_StunState stunStateData;
    [SerializeField] private D_DeadState deadStateData;

    [Header("Assignables")]
    [SerializeField] private Transform meleeAttackPosition;

    protected override void Start()
    {
        base.Start();

        idleState = new Wolf_IdleState(this, StateMachine, "idle", idleStateData);
        moveState = new Wolf_MoveState(this, StateMachine, "move", moveStateData);
        playerDetectedState = new Wolf_PlayerDetectedState(this, StateMachine, "playerDetected", playerDetectedStateData);
        chargeState = new Wolf_ChargeState(this, StateMachine, "move", chargeStateData);
        meleeAttackState = new Wolf_MeleeAttackState(this, StateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData);
        stunState = new Wolf_StunState(this, StateMachine, "stun", stunStateData);
        deadState = new Wolf_DeadState(this, StateMachine, "dead", deadStateData);

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
    }

    protected virtual void FinishMeleeAttack()
    {
        meleeAttackState.FinishAttack();
    }
}
