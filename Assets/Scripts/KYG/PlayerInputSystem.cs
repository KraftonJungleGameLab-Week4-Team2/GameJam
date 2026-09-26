using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputSystem : MonoBehaviour
{
    private InputSystem_Actions _actions;
    private float _xMoveValue;

    public event Action<Vector2> Move;
    public event Action Jump;
    public event Action Dive;
    public bool IsDive;
    public bool IsJump;

    private void Awake()
    {
        _actions = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        _actions.Player.Enable();
        _actions.Player.Move.performed += OnMove;
        _actions.Player.Move.canceled += OnMove;

        _actions.Player.Jump.performed += OnJump;
        _actions.Player.Jump.canceled += OnJump;

        _actions.Player.Dive.performed += OnDive;
        _actions.Player.Dive.canceled += OnDive;
    }
    private void OnDisable()
    {
        _actions.Player.Disable();
        _actions.Player.Move.performed -= OnMove;
        _actions.Player.Move.canceled -= OnMove;

        _actions.Player.Jump.performed -= OnJump;
        _actions.Player.Jump.canceled -= OnJump;

        _actions.Player.Dive.performed -= OnDive;
        _actions.Player.Dive.canceled -= OnDive;
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        _xMoveValue = moveInput.x;
        Move?.Invoke(moveInput);
    }
    private void OnDive(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            IsDive = context.ReadValueAsButton();
            Dive?.Invoke();
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            IsDive = false;
        }
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            IsJump = context.ReadValueAsButton();
            Jump?.Invoke();
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            IsJump = false;
        }
    }
}
