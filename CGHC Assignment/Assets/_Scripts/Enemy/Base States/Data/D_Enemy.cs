using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyData", menuName = "Data/Entity Data/Enemy Base Data")] //fileName is the name that the created asset is automatically named after, menuName is the name of the path to find and create the data
public class D_Enemy : ScriptableObject
{
    public float maxHealth = 30f;

    public float damageHopSpeed = 10f;

    public float wallCheckDistance = 0.2f;
    public float ledgeCheckDistance = 0.4f;
    public float groundCheckRadius = 0.3f;

    public float minAggroDistance = 3f;
    public float maxAggroDistance = 4f;
    public float closeRangeActionDistance = 1f;

    public float stunResistance = 3f; //amount of stun damage the enemy needs to take before getting stunned
    public float stunRecoveryTime = 2f; //The time needed for stun damage to reset. If the player hasn't deal damage to the enemy for this amount of time, the stun damage needed resets.

    public GameObject hitParticles;

    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
}
