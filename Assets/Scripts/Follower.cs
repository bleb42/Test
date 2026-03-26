using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Follower : Moveable
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _catchDistance = 1f;

    protected override void ApplyMovement()
    {
        if (_target == null)
            return;

        Vector3 direction = _target.position - transform.position;
        direction.y = 0f;

        if (direction.magnitude > _catchDistance)
        {
            Vector3 horizontalMotion = direction.normalized * _speed;
            _velocity.x = horizontalMotion.x;
            _velocity.z = horizontalMotion.z;
        }
        else
        {
            _velocity.x = 0f;
            _velocity.z = 0f;
        }
    }
}