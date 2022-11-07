using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDummyController : MonoBehaviour
{
    private PlayerController pc;
    private GameObject baseGO, brokenTopGO, brokenBotGO;
    private Rigidbody2D rbBase, rbBrokenTop, rbBrokenBot;
    private Animator animBase;

    [Header("Assignables")]
    [SerializeField] private GameObject hitParticles;

    [Header("Settings")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float knockbackSpeedX, knockbackSpeedY, knockbackDuration, knockbackDeathSpeedX, knockbackDeathSpeedY, deathTorque;
    [SerializeField] private bool applyKnockback;

    private float currentHealth, knockbackStart;
    private int playerFacingDirection;
    private bool playerOnLeft;
    private bool knockback;

    private void Start()
    {
        currentHealth = maxHealth;

        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        baseGO = transform.Find("Base").gameObject; //transform.Find finds a child object with the specifc name on a paarent object
        brokenTopGO = transform.Find("BrokenTop").gameObject;
        brokenBotGO = transform.Find("BrokenBottom").gameObject;
        animBase = baseGO.GetComponent<Animator>();
        rbBase = baseGO.GetComponent<Rigidbody2D>();
        rbBrokenTop = brokenTopGO.GetComponent<Rigidbody2D>();
        rbBrokenBot = brokenBotGO.GetComponent<Rigidbody2D>();
        baseGO.SetActive(true);
        brokenTopGO.SetActive(false);
        brokenBotGO.SetActive(false);
    }

    private void Update()
    {
        CheckKnockback();
    }

    private void TakeDamage(AttackDetails attackDetails)
    {
        currentHealth -= attackDetails.damageAmount;

        if (attackDetails.position.x < baseGO.transform.position.x) //Player on left
            playerFacingDirection = 1;
        else
            playerFacingDirection = -1;

        Instantiate(hitParticles, baseGO.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

        if (playerFacingDirection == 1)
            playerOnLeft = true;
        else
            playerOnLeft = false;

        animBase.SetBool("playerOnLeft", playerOnLeft);
        animBase.SetTrigger("damage");

        if (applyKnockback && currentHealth > 0f)
        {
            Knockback();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Knockback()
    {
        knockback = true;
        knockbackStart = Time.time;
        rbBase.velocity = new Vector2(knockbackSpeedX * playerFacingDirection, knockbackSpeedY);
    }

    private void CheckKnockback()
    {
        if (Time.time >= knockbackStart + knockbackDuration && knockback)
        {
            knockback = false;
            rbBase.velocity = new Vector2(0f, rbBase.velocity.y);
        }
    }

    private void Die()
    {
        baseGO.SetActive(false);
        brokenTopGO.SetActive(true);
        brokenBotGO.SetActive(true);

        brokenTopGO.transform.position = baseGO.transform.position;
        brokenBotGO.transform.position = baseGO.transform.position;

        rbBrokenBot.velocity = new Vector2(knockbackSpeedX * playerFacingDirection, knockbackSpeedY);
        rbBrokenTop.velocity = new Vector2(knockbackDeathSpeedX * playerFacingDirection, knockbackDeathSpeedY);
        rbBrokenTop.AddTorque(deathTorque * - playerFacingDirection, ForceMode2D.Impulse);
    }
}
