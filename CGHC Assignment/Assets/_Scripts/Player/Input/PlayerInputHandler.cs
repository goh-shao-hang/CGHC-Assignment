using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    private Camera cam;

    #region Properties and Fields

    #region Move

    public Vector2 RawMovementInput { get; private set; }
    public int NormalizedInputX { get; private set; }
    public int NormalizedInputY { get; private set; }

    #endregion

    #region Jump

    public bool JumpInput { get; private set; }
    public bool JumpInputStop { get; private set; }

    [SerializeField] private float jumpInputBuffer = 0.2f;
    private float jumpInputStartTime;

    #endregion

    #region Wall Interaction
    public bool GrabInput { get; private set; }
    #endregion

    #region Dash

    public bool DashInput { get; private set; }
    public bool DashInputStop { get; private set; }

    [SerializeField] private float dashInputBuffer = 0.2f;
    private float DashInputStartTime;

    #endregion

    #region DashDirection

    public Vector2 RawDashDirectionInput { get; private set; }
    public Vector2Int DashDirectionInput { get; private set; }

    #endregion

    #endregion

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        cam = Camera.main;
    }

    private void Update()
    {
        CheckJumpInputBuffer();
        CheckDashInputBuffer();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();

        NormalizedInputX = (int)(RawMovementInput * Vector2.right).normalized.x;
        NormalizedInputY = (int)(RawMovementInput * Vector2.up).normalized.y;

        /* Above 2 lines make sure that input in both axis are normalized to be either 0 or 1 depending if there is input. Normalizing a Vector2 directly only normalized the magnitude, not the x and y values.
         * This is NOT SUITABLE for games where the player can move diagonally, eg. top down games and 3d games, since you want the input direction normalized in those cases.
         */
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed) //equivalent to GetButton
        {
            JumpInput = true;
            JumpInputStop = false;
            jumpInputStartTime = Time.time;
        }

        if (context.canceled)
        {
            JumpInputStop = true;
        }
        /* Some notes: 
         * context.started is when the button is pressed (same as GetButtonDown)
         * context.performed is when the button is pressed and reached a certain threshold (strength / held time or both) and only called once (NOT the same as held)
         * context.canceled is when the button is released (same as GetButtonUp)
         * there is nothing equivalent to GetButton in the new input system atm, but there are some more complicated workarounds
         */
    }

    public void OnGrabInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GrabInput = true;
        }

        if (context.canceled)
        {
            GrabInput = false;
        }
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DashInput = true;
            DashInputStop = false;
            DashInputStartTime = Time.time;
        }
        else if (context.canceled)
        {
            DashInputStop = true;
        }
    }

    public void OnDashDirectionInput(InputAction.CallbackContext context)
    {
        RawDashDirectionInput = context.ReadValue<Vector2>();

        if (playerInput.currentControlScheme == "Keyboard") //If using mouse, the input needs to be converted before being used.
        {
            RawDashDirectionInput = cam.ScreenToWorldPoint(RawDashDirectionInput) - transform.position; //Convert mouse position (screen point) to world point, then minus player position to find direction from player to mouse
        }

        DashDirectionInput = Vector2Int.RoundToInt(RawDashDirectionInput.normalized);
    }

    public void UseJumpInput() => JumpInput = false;

    private void CheckJumpInputBuffer()
    {
        if (Time.time >= jumpInputStartTime + jumpInputBuffer)
        {
            JumpInput = false;
        }
    }

    public void UseDashInput() => DashInput = false;

    private void CheckDashInputBuffer()
    {
        if (Time.time >= DashInputStartTime + dashInputBuffer)
        {
            DashInput = false;
        }
    }

}
