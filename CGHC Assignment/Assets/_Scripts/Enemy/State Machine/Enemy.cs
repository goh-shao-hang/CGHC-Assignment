using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [Header("Base Data")]
    public D_Enemy enemyData;

    public GameObject baseGO { get; private set; }
    public AnimationToStateMachine atsm { get; private set; }

    [Header("Assignables")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private Transform groundCheck;
    
    public int lastDamageDirection { get; private set; }

    private float currentHealth;
    private float currentStunResistance;
    private float lastDamageTime;
    

    protected bool isStunned;
    protected bool isDead;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        baseGO = transform.Find("Base").gameObject;
        rb = baseGO.GetComponent<Rigidbody2D>();
        anim = baseGO.GetComponent<Animator>();
        atsm = baseGO.GetComponent<AnimationToStateMachine>();

        currentHealth = enemyData.maxHealth;
        currentStunResistance = enemyData.stunResistance;
        FacingDirection = 1;
    }

    protected override void Update()
    {
        base.Update();

        if (Time.time >= lastDamageTime + enemyData.stunRecoveryTime)
        {
            ResetStunResistance();

            anim.SetFloat("yVelocity", rb.velocity.y);
        }
    }

    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, baseGO.transform.right, enemyData.wallCheckDistance, enemyData.whatIsGround);
    }

    public virtual bool CheckLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, enemyData.ledgeCheckDistance, enemyData.whatIsGround);
    }

    public virtual bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, enemyData.groundCheckRadius, enemyData.whatIsGround);
    }

    public virtual bool CheckPlayerInMinAggroRange()
    {
        return Physics2D.Raycast(playerCheck.position, baseGO.transform.right, enemyData.minAggroDistance, enemyData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInMaxAggroRange()
    {
        return Physics2D.Raycast(playerCheck.position, baseGO.transform.right, enemyData.maxAggroDistance, enemyData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return Physics2D.Raycast(playerCheck.position, baseGO.transform.right, enemyData.closeRangeActionDistance, enemyData.whatIsPlayer);
    }

    public virtual void TakeDamage(AttackDetails attackDetails)
    {
        lastDamageTime = Time.time;
        currentHealth -= attackDetails.damageAmount;
        currentStunResistance -= attackDetails.stunDamageAmount;

        //Effects
        DamageHop(enemyData.damageHopSpeed);
        Instantiate(enemyData.hitParticles, baseGO.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

        if (currentHealth <= 0)
        {
            isDead = true;
            return;
        }

        if (attackDetails.position.x > baseGO.transform.position.x) //attack came from right, so knockback towards left
            lastDamageDirection = -1;
        else
            lastDamageDirection = 1;

        if (currentStunResistance <= 0)
            isStunned = true;
    }
    public virtual void ResetStunResistance()
    {
        isStunned = false;
        currentStunResistance = enemyData.stunResistance;
    }

    public virtual void DamageHop(float yVelocity)
    {
        rb.velocity = new Vector2(rb.velocity.x, yVelocity);
    }

    public override void Flip()
    {
        base.Flip();
        baseGO.transform.Rotate(0f, 180f, 0f);
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * FacingDirection * enemyData.wallCheckDistance));
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * enemyData.ledgeCheckDistance));
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * FacingDirection * enemyData.closeRangeActionDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * FacingDirection * enemyData.minAggroDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * FacingDirection * enemyData.maxAggroDistance), 0.2f);
    }
}
