using System.Collections;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 40f;
    private Vector3 _target;

    private Collider _meteorCollider;

    private void Awake()
    {
        _meteorCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        /* _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.AddForce(Vector3.down * 100, ForceMode.Impulse);  후보 1번 메테오 */
        _target = transform.position + Vector3.down * 300f;
        StartCoroutine(MoveFireBall(_target, _moveSpeed));
    }

    private IEnumerator MoveFireBall(Vector3 destination, float moveSpeed)
    {
        float distance = (destination - transform.position).sqrMagnitude;
        while (distance > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        SurfaceInstance surface = other.GetComponentInParent<SurfaceInstance>();
        if (surface == null || surface.Profile == null)
        {
            return;
        }

        FractureEffect fractureEffect = surface.Profile.GetEffect<FractureEffect>();
        if (fractureEffect == null)
        {
            Debug.LogWarning("The planet SurfaceProfile has no FractureEffect for meteor impacts.", surface);
            return;
        }

        Vector3 impactPoint = surface.GetTriggerContactPoint(_meteorCollider);
        fractureEffect.TryFractureFromMeteor(surface, impactPoint);
    }
}