using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class __TEST_PLAYER__ : MonoBehaviour
{
    private IGravityInfo _gravityInfo;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _gravityInfo = FindAnyObjectByType<GravityManager>().GravityInfo;
    }

    private void Update()
    {
        var dir = -(_gravityInfo.GravityOrigin.transform.position - transform.position).normalized;

        if (Input.GetKeyDown(KeyCode.Space)) _rigidbody.AddForce(dir * 500);
    }

    public void FixedUpdate()
    {
        var groundDir = (_gravityInfo.GravityOrigin.transform.position - transform.position).normalized;


        if (Physics.Raycast(transform.position, groundDir, out var hit, 1f))
        {
            if (hit.collider.CompareTag("Respawn"))
                _gravityInfo.ApplyGravity(0.0f);
        }
        else
        {
            _rigidbody.MovePosition(_rigidbody.position + groundDir * _gravityInfo.Gravity * Time.fixedDeltaTime);
        }
    }
}
