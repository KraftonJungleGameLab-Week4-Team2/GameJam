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
    public event Action<GravitySource> PlanetChanged;

    private GravitySource _gravitySource;
    private GravitySource GravitySource
    {
        get => _gravitySource;
        set
        {
            if (value != _gravitySource)
            {
                PlanetChanged?.Invoke(value);
            }
            _gravitySource = value;
        }
    }
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

    private void PlayerMoveInput(Vector2 value) => _xInput = value.y;

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

        var gravityDir = GetPlanetDir();
        var groundNormal = -gravityDir;

// 2D/횡스크롤 기준 (화면 앞쪽 z축이 고정된 평면일 경우)
// 기준 축(Vector3.forward)과 지면 법선의 외적으로 완벽한 접선(Tangent) 벡터 생성
        Vector3 moveDir = Vector3.Cross(Vector3.forward, groundNormal).normalized;

// _xVelocity 입력 방향(좌/우)에 맞게 곱해줌
        var horizontalVelocity = moveDir * _xVelocity;

// 수직 속도 계산
        if (!_isGrounded)
        {
            _yVelocity -= _gravityInfo.Gravity * Time.fixedDeltaTime;
        }

        var verticalVelocity = groundNormal * _yVelocity;
        _rb.linearVelocity = horizontalVelocity + verticalVelocity;

    }

    private void RotateToGroundNormal()
    {
        Vector3 gravityUp = -GetPlanetDir();

        // 2D 횡스크롤/서클 이동인 경우 화면 앞쪽(Vector3.forward)을 기준으로 안정적으로 회전 생성
        // 만약 Z축 회전 평면 게임이라면:
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;

        // 플레이어가 y축으로 회전하는 경우 방지
        targetRotation.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10.0f * Time.fixedDeltaTime);
    }

    private void CheckGround()
    {
        RaycastHit hit;
        bool hasGroundHit = Physics.Raycast(transform.position, GetPlanetDir(), out hit, _ChkGroundDistance);
        _isGrounded = hasGroundHit && _yVelocity <= 0.0f;
        if (_isGrounded)
        {
            // 그라운드 판정이 처음 들어갈 때
            if (_isGrounded == false)
            {
                if (hit.collider.TryGetComponent<GravitySource>(out var source))
                {
                    GravitySource = source;
                }

                // z축 좌표를 행성 z축 좌표와 고정시키기
                transform.position = new Vector3(transform.position.x, transform.position.y, hit.transform.position.z);
            }

            if (IsStomp)
            {
                SurfaceInstance surface = hit.collider.GetComponentInParent<SurfaceInstance>();
                if (surface != null && surface.Profile != null)
                {
                    FractureEffect effect = surface.Profile.GetEffect<FractureEffect>();
                    effect?.TryFractureFromStomp(surface, hit.point);
                }
            }

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
