using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class Moveable : MonoBehaviour
{
    [SerializeField] protected float _speed = 5f;
    [SerializeField] protected float _gravityFactor = 2f;
    [SerializeField] private float _groundedDownForce = 2f;
    [SerializeField] private float _groundStickDistance = 0.2f;

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
        if (_characterController.isGrounded)
        {
            if (_velocity.y < 0f)
                _velocity.y = -_groundedDownForce;
        }
        else if (IsNearGround())
        {
            _velocity.y -= Physics.gravity.magnitude * _gravityFactor * Time.deltaTime;
            _velocity.y = Mathf.Max(_velocity.y, -Physics.gravity.magnitude);
        }
        else
        {
            _velocity.y += Physics.gravity.y * _gravityFactor * Time.deltaTime;
        }
    }

    private bool IsNearGround()
    {
        float checkDistance = _characterController.height * 0.5f + _characterController.skinWidth + _groundStickDistance;

        return Physics.SphereCast(transform.position, _characterController.radius, Vector3.down, out RaycastHit _, checkDistance);
    }
}