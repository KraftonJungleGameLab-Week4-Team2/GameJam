using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputSystem _inputSystem;
    private Rigidbody _rb;
    private float _xDir;
    private bool _IsReadyJump;
    private Vector3 _groundNormal;
    [SerializeField] private bool _IsGrounded;
    [SerializeField] private bool _jumpCooldown;
    [SerializeField] private Vector3 _moveDir;
    [SerializeField] private float _moveSpeed;
    RaycastHit hit;




    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inputSystem = GetComponent<PlayerInputSystem>();
    }

    void Start()
    {

    }
    private void OnEnable()
    {
        _inputSystem.Move += PlayerMoveInput;
        _inputSystem.Jump += PlayerJump;
        _inputSystem.Dive += PlayerDive;
    }

    void Update()
    {

    }
    private void FixedUpdate()
    {
        CheckGround();
        PlayerMove();
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, _groundNormal) * transform.rotation;
        transform.rotation = targetRotation;
    }
    private void PlayerDive()
    {

    }
    private void PlayerJump()
    {

    }
    private void PlayerMoveInput(Vector2 value)
    {
        _xDir = value.x;
    }
    private void PlayerMove() //플레이어 움직임 + 속도 초기화 함수호출
    {
        _moveDir = _xDir * this.transform.right;
        Vector3 slopeMoveDir = Vector3.ProjectOnPlane(_moveDir, _groundNormal);
        _rb.AddForce(slopeMoveDir.normalized * _moveSpeed, ForceMode.Force);
        SpeedControl();

    }

    private void SpeedControl() //AddForce로 인한 속도 누적 초기화
    {
        Vector3 floatVelocity = new Vector3(_rb.linearVelocity.x, 0f, 0f);
        if (floatVelocity.magnitude > _moveSpeed)
        {
            Vector3 limitedVelocity = floatVelocity.normalized * _moveSpeed;
            _rb.linearVelocity = new Vector3(limitedVelocity.x, _rb.linearVelocity.y, _rb.linearVelocity.z);
        }
    }
    private void CheckGround()
    {
        _IsGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 3f);
        Debug.DrawRay(transform.position, -transform.up, Color.red, 1f);
        _groundNormal = hit.normal;
    }
}
