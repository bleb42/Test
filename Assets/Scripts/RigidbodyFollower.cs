using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class RigidbodyFollower : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _target;
    [SerializeField] private float _catchDistance = 1.2f;

    [Header("Movement")]
    [SerializeField] private float _speed = 5f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _groundCheckOffset = 0.1f;
    [SerializeField] private float _groundCheckDistance = 0.15f;
    [SerializeField] private float _groundSphereRadius = 0.5f;

    [Header("Slope")]
    [SerializeField] private float _maxSlopeAngle = 45f;
    [SerializeField] private float _slopeRayDistance = 0.7f;
    [SerializeField] private float _slopeRayVerticalBias = 0.5f;
    [SerializeField] private float _slopeProjectRayDistance = 0.3f;

    [Header("Step")]
    [SerializeField] private float _stepOffset = 0.51f;
    [SerializeField] private float _stepCheckDistance = 0.05f;
    [SerializeField] private float _stepRayOffset = 0.15f;

    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;
    private Vector3 _lastMoveDirection;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        if (_target == null)
            return;

        bool isGrounded = IsGrounded();

        Vector3 direction = _target.position - transform.position;
        direction.y = 0f;

        if (direction.magnitude <= _catchDistance)
        {
            _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);

            return;
        }

        direction = direction.normalized;

        if (CanWalkOnSlope(direction) == false)
        {
            _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);

            return;
        }

        Vector3 moveDirection = isGrounded ? ProjectOnSlope(direction) : direction;

        if(TryClimbStep(moveDirection) == false)
            _rigidbody.linearVelocity = new Vector3(moveDirection.x * _speed, _rigidbody.linearVelocity.y, moveDirection.z * _speed);
    }

    private bool IsGrounded()
    {
        float checkDistance = _collider.height * 0.5f + _groundCheckDistance;
        float sphereRadius = _collider.radius * _groundSphereRadius;
        Vector3 origin = transform.position + Vector3.up * _groundCheckOffset;

        return Physics.SphereCast(origin, sphereRadius, Vector3.down, out RaycastHit hitInfo, checkDistance, _groundMask);
    }

    private bool CanWalkOnSlope(Vector3 moveDirection)
    {
        Vector3 rayDirection = moveDirection + Vector3.down * _slopeRayVerticalBias;

        if (Physics.Raycast(transform.position + Vector3.up * _groundCheckOffset, rayDirection, out RaycastHit hit, _slopeRayDistance, _groundMask))
            return Vector3.Angle(hit.normal, Vector3.up) <= _maxSlopeAngle;

        return true;
    }

    private Vector3 ProjectOnSlope(Vector3 moveDirection)
    {
        float checkDistance = _collider.height * 0.5f + _slopeProjectRayDistance;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, checkDistance, _groundMask))
            return Vector3.ProjectOnPlane(moveDirection, hit.normal).normalized;

        return moveDirection;
    }

    private bool TryClimbStep(Vector3 moveDirection)
    {
        _lastMoveDirection = moveDirection;

        Vector3 rayPoint = new Vector3(transform.position.x, transform.position.y - _collider.height * 0.5f + _stepRayOffset, transform.position.z);

        bool isBlockedLow = Physics.Raycast(rayPoint, moveDirection, _collider.radius + _stepCheckDistance, _groundMask);

        rayPoint.y += _stepOffset;

        bool isBlockedAbove = Physics.Raycast(rayPoint, moveDirection, _collider.radius + _stepCheckDistance, _groundMask);

        if (isBlockedLow && isBlockedAbove == false)
        {
            Physics.Raycast(rayPoint + _lastMoveDirection * (_collider.radius + _stepCheckDistance), Vector3.down, out RaycastHit hitInfo, _stepOffset, _groundMask);
            Vector3 targetPosition = new Vector3(transform.position.x + moveDirection.x * _speed * Time.fixedDeltaTime, hitInfo.point.y + _collider.height * 0.5f, transform.position.z + moveDirection.z * _speed * Time.fixedDeltaTime);

            _rigidbody.MovePosition(targetPosition);
            _rigidbody.linearVelocity = Vector3.zero;

            return true;
        }

        return false;
    }
}