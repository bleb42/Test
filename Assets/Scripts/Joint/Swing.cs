using UnityEngine;
using UnityEngine.InputSystem;

public class Swing : MonoBehaviour
{
    private const string SwingAction = "Swing";

    [SerializeField] private float _force = 5f;
    [SerializeField] private Vector3 _direction = Vector3.forward;
    [SerializeField] private ForceMode _forceMode;
    [SerializeField] private InputActionAsset _inputActions;

    private Rigidbody _rigidbody;
    private InputAction _swingAction;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _swingAction = _inputActions.FindAction(SwingAction);
    }

    private void OnEnable()
    {
        _swingAction.Enable();
        _swingAction.performed += OnSwing;
    }

    private void OnDisable()
    {
        _swingAction.performed -= OnSwing;
        _swingAction.Disable();
    }

    private void OnSwing(InputAction.CallbackContext ctx)
    {
        _rigidbody.AddForce(_direction * _force, _forceMode);
    }
}
