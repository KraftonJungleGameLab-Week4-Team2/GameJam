using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 60f;
    [SerializeField] private float lifeTime = 5f;

    private Vector3 _waypoint;
    private Transform _target;
    private Vector3 _direction;
    private Collider _bulletCollider;
    private bool _hasHit;

    private void Awake()
    {
        _bulletCollider = GetComponent<Collider>();
    }

    public void Init(Vector3 waypoint, Transform target)
    {
        _waypoint = waypoint;
        _target = target;
        _direction = (_waypoint - transform.position).normalized;
        FaceDirection(_direction);
        StartCoroutine(Shot());
    }

    private IEnumerator Shot()
    {
        while ((_waypoint - transform.position).sqrMagnitude > 0.01f)
        {
            Vector3 toWaypoint = _waypoint - transform.position;
            _direction = toWaypoint.normalized;
            FaceDirection(_direction);

            float step = speed * Time.deltaTime;
            if (MoveAndCheckHit(_direction, step))
            {
                yield break;
            }

            transform.position = Vector3.MoveTowards(transform.position, _waypoint, step);
            yield return null;
        }

        if (_target != null)
        {
            _direction = (_target.position - transform.position).normalized;
            FaceDirection(_direction);
        }

        float elapsed = 0f;
        while (elapsed < lifeTime && !_hasHit)
        {
            FaceDirection(_direction);
            float step = speed * Time.deltaTime;
            if (MoveAndCheckHit(_direction, step))
            {
                yield break;
            }

            transform.position += _direction * step;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private bool MoveAndCheckHit(Vector3 direction, float distance)
    {
        if (_bulletCollider is not BoxCollider box || distance <= 0f)
        {
            return false;
        }

        Vector3 scale = box.transform.lossyScale;
        Vector3 halfExtents = Vector3.Scale(box.size,
            new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z))) * 0.5f;
        RaycastHit[] hits = Physics.BoxCastAll(box.bounds.center, halfExtents, direction,
            transform.rotation, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);

        float closestDistance = float.MaxValue;
        Collider closestCollider = null;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            if (IsPlayer(hit.collider.transform) || HasTagInParents(hit.collider.transform, "Planet"))
            {
                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    closestCollider = hit.collider;
                }
            }
        }

        if (closestCollider == null)
        {
            return false;
        }

        HandleHit();
        return true;
    }

    private void FaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other.transform) || HasTagInParents(other.transform, "Planet"))
        {
            HandleHit();
        }
    }

    private bool IsPlayer(Transform target)
    {
        return HasTagInParents(target, "Player");
    }

    private bool HasTagInParents(Transform target, string tagName)
    {
        while (target != null)
        {
            if (target.CompareTag(tagName))
            {
                return true;
            }

            target = target.parent;
        }

        return false;
    }

    private void HandleHit()
    {
        if (_hasHit)
        {
            return;
        }

        _hasHit = true;
        Destroy(gameObject);
    }
}
