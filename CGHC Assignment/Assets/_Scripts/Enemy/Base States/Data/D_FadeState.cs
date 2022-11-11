using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newFadeStateData", menuName = "Data/State Data/Fade State")]
public class D_FadeState : ScriptableObject
{
    public float fadeCooldown = 10f;
    public float reappearTime = 3f;
    public float appearXOffset;
}
