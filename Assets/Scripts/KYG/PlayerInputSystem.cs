using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputSystem : MonoBehaviour
{
    private InputSystem_Actions _actions;

    private float _xMoveValue;

    public event Action<Vector2> Move;
    public event Action Jump;
    public event Action Stomp;
    private bool _isStomp;
    private bool _isJump;

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

        _actions.Player.Stomp.performed += OnStomp;
        _actions.Player.Stomp.canceled += OnStomp;

    }
    private void OnDisable()
    {
        _actions.Player.Disable();
        _actions.Player.Move.performed -= OnMove;
        _actions.Player.Move.canceled -= OnMove;

        _actions.Player.Jump.performed -= OnJump;
        _actions.Player.Jump.canceled -= OnJump;

        _actions.Player.Stomp.performed -= OnStomp;
        _actions.Player.Stomp.canceled -= OnStomp;

    }
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        _xMoveValue = moveInput.x;
        Move?.Invoke(moveInput);
    }
    private void OnStomp(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _isStomp = context.ReadValueAsButton();
            Stomp?.Invoke();
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            _isStomp = false;
        }
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _isJump = context.ReadValueAsButton();
            Jump?.Invoke();
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            _isJump = false;
        }
    }

}
