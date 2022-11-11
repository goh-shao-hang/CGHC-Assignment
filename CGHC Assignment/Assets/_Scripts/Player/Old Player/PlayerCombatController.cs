using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;

public class PlayerCombatController : MonoBehaviour, IDamageable
{
    private Animator anim;
    private SpriteRenderer sr;
    private PlayerController pc;
    private PlayerStats ps;
    private CinemachineImpulseSource impulseSource;
    private Volume volume;

    public AudioSource audioSource;

    [Header("Assignables")]
    [SerializeField] private Transform attack1HitboxPos;
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private GameObject playerHitParticles;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private Volume CAVolume;

    [SerializeField] private bool combatEnabled;
    [SerializeField] private float inputTimer, comboTimer, attack1Radius, attack1Damage, stunDamageAmount = 1f;

    private bool gotInput, isAttacking;
    private int combo;
    private float lastInputTime = Mathf.NegativeInfinity; //Always can attack when the game starts

    private AttackDetails attackDetails;
    private Material originalMat;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combatEnabled);
        pc = GetComponent<PlayerController>();
        ps = GetComponent<PlayerStats>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        sr = GetComponent<SpriteRenderer>();
        CAVolume.enabled = false;
        originalMat = sr.material;
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
                audioSource.Play();
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
            collider.GetComponent<IDamageable>()?.TakeDamage(attackDetails);
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

        StartCoroutine(HitEffect());

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

    IEnumerator HitEffect()
    {
        Time.timeScale = 0f;
        sr.material = flashMaterial;
        impulseSource.GenerateImpulse(5f);
        CAVolume.enabled = true;
        yield return new WaitForSecondsRealtime(.3f);
        Instantiate(playerHitParticles, transform.position, Quaternion.identity);
        Time.timeScale = 1f;
        sr.material = originalMat;
        CAVolume.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitboxPos.position, attack1Radius);
    }
}
