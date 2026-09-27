using UnityEngine;

public class GravityManager : MonoBehaviour
{
    [SerializeField] private bool _findSourcsInScene = true;

    [SerializeField] private Transform _player;

    [SerializeField] private GravitySource[] _gravitySources;

    [SerializeField] private GravityInfo _gravityInfo;

    private GravitySource _gravitySource;
    public IGravityInfo GravityInfo => _gravityInfo;

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
        var maxDistance = float.MaxValue;
        foreach (var gs in _gravitySources)
        {
            var sqrDistance = (_player.position - gs.transform.position).sqrMagnitude;

            if (gs.GravityRange * gs.GravityRange < sqrDistance)
                continue;

            if (maxDistance * maxDistance > sqrDistance)
            {
                maxDistance = sqrDistance;
                tempOrigin = gs;
            }
        }

        GravityOrigin = tempOrigin;

        if (GravityOrigin == null)
            _gravityInfo.Gravity = 0.0f;
        else
            _gravityInfo.Gravity += GravityConstants.GravityAccel * Time.deltaTime;
    }
}
