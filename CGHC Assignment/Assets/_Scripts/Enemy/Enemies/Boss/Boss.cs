using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : Enemy
{
    public Boss_IdleState idleState { get; private set; }
    public Boss_MoveState moveState { get; private set; }
    public Boss_PlayerDetectedState playerDetectedState { get; private set; }
    public Boss_MeleeAttackState meleeAttackState { get; private set; }
    public Boss_MagicAttackState magicAttackState { get; private set; }
    public Boss_FadeState fadeState { get; private set; }
    public Boss_StunState stunState { get; private set; }
    public Boss_DeadState deadState { get; private set; }

    [Header("Scriptable Objects")]
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_PlayerDetectedState playerDetectedStateData;
    [SerializeField] private D_MeleeAttackState meleeAttackStateData;
    [SerializeField] private D_MagicAttackState magicAttackStateData;
    [SerializeField] private D_FadeState fadeStateData;
    [SerializeField] private D_StunState stunStateData;
    [SerializeField] private D_DeadState deadStateData;

    [Header("Assignables")]
    [SerializeField] private Transform meleeAttackPosition;

    public D_FadeState FadeStateData => fadeStateData;

    protected override void Start()
    {
        base.Start();

        idleState = new Boss_IdleState(this, StateMachine, "idle", idleStateData);
        moveState = new Boss_MoveState(this, StateMachine, "move", moveStateData);
        playerDetectedState = new Boss_PlayerDetectedState(this, StateMachine, "idle", playerDetectedStateData);
        meleeAttackState = new Boss_MeleeAttackState(this, StateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData);
        magicAttackState = new Boss_MagicAttackState(this, StateMachine, "magicAttack", null, magicAttackStateData);
        fadeState = new Boss_FadeState(this, StateMachine, "fade", fadeStateData);
        stunState = new Boss_StunState(this, StateMachine, "stun", stunStateData);
        deadState = new Boss_DeadState(this, StateMachine, "dead", deadStateData);

        StateMachine.Inititalize(moveState);
    }

    public override void TakeDamage(AttackDetails attackDetails)
    {
        base.TakeDamage(attackDetails);

        if (StateMachine.CurrentState == fadeState) return;

        if (StateMachine.CurrentState == idleState || StateMachine.CurrentState == moveState)
            StateMachine.ChangeState(playerDetectedState);

        if (isDead)
        {
            StateMachine.ChangeState(deadState);
            if (gameObject.name == "Boss")
                Invoke(nameof(BossDeath), 5f);
        }
        else if (isStunned && StateMachine.CurrentState != stunState)
        {
            StateMachine.ChangeState(stunState);
        }
    }
    public void BossDeath()
    {
        SceneManager.LoadScene(4);
    }

    public void TriggerMeleeAttack() => meleeAttackState.TriggerAttack();

    public void FinishMeleeAttack() => meleeAttackState.FinishAttack();

    public void TriggerMagicAttack() => magicAttackState.TriggerAttack();

    public void FinishMagicAttack() => magicAttackState.FinishAttack();

    public void TriggerFade() => fadeState.StartFade();

    public void FinishFade() => fadeState.FinishFade();

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleeAttackPosition.position, meleeAttackStateData.attackRadius);
    }


}
