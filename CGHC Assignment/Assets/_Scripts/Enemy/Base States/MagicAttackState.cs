using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttackState : AttackState
{
    protected D_MagicAttackState stateData;
    protected GameObject magicAttack;
    protected BossMagic bossMagic;
    protected Transform player;

    public MagicAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MagicAttackState stateData) : base(entity, stateMachine, animBoolName, attackPosition)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        base.Enter();

        player = GameObject.Find("Player").transform;
    }

    public override void Exit()
    {
        base.Exit();

        player = null;
    }
    public override void TriggerAttack()
    {
        base.TriggerAttack();

        magicAttack = GameObject.Instantiate(stateData.magicAttack, player.position, Quaternion.identity);
        bossMagic = magicAttack.GetComponent<BossMagic>();
        //BossMagic.FireProjectile(stateData.projectileSpeed, stateData.projectileTravelDistance, stateData.projectileDamage);
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
    }

}
