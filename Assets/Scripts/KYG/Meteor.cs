using System.Collections;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 40f;
    [SerializeField] private float _lifeTime = 8f;
    private Vector3 _target;
    private bool _hasLaunchTarget;

    private Collider _meteorCollider;
    private bool _hasHitPlayer;

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

        FaceDirection(_target - transform.position);

        StartCoroutine(MoveFireBall(_target, _moveSpeed));
        Destroy(gameObject, _lifeTime);
    }

    public void Launch(Vector3 targetPlanetPosition)
    {
        Vector3 direction = (targetPlanetPosition - transform.position).normalized;
        _target = targetPlanetPosition + direction * 300f;
        _hasLaunchTarget = true;
        FaceDirection(direction);
    }

    private IEnumerator MoveFireBall(Vector3 destination, float moveSpeed)
    {
        while ((destination - transform.position).sqrMagnitude > 0.0001f)
        {
            Vector3 direction = destination - transform.position;
            FaceDirection(direction);
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void FaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.right, direction.normalized);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerStatus player = other.GetComponentInParent<PlayerStatus>();
        if (player != null && !_hasHitPlayer)
        {
            _hasHitPlayer = true;
            player.TakeDamage(1f);
            return;
        }

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
