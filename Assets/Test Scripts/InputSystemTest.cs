using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemTest : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed = 5f;
    [SerializeField] protected float _sprintSpeed = 1.2f;
    protected Rigidbody _rb;
    protected float _movementX;
    protected float _movementY;
    protected float _tempSprintSpeed = 1f;

    InputAction moveAction;
    InputAction sprintAction;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
    }

    private void Update()
    {
        if (sprintAction.IsPressed())
            _tempSprintSpeed = _sprintSpeed;
        else
            _tempSprintSpeed = 1f;
    }

    private void FixedUpdate()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        _movementX = moveValue.x;
        _movementY = moveValue.y;
        Vector3 movement = new Vector3(_movementX, 0.0f, _movementY);
        _rb.AddRelativeForce(movement * _moveSpeed * _tempSprintSpeed);
    }
}
