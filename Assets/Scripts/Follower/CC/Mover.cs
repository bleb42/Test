using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class Mover : MonoBehaviour
{
    [SerializeField] protected float _speed = 5f;
    [SerializeField] protected float _gravityFactor = 2f;
    [SerializeField] private float _groundedDownForce = 2f;
    [SerializeField] private float _groundStickDistance = 0.2f;

    protected CharacterController CharacterController;
    protected Vector3 Velocity;

    protected virtual void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
    }

    protected virtual void Update()
    {
        ApplyMovement();
        ApplyGravity();

        CharacterController.Move(Velocity * Time.deltaTime);
    }

    protected abstract void ApplyMovement();

    protected virtual void ApplyGravity()
    {
        if (CharacterController.isGrounded)
        {
            if (Velocity.y < 0f)
                Velocity.y = -_groundedDownForce;
        }
        else if (IsNearGround())
        {
            Velocity.y -= Physics.gravity.magnitude * _gravityFactor * Time.deltaTime;
            Velocity.y = Mathf.Max(Velocity.y, -Physics.gravity.magnitude);
        }
        else
        {
            Velocity.y += Physics.gravity.y * _gravityFactor * Time.deltaTime;
        }
    }

    private bool IsNearGround()
    {
        float checkDistance = CharacterController.height * 0.5f + CharacterController.skinWidth + _groundStickDistance;

        return Physics.SphereCast(transform.position, CharacterController.radius, Vector3.down, out RaycastHit _, checkDistance);
    }
}