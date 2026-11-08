using UnityEngine;

public class Character_Controller : MonoBehaviour
{
    [SerializeField] float _walkSpeed = 2.0f;
    [SerializeField] float _sprintSpeed = 5.335f;
    [SerializeField] float _jumpForce = 256;

    [Space(8)]
    [SerializeField] LayerMask _groundLayers;

    [Space(8)]
    [SerializeField] Transform _main;
    [SerializeField] Transform _lookRoot;
    //[SerializeField] Transform _graphic;

    internal Vector3 _move;
    //internal Vector3 _look;
    internal bool _sprint;

    bool _grounded;
    float _targetSpeed;
    readonly float _colliderRadius = .25f;

    // animation IDs
    //int _animIDSpeed;
    //int _animIDGrounded;
    //int _animIDJump;
    //int _animIDFreeFall;

    Rigidbody _rigidbody;
    //Animator _animator;

    void Awake()
    {
        if (_rigidbody == null) _main.TryGetComponent(out _rigidbody);
        //if (_animator == null) _graphic.TryGetComponent(out _animator);
    }

    //void Start()
    //{
    //    AssignAnimationIDs();
    //}

    public void UpdateBehaviour()
    {
        Movement();
        GraphicsMovements();

        GroundedCheck();

        //_animator.SetBool(_animIDFreeFall, _rigidbody.linearVelocity.y < -1.75f * 1.5f);
    }

    void Movement()
    {
        _targetSpeed = Mathf.MoveTowards(_targetSpeed, (_move.magnitude > 0 ? (_sprint ? _sprintSpeed : _walkSpeed) : 0), (_move.magnitude > 0 ? 8 : 16) * Time.deltaTime);
        //_animator.SetFloat(_animIDSpeed, _targetSpeed);

        Vector3 cameraForward = _lookRoot.forward;
        cameraForward.y = 0;
        cameraForward = cameraForward.normalized;

        // Calculate the movement direction relative to the camera
        Vector3 moveDirection = ((_move.x * _lookRoot.right) + (_move.y * cameraForward)).normalized;
        _rigidbody.MovePosition(_rigidbody.position + (_targetSpeed * Time.deltaTime * moveDirection));
    }

    void GraphicsMovements()
    {
        var playerMeshLookDirection = _lookRoot.forward * _move.y + _lookRoot.right * _move.x;
        playerMeshLookDirection.y = 0f;

        // Rotate the player mesh to face the calculated direction
        //if (playerMeshLookDirection.magnitude > 0) _graphic.rotation = Quaternion.Lerp(_graphic.rotation, Quaternion.LookRotation(playerMeshLookDirection, Vector3.up), Time.deltaTime * 12);
    }

    public void JumpInputBegan()
    {
        if (!_grounded) return;

        //_animator.SetBool(_animIDJump, true);
        _rigidbody.AddForceAtPosition(new Vector3(0, _jumpForce, 0), Vector3.up, ForceMode.Impulse);
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = _main.position;
        spherePosition += _main.up.normalized * (_colliderRadius / 4);

        _grounded = Physics.CheckSphere(spherePosition, _colliderRadius / 2, _groundLayers, QueryTriggerInteraction.Ignore);

        //_animator.SetBool(_animIDGrounded, _grounded);
        //_animator.SetBool(_animIDJump, false);
    }

    //void AssignAnimationIDs()
    //{
    //    _animIDSpeed = Animator.StringToHash("Speed");
    //    _animIDGrounded = Animator.StringToHash("Grounded");
    //    _animIDJump = Animator.StringToHash("Jump");
    //    _animIDFreeFall = Animator.StringToHash("FreeFall");
    //}
}
