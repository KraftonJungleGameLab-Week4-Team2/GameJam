using System;
using UnityEngine;

[Serializable]
public class PlayerVari
{
    [Header("StateCheck")]
    public float airMultiplier;
    [Range(0f, 1000f)]

    [Header("Force")]
    public float moveSpeed = 10;
    public float jumpForce = 10;
    public float stompForce = 10;

    public float xDir;
    public Vector3 moveDir;
}

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputSystem _inputSystem;
    private Rigidbody _rb;
    private PlayerVari _playerVari = new PlayerVari();
    [SerializeField] private IGravityInfo _gravityInfo;
    [SerializeField] private float _ChkDownDistance;
    [SerializeField] private float _ChkGroundDistance;
    private Vector3 _isDownNormal;
    private bool _isReadyJump;
    private bool _isStomp;
    private bool _isDown;
    public bool _isGrounded;
    RaycastHit hit;



    private void Awake()
    {
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

    private void FixedUpdate()
    {
        CheckGround();
        PlayerMove();
        RotateToGroundNormal();


        var groundDir = (_gravityInfo.PlanetPos - transform.position).normalized;
        _rb.MovePosition(_rb.position + groundDir * _gravityInfo.Gravity * Time.fixedDeltaTime);
        //_rb.AddForce(groundDir * _gravityInfo.Gravity * 15, ForceMode.Force); addFoce버전


        //if (Physics.Raycast(transform.position, groundDir, out var hit, 1f)) 원래의 조건
        //{
        //    if (hit.collider.CompareTag("Respawn"))
        //        _gravityInfo.ApplyGravity(0.0f);
        //}
        //else
        //{
        //    _rb.MovePosition(_rb.position + groundDir * _gravityInfo.Gravity * Time.fixedDeltaTime);
        //}
        if (_isGrounded)
        {
            _gravityInfo.ApplyGravity(0.0f); // 미정
        }
    }

    public void Initialize(IGravityInfo gravityInfo)
    {
        _gravityInfo = gravityInfo;
    }

    private void RotateToGroundNormal() => transform.rotation = Quaternion.FromToRotation(transform.up, _isDownNormal) * transform.rotation;

    private void PlayerStomp() //내려찍기함수
    {
        if (_isGrounded == false && _isStomp == false)
        {
            _isStomp = true;
            Debug.Log($"{nameof(PlayerMovement)} : dive");
            _rb.linearVelocity = -transform.up * _playerVari.stompForce;

        }
    }
    private void PlayerJump()
    {
        if (_isGrounded)
        {

            var groundDir = (_gravityInfo.PlanetPos - transform.position).normalized;
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, 0f);
            _rb.AddForce(transform.up * _playerVari.jumpForce, ForceMode.Impulse);


        }
    }
    private void PlayerMoveInput(Vector2 value)
    {
        _playerVari.xDir = value.x;
    }
    private void PlayerMove() //플레이어 움직임 + 속도 초기화 함수호출
    {
        _playerVari.moveDir = _playerVari.xDir * this.transform.right;
        Vector3 slopeMoveDir = Vector3.ProjectOnPlane(_playerVari.moveDir, _isDownNormal);
        //_rb.AddForce(slopeMoveDir.normalized * _playerVari.moveSpeed, ForceMode.Force);

        var groundDir = (_gravityInfo.PlanetPos - transform.position).normalized;
        _rb.MovePosition(_rb.position + slopeMoveDir * _playerVari.moveSpeed * Time.fixedDeltaTime);

        SpeedControl();

    }

    private void SpeedControl() //AddForce로 인한 속도 누적 초기화
    {
        Vector3 floatVelocity = new Vector3(_rb.linearVelocity.x, 0f, 0f);
        if (floatVelocity.magnitude > _playerVari.moveSpeed)
        {
            Vector3 limitedVelocity = floatVelocity.normalized * _playerVari.moveSpeed;
            _rb.linearVelocity = new Vector3(limitedVelocity.x, _rb.linearVelocity.y, _rb.linearVelocity.z);
        }
    }
    private void CheckGround()
    {
        _isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, _ChkGroundDistance);
        if (_isGrounded)
        {
            _isStomp = false;
        }
        _isDown = Physics.Raycast(transform.position, -transform.up, out hit, _ChkDownDistance);
        Debug.DrawRay(transform.position, -transform.up, Color.red, 1f);
        _isDownNormal = hit.normal;
    }

}
