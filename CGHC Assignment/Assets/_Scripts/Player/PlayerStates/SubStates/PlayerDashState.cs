using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerAbilityState
{
    public bool CanDash { get; private set; }

    private float lastDashTime;
    private bool isHolding;
    
    private bool dashInputStop;
    private Vector2 dashDirectionInput;

    private Vector2 dashDirection;

    private Vector2 lastAfterImagePos;

    public PlayerDashState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, PlayerData playerData) : base(entity, stateMachine, animBoolName, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();

        CanDash = false;
        player.InputHandler.UseDashInput();

        isHolding = true;
        dashDirection = Vector2.right * player.FacingDirection; //Default dash direction

        Time.timeScale = playerData.holdTimeScale;
        startTime = Time.unscaledTime; //Time.time will not work after the first dash since it is scaled during the slowmo

        player.DashDirectionIndicator.gameObject.SetActive(true);
    }

    public override void Exit()
    {
        base.Exit();

        if (player.CurrentVelocity.y > 0) //Limit y velocity if finish dashing up. We only need to care about upwards y velocity since x velocity is constantly handled, and we don't want to decrease downwards velocity when dashing down
        {
            player.SetVelocityY(player.CurrentVelocity.y * playerData.dashEndYMultipler);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isExitingState) return;

        player.anim.SetFloat("yVelocity", player.CurrentVelocity.y);
        player.anim.SetFloat("xVelocity", Mathf.Abs(player.CurrentVelocity.x));

        if (isHolding) //In slowmo phase
        {
            dashDirectionInput = player.InputHandler.DashDirectionInput;
            dashInputStop = player.InputHandler.DashInputStop;  

            if (dashDirectionInput != Vector2.zero) //Only update dash direction if the input is not zero
            {
                dashDirection = dashDirectionInput.normalized;
            }

            float angle = Vector2.SignedAngle(Vector2.right, dashDirection); //Return angle between two Vector2
            player.DashDirectionIndicator.rotation = Quaternion.Euler(0f, 0f, angle - 45); //substractby 45 degree since sprite is rotated by 45 degree

            if (dashInputStop || Time.unscaledTime >= startTime + playerData.maxHoldTime)
            {
                isHolding = false;
                Time.timeScale = 1f;
                startTime = Time.time; //Reset start time to unscaled time to track how long have we dashed for
                player.CheckIfShouldFlip(Mathf.RoundToInt(dashDirection.x)); //Flip if dashing backwards
                player.rb.drag = playerData.drag;
                player.SetVelocity(playerData.dashSpeed, dashDirection);
                player.DashDirectionIndicator.gameObject.SetActive(false);
                PlaceAfterImage();
            }
        }
        else //is dashing
        {
            player.SetVelocity(playerData.dashSpeed, dashDirection);
            CheckIfShouldPlaceAfterImage();
            if (Time.time >= startTime + playerData.dashTime)
            {
                player.rb.drag = 0f;
                isAbilityDone = true; 
                lastDashTime = Time.time;
            }
        }
    }

    public bool CheckIfCanDash => CanDash && Time.time >= lastDashTime + playerData.dashCooldown;

    public void ResetCanDash() => CanDash = true;

    private void CheckIfShouldPlaceAfterImage()
    {
        if (Vector2.Distance(player.transform.position, lastAfterImagePos) > playerData.distanceBetweenAfterImages)
        {
            PlaceAfterImage();
        }
    }

    private void PlaceAfterImage()
    {
        PlayerAfterImagePool.Instance.GetFromPool();
        lastAfterImagePos = player.transform.position;
    }
}
