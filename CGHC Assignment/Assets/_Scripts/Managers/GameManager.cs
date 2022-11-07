using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get; private set; }

    private CinemachineVirtualCamera cvc;

    [Header("Assignables")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject player;
    [SerializeField] private float respawnTime;

    private float respawnTimeStart;
    private bool respawn;

    private void Awake()
    {
        Instance = this;
        cvc = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        CheckRespawn();
    }

    public void Respawn()
    {
        respawnTimeStart = Time.time;
        respawn = true;
    }

    private void CheckRespawn()
    {
        if (Time.time >= respawnTimeStart + respawnTime && respawn)
        {
            respawn = false;
            var playerTemp = Instantiate(player, respawnPoint.position, player.transform.rotation);
            cvc.m_Follow = playerTemp.transform;
        }
    }
}
