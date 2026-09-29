using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PlayerStat
{
    [Header("StateCheck")]
    public float airMultiplier;
    [Range(0f, 1000f)]

    [Header("Force")]
    public float moveSpeed = 10;
    public float jumpForce = 50;
    public float stompForce = 50;

    public float acceleration = 20f;
    public float deceleration = 20f;
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
    public bool IsStomp { get; private set; }
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
        if (_rb != null)
        {
            // 새 행성 기준의 위쪽 벡터 (Ground Normal)
            Vector3 newGroundNormal = GetPlanetDir();

            // 기존에 날아가던 실제 월드 속도를 새 행성의 수직 축으로 재투영
            _yVelocity = Vector3.Dot(_rb.linearVelocity, newGroundNormal);
        }
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
        if (_isGrounded == false && IsStomp == false)
        {
            IsStomp = true;
            _yVelocity = -_playerStat.stompForce;
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

        // 핵심: transform.right를 행성 표면 평면에 완벽히 투영하여 접선 벡터 추출
        var moveDir = Vector3.ProjectOnPlane(transform.right, groundNormal).normalized;
        var horizontalVelocity = moveDir * _xVelocity;

        var verticalVelocity = Vector3.zero;
        if (_isGrounded == false)
        {
            _yVelocity -= _gravityInfo.Gravity * Time.fixedDeltaTime;
        }
        verticalVelocity = groundNormal * _yVelocity;

        _rb.linearVelocity = horizontalVelocity + verticalVelocity;
    }

    private void RotateToGroundNormal()
    {
        Vector3 gravityUp = -GetPlanetDir();

        // 2D 횡스크롤/서클 이동인 경우 화면 앞쪽(Vector3.forward)을 기준으로 안정적으로 회전 생성
        // 만약 Z축 회전 평면 게임이라면:
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10.0f * Time.fixedDeltaTime);
    }

    private void CheckGround()
    {
        _isGrounded = Physics.Raycast(transform.position, GetPlanetDir(), out var hit, _ChkGroundDistance) && _yVelocity <= 0.0f;
        if (_isGrounded)
        {
            _gravityInfo.ApplyGravity(0.0f);
            IsStomp = false;
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
