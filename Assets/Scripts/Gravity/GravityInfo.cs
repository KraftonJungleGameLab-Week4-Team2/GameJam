using System;
using UnityEngine;

public interface IGravityInfo
{
    public Vector3 PlanetPos { get; }
    public Vector3 PlayerPos { get; }
    public float Gravity { get; }
}

[Serializable]
public class GravityInfo : IGravityInfo
{
    [field:SerializeField]
    public Vector3 PlanetPos { get; set; }
    [field:SerializeField]
    public Vector3 PlayerPos { get; set; }
    [field:SerializeField]
    public float Gravity { get; set; }
}
