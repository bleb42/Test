using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerMover : Moveable
{
    private Vector2 _moveInput;

    public void OnMove(InputValue inputValue)
    {
        _moveInput = inputValue.Get<Vector2>();
    }

    protected override void ApplyMovement()
    {
        Vector3 inputDirection = new Vector3(_moveInput.x, 0, _moveInput.y);

        if (inputDirection.magnitude > 1)
            inputDirection.Normalize();

        Vector3 move = transform.TransformDirection(inputDirection);

        _velocity.x = move.x * _speed;
        _velocity.z = move.z * _speed;
    }
}