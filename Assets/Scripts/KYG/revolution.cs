using UnityEngine;

public class revolution : MonoBehaviour
{
    public Transform target;
    private Rigidbody _rigidbody;
    [SerializeField] private float rotateSpeed = 0.05f;

    [SerializeField] private float _radius = 40f; //거리

    [Range(0, 6.28f)]
    [SerializeField] private float _angle; //행성의 각도

    //private void Awake()
    //{
    //    st();
    //}
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

    }


    private void FixedUpdate()
    {
        Revolution();

    }
    private void Revolution()
    {
        float x = target.position.x + Mathf.Cos(_angle) * _radius;
        float y = target.position.y + Mathf.Sin(_angle) * _radius;
        float z = target.position.z;
        Vector3 nextPos = new Vector3(x, y, z);
        _rigidbody.linearVelocity = (nextPos - _rigidbody.position) / Time.fixedDeltaTime; //

        _angle += Time.fixedDeltaTime * rotateSpeed;
    }

}
