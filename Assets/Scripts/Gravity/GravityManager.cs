using UnityEngine;

public class GravityManager : MonoBehaviour
{
    [SerializeField] private bool _findSourcsInScene = true;

    [SerializeField] private Transform _player;

    [SerializeField] private GravitySource[] _gravitySources;

    [SerializeField] private GravityInfo _gravityInfo;

    private GravitySource _gravitySource;

    private GravitySource GravityOrigin
    {
        get => _gravitySource;
        set
        {
            if (_gravitySource != value) _gravityInfo.Gravity = 0.0f;
            _gravitySource = value;
        }
    }

    private void Start()
    {
        if (_findSourcsInScene)
            _gravitySources = FindObjectsByType<GravitySource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    private void Update()
    {
        CalculateGravity();
        if (GravityOrigin != null) _gravityInfo.PlanetPos = GravityOrigin.transform.position;
    }

    private void CalculateGravity()
    {
        GravitySource tempOrigin = null;

        var maxDistance = 0.0f;
        foreach (var gs in _gravitySources)
        {
            var distance = Vector3.Distance(_player.position, gs.transform.position);
            if (maxDistance < distance)
            {
                maxDistance = distance;
                tempOrigin = gs;
            }
        }

        GravityOrigin = tempOrigin;
        _gravityInfo.Gravity += GravityConstants.GravityAccel * Time.deltaTime;
    }
}
