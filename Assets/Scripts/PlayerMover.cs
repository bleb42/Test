using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerMover : Mover
{
    private Vector2 _moveInput;

    public void OnMove(InputValue inputValue)
    {
        _moveInput = inputValue.Get<Vector2>();
    }

    protected override void ApplyMovement()
    {
        Vector3 inputDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);
        inputDirection = Vector3.ClampMagnitude(inputDirection, 1f);

        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        Velocity.x = worldDirection.x * _speed;
        Velocity.z = worldDirection.z * _speed;
    }
}