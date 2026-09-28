using System;
using UnityEngine;

[Serializable]
public class PlayerStat
{
    [Header("StateCheck")]
    public float airMultiplier;
    [Range(0f, 1000f)]

    [Header("Force")]
    public float moveSpeed = 5;
    public float jumpForce = 7;
    public float stompForce = 100;

    public float acceleration = 10f;
    public float deceleration = 10f;
}

[RequireComponent(typeof(SurfaceMovement))]
public class PlayerMovement : MonoBehaviour
{
    private SurfaceMovement _surfaceMovement;
    private PlayerInputSystem _inputSystem;
    private Rigidbody _rb;
    private PlayerStat _playerStat = new PlayerStat();

    [SerializeField] private IGravityInfo _gravityInfo;
    [SerializeField] private float _ChkDownDistance;
    [SerializeField] private float _ChkGroundDistance;

    private bool _isReadyJump;
    private bool _isStomp;
    private bool _isGrounded;
    private float _xInput;
    private Vector3 _moveDir;
    private float _yVelocity;
    private float _xVelocity;

    public void Initialize(IGravityInfo gravityInfo)
    {
        _gravityInfo = gravityInfo;
        _gravityInfo.GravityOriginChanged += OnGravitySourceChanged;
    }

    private void OnGravitySourceChanged(GravitySource source)
    {
        if(source != null)
            _yVelocity = 0.0f;
    }

    private void Awake()
    {
        _surfaceMovement = GetComponent<SurfaceMovement>();
        _rb = GetComponent<Rigidbody>();
        _inputSystem = GetComponent<PlayerInputSystem>();
    }

    private void OnEnable()
    {
        _inputSystem.Move += PlayerMoveInput;
        _inputSystem.Jump += PlayerJump;
        _inputSystem.Stomp += PlayerStomp;
    }

    private void OnDisable()
    {
        _inputSystem.Move -= PlayerMoveInput;
        _inputSystem.Jump -= PlayerJump;
        _inputSystem.Stomp -= PlayerStomp;
    }

    private void PlayerMoveInput(Vector2 value) => _xInput = value.x;

    private void PlayerStomp()
    {
        if (_isGrounded == false && _isStomp == false)
        {
            _isStomp = true;
            _yVelocity = _playerStat.stompForce;
        }
    }

    private void PlayerJump()
    {
        if (_isGrounded)
        {
            _yVelocity = _playerStat.jumpForce;
        }
    }

    private void FixedUpdate()
    {
        CheckGround();
        RotateToGroundNormal();
        ApplyMovement();

    }

    private Vector3 GetPlanetDir()
    {
        if (_gravityInfo?.GravityOrigin != null)
            return (_gravityInfo.GravityOrigin.transform.position - transform.position).normalized;
        else
            return Vector3.zero;
    }

    private void ApplyMovement()
    {
        var accel = _playerStat.acceleration * _surfaceMovement.Current.accelerationMultiplier;
        var decel = _playerStat.deceleration * _surfaceMovement.Current.decelerationMultiplier;
        if (_xInput > 0)
        {
            _xVelocity = Mathf.Lerp(_xVelocity, _playerStat.moveSpeed, Time.fixedDeltaTime * accel);
        }
        else if (_xInput < 0)
        {
            _xVelocity = Mathf.Lerp(_xVelocity, -_playerStat.moveSpeed, Time.fixedDeltaTime * accel);
        }
        else
        {
            _xVelocity = Mathf.Lerp(_xVelocity, 0.0f, Time.fixedDeltaTime * decel);
        }

        Debug.Log(_xVelocity);
        var gravityDir = GetPlanetDir();
        var groundNormal = -gravityDir;
        var moveDir = transform.right;
        var horizontalVelocity = Vector3.ProjectOnPlane(moveDir * _xVelocity, groundNormal) ;

        if (_isGrounded == false)
        {
            _yVelocity -= _gravityInfo.Gravity * Time.fixedDeltaTime;
        }

        var verticalVelocity = _yVelocity * groundNormal;
        _rb.linearVelocity = horizontalVelocity + verticalVelocity;
    }

    private void RotateToGroundNormal()
    {
        Vector3 gravityUp = -GetPlanetDir();
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, gravityUp);

        if (forward.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forward, gravityUp);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3f * Time.fixedDeltaTime);
        }
    }

    private void CheckGround()
    {
        _isGrounded = Physics.Raycast(transform.position, GetPlanetDir(), out var hit, _ChkGroundDistance) && _yVelocity <= 0.0f;
        if (_isGrounded)
        {
            _gravityInfo.ApplyGravity(0.0f);
            _isStomp = false;
        }
        Debug.DrawRay(transform.position, GetPlanetDir() * _ChkGroundDistance, Color.red);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), $"{_rb.linearVelocity} / {_yVelocity}");
    }

    private void OnDestroy()
    {
        if(_gravityInfo != null)
            _gravityInfo.GravityOriginChanged -= OnGravitySourceChanged;
    }
}
