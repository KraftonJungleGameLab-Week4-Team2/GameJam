using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private IPlayerInput _playerInput;
    private PlayerVari _playerVari;
    private Rigidbody _rb;
    private IGravityInfo _gravityInfo;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    private void RotateToGroundNormal() => transform.rotation = Quaternion.FromToRotation(transform.up, -GetGroundDir()) * transform.rotation;

    public void Initialize(IPlayerInput playerInput, PlayerVari playerVari, IGravityInfo gravityInfo)
    {
        _playerInput = playerInput;
        _playerVari = playerVari;
        _gravityInfo = gravityInfo;
    }

    private Vector3 GetGroundDir() => (_gravityInfo.PlanetPos - transform.position).normalized;

    private void PlayerMove() //플레이어 움직임 + 속도 초기화 함수호출
    {
        _playerVari.moveDir = _playerVari.xDir * this.transform.right;
        Vector3 slopeMoveDir = Vector3.ProjectOnPlane(_playerVari.moveDir, GetGroundDir());
        //_rb.AddForce(slopeMoveDir.normalized * _playerVari.moveSpeed, ForceMode.Force);


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
}
