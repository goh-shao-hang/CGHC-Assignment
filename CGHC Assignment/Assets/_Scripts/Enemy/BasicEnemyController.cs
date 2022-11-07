using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    private enum State
    {
        Moving,
        Knockback,
        Dead
    }

    private State currentState;

    private GameObject baseGO;
    private Rigidbody2D baseRB;
    private Animator baseAnim;

    [Header("Assignables")]
    [SerializeField] private GameObject hitParticle;
    [SerializeField] private GameObject deathChunkParticle;
    [SerializeField] private GameObject deathBloodParticle;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform touchDamageCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsPlayer;

    [Header("Settings")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float speed;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private Vector2 knockbackSpeed;
    [SerializeField] private float touchDamage, touchDamageCooldown, touchDamageWidth, touchDamageHeight; //Touch damage
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;

    private float currentHealth;
    private float knockbackStartTime;
    private bool groundDetected, wallDetected;
    private int facingDirection = 1;
    private int damageDirection;
    private Vector2 movement;
    private float lastTouchDamageTime;
    private Vector2 touchDamageBotLeft, touchDamageTopRight;
    private float[] attackDetails = new float[2];

    private void Start()
    {
        baseGO = transform.Find("Base").gameObject;
        baseRB = baseGO.GetComponent<Rigidbody2D>();
        baseAnim = baseGO.GetComponent<Animator>();

        currentHealth = maxHealth;
        facingDirection = 1;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    private void EnterMovingState()
    {
        
    }

    private void UpdateMovingState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround); //transform.right instead of Vector2.right so that local rotation is considered

        CheckTouchDamage();

        if (!groundDetected || wallDetected)
        {
            Flip();
        }
        else
        {
            movement.Set(speed * facingDirection, baseRB.velocity.y);
            baseRB.velocity = movement;
        }
    }

    private void ExitMovingState()
    {

    }

    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        baseRB.velocity = movement;
        baseAnim.SetBool("knockback", true);
    }

    private void UpdateKnockbackState()
    {
        if (Time.time > knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Moving);
        }
    }

    private void ExitKnockbackState()
    {
        baseAnim.SetBool("knockback", false);
    }

    private void EnterDeadState()
    {
        Instantiate(deathChunkParticle, baseGO.transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodParticle, baseGO.transform.position, deathBloodParticle.transform.rotation);
        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }


    //OTHER FUNCTIONS
    private void SwitchState(State nextState)
    {
        switch (currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (nextState)
        {
            case State.Moving:
                EnterMovingState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = nextState;
    }
    
    private void TakeDamage(float[] attackDetails) //Since SendMessage only takes 1 parameter, float array is used to passed in multiple parameters
    {
        currentHealth -= attackDetails[0]; //damage
        Instantiate(hitParticle, baseGO.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

        if (attackDetails[1] > baseGO.transform.position.x) //direction
        {
            damageDirection = -1; //Player on right, thus attack is aimed left
        }
        else
        {
            damageDirection = 1; //Player on left, thus attack is aimed right
        }

        if (currentHealth > 0f)
        {
            SwitchState(State.Knockback);
        }
        else
        {
            SwitchState(State.Dead);
        }
    }

    private void CheckTouchDamage()
    {
        if (Time.time >= lastTouchDamageTime + touchDamageCooldown)
        {
            touchDamageBotLeft.Set(touchDamageCheck.position.x - touchDamageWidth * 0.5f, touchDamageCheck.position.y - touchDamageHeight * 0.5f);
            touchDamageTopRight.Set(touchDamageCheck.position.x + touchDamageWidth * 0.5f, touchDamageCheck.position.y + touchDamageHeight * 0.5f);

            Collider2D hit = Physics2D.OverlapArea(touchDamageBotLeft, touchDamageTopRight, whatIsPlayer);

            if (hit != null)
            {
                lastTouchDamageTime = Time.time;
                attackDetails[0] = touchDamage;
                attackDetails[1] = baseGO.transform.position.x;
                hit.SendMessage("TakeDamage", attackDetails);
            }
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        baseGO.transform.Rotate(0f, 180f, 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance * facingDirection, wallCheck.position.y));

        //Hitbox
        Vector2 botLeft = new Vector2(touchDamageCheck.position.x - touchDamageWidth * 0.5f, touchDamageCheck.position.y - touchDamageHeight * 0.5f);
        Vector2 botRight = new Vector2(touchDamageCheck.position.x + touchDamageWidth * 0.5f, touchDamageCheck.position.y - touchDamageHeight * 0.5f);
        Vector2 topLeft = new Vector2(touchDamageCheck.position.x - touchDamageWidth * 0.5f, touchDamageCheck.position.y + touchDamageHeight * 0.5f);
        Vector2 topRight = new Vector2(touchDamageCheck.position.x + touchDamageWidth * 0.5f, touchDamageCheck.position.y + touchDamageHeight * 0.5f);
        Gizmos.DrawLine(botLeft, botRight);
        Gizmos.DrawLine(botRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, botLeft);
    }
}
