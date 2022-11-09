using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    private Animator anim;
    private PlayerController pc;
    private PlayerStats ps;

    [Header("Assignables")]
    [SerializeField] private Transform attack1HitboxPos;
    [SerializeField] private LayerMask whatIsDamageable;

    [SerializeField] private bool combatEnabled;
    [SerializeField] private float inputTimer, comboTimer, attack1Radius, attack1Damage, stunDamageAmount = 1f;

    private bool gotInput, isAttacking;
    private int combo;
    private float lastInputTime = Mathf.NegativeInfinity; //Always can attack when the game starts

    private AttackDetails attackDetails;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combatEnabled);
        pc = GetComponent<PlayerController>();
        ps = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        CheckCombatInput();
        CheckAttacks();
    }

    private void CheckCombatInput()
    {
        if (!combatEnabled) return;

        if (Input.GetMouseButtonDown(0))
        {
            gotInput = true;
            lastInputTime = Time.time;
        }
    }

    private void CheckAttacks()
    {
        if (gotInput)
        {
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                combo++;
                if (combo > 3)
                    combo = 1;
                anim.SetBool("attack1", true);
                anim.SetInteger("combo", combo);
                anim.SetBool("isAttacking", isAttacking);
            }
        }

        if (Time.time >= lastInputTime + inputTimer)
        {
            gotInput = false;
        }
        if (Time.time >= lastInputTime + comboTimer)
        {
            combo = 0;
            anim.SetInteger("combo", combo);
        }
    }

    private void CheckAttackHitbox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitboxPos.position, attack1Radius, whatIsDamageable);

        attackDetails.damageAmount = attack1Damage;
        attackDetails.position = transform.position;
        attackDetails.stunDamageAmount = stunDamageAmount;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("TakeDamage", attackDetails); //SendMessage finds a method in a script without knowing what the script is
        }
    }

    private void FinishAttack1()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack1", false);
    }

    public void TakeDamage(AttackDetails attackDetails)
    {
        if (pc.GetDashStatus()) return; //if player is dashing, don't damage them

        ps.DecreaseHealth(attackDetails.damageAmount);

        int direction;
        if (attackDetails.position.x < transform.position.x) //enemy on left, attack aimed right
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
            

        pc.Knockback(direction);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitboxPos.position, attack1Radius);
    }
}
