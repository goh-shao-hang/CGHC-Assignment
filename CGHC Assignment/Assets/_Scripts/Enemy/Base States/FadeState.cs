using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeState : EnemyState
{
    protected D_FadeState stateData;
    protected Transform player;
    protected float fadeTime;
    protected bool isFaded;
    protected bool fadeFinished;

    public FadeState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_FadeState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        base.Enter();

        isFaded = false;
        fadeFinished = false;
        enemy.anim.SetBool("fadeFinished", false);
        player = GameObject.Find("Player").transform;
    }

    public override void Exit()
    {
        base.Exit();

        player = null;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= fadeTime + stateData.reappearTime && isFaded)
        {
            Reappear();
        }
    }

    public void StartFade()
    {
        fadeTime = Time.time;
        isFaded = true;
    }

    public void Reappear()
    {
        float playerFacingDir = player.GetComponent<PlayerController>().GetFacingDirection();
        Vector3 targetDestination = new Vector3(player.position.x, enemy.transform.position.y, enemy.transform.position.z);
        RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.right * -playerFacingDir, stateData.appearXOffset, enemy.enemyData.whatIsGround);
        if (hit.collider != null)
        {
            targetDestination.x = hit.point.x;
        }
        else
        {
            targetDestination.x = player.position.x + stateData.appearXOffset * -playerFacingDir;
        }

        enemy.transform.position = targetDestination;

        if (enemy.transform.position.x > player.transform.position.x && enemy.FacingDirection != -1 || enemy.transform.position.x < player.transform.position.x && enemy.FacingDirection != 1)
        {
            enemy.Flip();
        }

        isFaded = false;
        enemy.anim.SetBool("fadeFinished", true);
    }

    public void FinishFade()
    {
        fadeFinished = true;
    }
}
