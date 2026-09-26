using System;
using UnityEngine;

public interface IGravityInfo
{
    public Vector3 PlanetPos { get; }
    public Vector3 PlayerPos { get; }
    public float Gravity { get; }
}

public class GravityInfo : IGravityInfo
{
    public Vector3 PlanetPos { get; set; }
    public Vector3 PlayerPos { get; set; }
    public float Gravity { get; set; }
}
