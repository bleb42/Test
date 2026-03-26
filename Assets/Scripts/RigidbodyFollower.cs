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

    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;
    private bool _isGrounded;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        CheckGround();
        ApplyMovement();
    }

    private void CheckGround()
    {
        float checkDistance = _collider.height * 0.5f + _groundCheckDistance;
        float sphereRadius = _collider.radius * _groundSphereRadius;
        Vector3 origin = transform.position + Vector3.up * _groundCheckOffset;

        _isGrounded = Physics.SphereCast(origin, _groundSphereRadius, Vector3.down, out RaycastHit hitInfo, checkDistance, _groundMask);
    }

    private void ApplyMovement()
    {
        if (_target == null)
            return;

        Vector3 target = _target.position - transform.position;
        target.y = 0f;

        if (target.magnitude <= _catchDistance)
        {
            _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);

            return;
        }

        Vector3 direction = target.normalized;

        if (CanWalkOnSlope(direction) == false)
        {
            _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);

            return;
        }

        Vector3 moveDirection = _isGrounded ? ProjectOnSlope(direction) : direction;

        _rigidbody.linearVelocity = new Vector3(moveDirection.x * _speed, _rigidbody.linearVelocity.y, moveDirection.z * _speed);
    }

    private bool CanWalkOnSlope(Vector3 direction)
    {
        Vector3 rayDirection = direction + Vector3.down * _slopeRayVerticalBias;

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
}