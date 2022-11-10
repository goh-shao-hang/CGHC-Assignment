using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] public GameObject deathChunkParticle;
    [SerializeField] public GameObject deathBloodParticle;

    [Header("Settings")]
    [SerializeField] public float maxHealth;

    public float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
            SceneManager.LoadScene(2);
        }
    }

    private void Die()
    {
        Instantiate(deathChunkParticle, transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodParticle, transform.position, deathBloodParticle.transform.rotation);
        GameManager.Instance?.Respawn();
        Destroy(gameObject); 
    }
}
