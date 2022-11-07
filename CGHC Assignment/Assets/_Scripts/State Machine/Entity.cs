using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Assignables
    public FiniteStateMachine StateMachine;

    public Rigidbody2D rb { get; protected set; }
    public Animator anim { get; protected set; }
    #endregion

    #region Variables
    public Vector2 CurrentVelocity { get; private set; }
    protected Vector2 vector2Workspace; //used to set values without creating new vector2\
    public int FacingDirection { get; protected set; }
    #endregion

    #region Unity Callback Functions
    protected virtual void Awake()
    {
        StateMachine = new FiniteStateMachine();
    }

    protected virtual void Start()
    {
        FacingDirection = 1;
    }

    protected virtual void Update()
    {
        CurrentVelocity = rb.velocity;

        StateMachine.CurrentState.LogicUpdate();
    }

    protected virtual void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion

    #region Other Functions
    public void SetVelocityZero()
    {
        rb.velocity = Vector2.zero;
        CurrentVelocity = Vector2.zero;
    }

    public virtual void SetVelocityX(float xVelocity)
    {
        vector2Workspace.Set(xVelocity, CurrentVelocity.y);
        rb.velocity = vector2Workspace;
        CurrentVelocity = vector2Workspace;
    }

    public virtual void SetVelocityY(float yVelocity)
    {
        vector2Workspace.Set(CurrentVelocity.x, yVelocity);
        rb.velocity = vector2Workspace;
        CurrentVelocity = vector2Workspace;
    }

    public virtual void SetVelocity(float velocity, Vector2 angle, int direction) //set velocity towards a specific angle. Note that you should always input a positive angle and only use direction to determine if we should flip that angle in the x axis (based on facing direction or attack direction etc.)
    {
        angle.Normalize();
        vector2Workspace.Set(angle.x * velocity * direction, angle.y * velocity); //Direction multiplied only on x axis to flip knockback horizontally only
        rb.velocity = vector2Workspace;
        CurrentVelocity = vector2Workspace;
    }

    public virtual void SetVelocity(float velocity, Vector2 direction) //Alternate set velocity that simply set velocity towards a direction
    {
        rb.velocity = direction * velocity;
        CurrentVelocity = rb.velocity;
    }

    public virtual void Flip()
    {
        FacingDirection *= -1;
    }
    #endregion
}
