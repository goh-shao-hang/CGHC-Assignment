using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public Boss_IdleState idleState { get; private set; }
    public Boss_MoveState moveState { get; private set; }
    public Boss_PlayerDetectedState playerDetectedState { get; private set; }
    public Boss_MeleeAttackState meleeAttackState { get; private set; }
    public Boss_StunState stunState { get; private set; }
    public Boss_DeadState deadState { get; private set; }

    [Header("Scriptable Objects")]
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_PlayerDetectedState playerDetectedStateData;
    [SerializeField] private D_MeleeAttackState meleeAttackStateData;
    [SerializeField] private D_StunState stunStateData;
    [SerializeField] private D_DeadState deadStateData;

    [Header("Assignables")]
    [SerializeField] private Transform meleeAttackPosition;

    protected override void Start()
    {
        base.Start();

        idleState = new Boss_IdleState(this, StateMachine, "idle", idleStateData);
        moveState = new Boss_MoveState(this, StateMachine, "move", moveStateData);
        playerDetectedState = new Boss_PlayerDetectedState(this, StateMachine, "idle", playerDetectedStateData);
        meleeAttackState = new Boss_MeleeAttackState(this, StateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData);
        stunState = new Boss_StunState(this, StateMachine, "stun", stunStateData);
        deadState = new Boss_DeadState(this, StateMachine, "dead", deadStateData);

        StateMachine.Inititalize(moveState);
    }

    public void TriggerMeleeAttack() => meleeAttackState.TriggerAttack();

    public void FinishMeleeAttack() => meleeAttackState.FinishAttack();

    public void TriggerMagicAttack() { }
}
