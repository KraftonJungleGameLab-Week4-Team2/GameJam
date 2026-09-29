using UnityEngine;

public class GravityManager : MonoBehaviour
{
    [SerializeField] private bool _findSourcsInScene = true;

    [SerializeField] private Transform _player;

    [SerializeField] private GravitySource[] _gravitySources;

    [SerializeField] private GravityInfo _gravityInfo;


    public IGravityInfo GravityInfo => _gravityInfo;


    private void Start()
    {
        if (_findSourcsInScene)
            _gravitySources = FindObjectsByType<GravitySource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    private void Update()
    {
        CalculateGravity();
    }

    private void CalculateGravity()
    {
        GravitySource closestSource = null;
        GravitySource origin = null;
        var minDistance = float.MaxValue;
        foreach (var gs in _gravitySources)
        {
            if (gs.IsActive == false)
                return;
            var distance = (_player.position - gs.transform.position).magnitude;

            if (minDistance > distance)
            {
                if (gs.GravityRange > distance)
                {
                    minDistance = distance;
                }
                closestSource = gs;
                origin = gs;
            }
        }

        if (_gravityInfo.GravityOrigin != origin)
        {
            _gravityInfo.Gravity = 0.0f;
        }

        _gravityInfo.GravityOrigin = origin == null ? closestSource : origin;
        if (_gravityInfo.GravityOrigin != null)
        {
            _gravityInfo.Gravity += GravityConstants.GravityAccel * Time.deltaTime;
        }
    }
}
