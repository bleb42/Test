using UnityEngine;

public class Follower : Mover
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
            Velocity.x = horizontalMotion.x;
            Velocity.z = horizontalMotion.z;
        }
        else
        {
            Velocity.x = 0f;
            Velocity.z = 0f;
        }
    }
}