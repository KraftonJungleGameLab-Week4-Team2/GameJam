using System.Collections;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 40f;
    [SerializeField] private float _lifeTime = 8f;
    private Vector3 _target;
    private bool _hasLaunchTarget;

    private Collider _meteorCollider;

    private void Awake()
    {
        _meteorCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        if (!_hasLaunchTarget)
        {
            _target = transform.position + Vector3.down * 300f;
        }

        StartCoroutine(MoveFireBall(_target, _moveSpeed));
        Destroy(gameObject, _lifeTime);
    }

    public void Launch(Vector3 targetPlanetPosition)
    {
        Vector3 direction = (targetPlanetPosition - transform.position).normalized;
        _target = targetPlanetPosition + direction * 300f;
        _hasLaunchTarget = true;
    }

    private IEnumerator MoveFireBall(Vector3 destination, float moveSpeed)
    {
        while ((destination - transform.position).sqrMagnitude > 0.0001f)
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
            return;
        }

        Vector3 impactPoint = surface.GetTriggerContactPoint(_meteorCollider);
        fractureEffect.TryFractureFromMeteor(surface, impactPoint);
    }
}
