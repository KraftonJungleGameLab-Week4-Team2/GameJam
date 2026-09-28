using System;
using UnityEngine;

public interface IGravityInfo
{
    public event Action<GravitySource> GravityOriginChanged;
    public GravitySource GravityOrigin { get; }
    public float Gravity { get; }
    public void ApplyGravity(float gravity);
}

[Serializable]
public class GravityInfo : IGravityInfo
{
    public event Action<GravitySource> GravityOriginChanged;

    private GravitySource _gravityOrigin;
    public GravitySource GravityOrigin
    {
        get => _gravityOrigin;
        set
        {
            if(value != _gravityOrigin)
                GravityOriginChanged?.Invoke(value);

            _gravityOrigin = value;
        }
    }
    [field: SerializeField] public float Gravity { get; set; }

    public void ApplyGravity(float gravity)
    {
        Gravity = gravity;
    }
}
