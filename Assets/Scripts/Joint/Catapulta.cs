using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Catapulta : MonoBehaviour
{
    private const string ShootAction = "Shoot";
    private const string PullAction = "Pull";

    [SerializeField] private SpringJoint _springJoint;
    [SerializeField] private float _projectileRespawnDelay = 2f;

    [Header("Shoot")]
    [SerializeField] private float _shootSpring = 1000f;
    [SerializeField] private float _shootDamper = 0f;
    [SerializeField] private Rigidbody _upPoint;

    [Header("Pull")]
    [SerializeField] private float _pullSpring = 10f;
    [SerializeField] private float _pullDamper = 1000f;
    [SerializeField] private Rigidbody _downPoint;

    [Header("Projectile")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _spawnPoint;

    [SerializeField] private InputActionAsset _inputActions;

    private InputAction _shootAction;
    private InputAction _pullAction;
    private Coroutine _returnCoroutine;

    private bool _isPulled = false;

    private void Awake()
    {
        _shootAction = _inputActions.FindAction(ShootAction);
        _pullAction = _inputActions.FindAction(PullAction);
    }

    private void Start()
    {
        SetJoint(_pullSpring, _pullDamper, _downPoint.position);
        SpawnProjectile();
    }

    private void OnEnable()
    {
        _shootAction.Enable();
        _pullAction.Enable();
        _shootAction.performed += OnShoot;
        _pullAction.performed += OnPull;
    }

    private void OnDisable()
    {
        _shootAction.performed -= OnShoot;
        _pullAction.performed -= OnPull;
        _shootAction.Disable();
        _pullAction.Disable();
    }

    private void OnPull(InputAction.CallbackContext callbackContext)
    {
        if (_isPulled)
            return;

        _isPulled = true;
        SetJoint(_pullSpring, _pullDamper, _downPoint.position);

        if (_returnCoroutine != null)
            StopCoroutine(_returnCoroutine);

        _returnCoroutine = StartCoroutine(WaitForReturn());
    }

    private void OnShoot(InputAction.CallbackContext callbackContext)
    {
        if (_isPulled == false) 
            return;

        _isPulled = false;
        SetJoint(_shootSpring, _shootDamper, _upPoint.position);
    }

    private IEnumerator WaitForReturn()
    {
        yield return new WaitForSeconds(_projectileRespawnDelay);

        SpawnProjectile();
    }

    private void SpawnProjectile()
    {
        Instantiate(_projectilePrefab, _spawnPoint.position, Quaternion.identity);
    }

    private void SetJoint(float spring, float damper, Vector3 connectedAnchor)
    {
        _springJoint.spring = spring;
        _springJoint.damper = damper;
        _springJoint.connectedAnchor = connectedAnchor;
    }
}
