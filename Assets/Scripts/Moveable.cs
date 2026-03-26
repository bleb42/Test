using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class Moveable : MonoBehaviour
{
    [SerializeField] protected float _speed = 5f;
    [SerializeField] protected float _gravityFactor = 2f;

    protected CharacterController _characterController;
    protected Vector3 _velocity;

    protected virtual void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    protected virtual void Update()
    {
        ApplyMovement();
        ApplyGravity();

        _characterController.Move(_velocity * Time.deltaTime);
    }

    protected abstract void ApplyMovement();

    protected virtual void ApplyGravity()
    {
        if (_characterController.isGrounded && _velocity.y < 0)
            _velocity.y = 0;
        else
            _velocity.y += Physics.gravity.y * _gravityFactor * Time.deltaTime;
    }
}