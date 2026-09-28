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
        GravitySource tempOrigin = null;
        var maxDistance = float.MaxValue;
        foreach (var gs in _gravitySources)
        {
            var distance = (_player.position - gs.transform.position).magnitude;

            if (gs.GravityRange < distance)
                continue;

            if (maxDistance > distance)
            {
                maxDistance = distance;
                tempOrigin = gs;
            }
        }

        if (_gravityInfo.GravityOrigin != tempOrigin)
        {
            _gravityInfo.Gravity = 0.0f;
        }

        _gravityInfo.GravityOrigin = tempOrigin == null ? _gravityInfo.GravityOrigin : tempOrigin;
        if (_gravityInfo.GravityOrigin != null)
        {
            _gravityInfo.Gravity += GravityConstants.GravityAccel * Time.deltaTime;
        }
    }
}
