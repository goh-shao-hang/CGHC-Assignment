using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMagic : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D hitbox;
    private AttackDetails attackDetails;
    private bool damaged = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        hitbox = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        attackDetails.position = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !damaged)
        {
            damaged = true;
            other.GetComponent<IDamageable>()?.TakeDamage(attackDetails);
        }  
    }

    public void SetDamage(float damage)
    {
        attackDetails.damageAmount = damage;
    }

    private void ActivateHitbox() => hitbox.enabled = true;

    private void DisableHitbox() => hitbox.enabled = false;

    private void FinishMagicAttack()
    {
        damaged = false;
        Destroy(gameObject);
    }
}
