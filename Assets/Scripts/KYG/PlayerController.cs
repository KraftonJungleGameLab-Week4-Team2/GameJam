using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IPlayerInput _inputSystem;
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

    private Vector3 GetGroundDir() => (_gravityInfo.PlanetPos - transform.position).normalized;

    private void FixedUpdate()
    {
        CheckGround();
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

    public void Initialize(IGravityInfo gravityInfo, IPlayerInput playerInput, PlayerVari playerVari)
    {
        _gravityInfo = gravityInfo;
        _playerVari = playerVari;
        _gravityInfo = gravityInfo;
    }


    private void PlayerStomp() //내려찍기함수
    {
        if (_isGrounded == false && _isStomp == false)
        {
            _isStomp = true;
            Debug.Log($"{nameof(PlayerController)} : dive");
            _rb.linearVelocity = -transform.up * _playerVari.stompForce;

        }
    }
    private void PlayerJump()
    {
        if (_isGrounded)
        {


            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, 0f);
            //_rb.AddForce(transform.up * _playerVari.jumpForce, ForceMode.Impulse);
            _rb.MovePosition(_rb.position + -GetGroundDir() * 150 * Time.fixedDeltaTime);

        }
    }
    private void PlayerMoveInput(Vector2 value)
    {
        _playerVari.xDir = value.x;
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
