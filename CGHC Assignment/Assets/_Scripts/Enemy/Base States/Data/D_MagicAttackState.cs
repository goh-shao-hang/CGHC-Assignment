using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMagicAttackStateData", menuName = "Data/State Data/Magic Attack State")]
public class D_MagicAttackState : ScriptableObject
{
    public GameObject magicAttack;

    public float magicDamage;
    public Vector2 spawnOffset;
}
