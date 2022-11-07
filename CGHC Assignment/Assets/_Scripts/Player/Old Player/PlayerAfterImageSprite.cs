using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{
    private Transform player;
    private SpriteRenderer sr;
    private SpriteRenderer playersr;
    private Color color;

    [SerializeField] private float activeTime = 0.1f;
    [SerializeField] private float startAlpha = 0.8f;
    [SerializeField] private float alphaDecay = 10f;

    private float timeActivated;
    private float currentAlpha;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playersr = player.GetComponent<SpriteRenderer>();

        currentAlpha = startAlpha;
        sr.sprite = playersr.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        timeActivated = Time.time;
    }

    private void Update()
    {
        currentAlpha -= alphaDecay * Time.deltaTime; //Framerate independent
        color = new Color(1f, 1f, 1f, currentAlpha);
        sr.color = color;

        if (Time.time >= (timeActivated + activeTime))
        {
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
