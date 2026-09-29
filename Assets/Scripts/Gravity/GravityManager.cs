using UnityEngine;

public class GravityManager : MonoBehaviour
{
    [SerializeField] private Transform _player;

    [SerializeField] private GravityInfo _gravityInfo;

    [SerializeField] private GravitySource[] _gravitySources;

    public IGravityInfo GravityInfo => _gravityInfo;


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
            if (gs.IsBroken == true)
            {
                gs.SetRange(1f);
                gs.SetHighlight(false);
                continue;
            }

            gs.SetHighlight(false);
            gs.SetRange(gs.GravityRange);

            var distance = (_player.position - gs.transform.position).magnitude;

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
            _gravityInfo.GravityOrigin.SetHighlight(true);
            _gravityInfo.Gravity += GravityConstants.GravityAccel * Time.deltaTime;
        }
    }
}
