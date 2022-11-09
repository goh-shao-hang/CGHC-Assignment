using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerDetectedStateData", menuName = "Data/State Data/Player Detected State")]
public class D_PlayerDetectedState : ScriptableObject
{
    public float closeRangeActionTime = 1.5f; //Action to do after player is in range for a certain amount of time
    public float longRangeActionTime = 1.5f; 
}
