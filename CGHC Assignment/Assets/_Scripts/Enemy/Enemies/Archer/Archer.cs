using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Enemy
{
    public Archer_MoveState moveState { get; private set; }
    public Archer_IdleState idleState { get; private set; }
    public Archer_PlayerDetectedState playerDetectedState { get; private set; }
    public Archer_MeleeAttackState meleeAttackState { get; private set; }
    public Archer_LookForPlayerState lookForPlayerState { get; private set; }
    public Archer_StunState stunState { get; private set; }
    public Archer_DeadState deadState { get; private set; }
    public Archer_DodgeState dodgeState { get; private set; }
    public Archer_RangedAttackState rangedAttackState { get; private set; }

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

        moveState = new Archer_MoveState(this, StateMachine, "move", moveStateData);
        idleState = new Archer_IdleState(this, StateMachine, "idle", idleStateData);
        playerDetectedState = new Archer_PlayerDetectedState(this, StateMachine, "playerDetected", playerDetectedStateData);
        meleeAttackState = new Archer_MeleeAttackState(this, StateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData);
        lookForPlayerState = new Archer_LookForPlayerState(this, StateMachine, "lookForPlayer", lookForPlayerStateData);
        stunState = new Archer_StunState(this, StateMachine, "stun", stunStateData);
        deadState = new Archer_DeadState(this, StateMachine, "dead", deadStateData);
        dodgeState = new Archer_DodgeState(this, StateMachine, "dodge", dodgeStateData);
        rangedAttackState = new Archer_RangedAttackState(this, StateMachine, "rangedAttack", rangedAttackPosition, rangedAttackStateData);

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
        else if (PlayerInMinAggroRange)
        {
            StateMachine.ChangeState(rangedAttackState);
        }
        else if (!PlayerInMinAggroRange&& !isStunned) //If player hits from behind, go to look for player state
        {
                idleState.SetFlipAfterIdle(false); //if this is left true, the enemy will flip twice as a result
                lookForPlayerState.SetTurnImmediately(true);
                StateMachine.ChangeState(lookForPlayerState);
        }
    }

    public void TriggerMeleeAttack()
    {
        meleeAttackState.TriggerAttack();
    }

    public void FinishMeleeAttack()
    {
        meleeAttackState.FinishAttack();
    }

    public void TriggerRangedAttack()
    {
        rangedAttackState.TriggerAttack();
    }

    public void FinishRangedAttack()
    {
        rangedAttackState.FinishAttack();
    }


    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(meleeAttackPosition.position, meleeAttackStateData.attackRadius);
    }
}
