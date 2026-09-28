using System;
using UnityEngine;

[Serializable]
public struct MovementMultipliers
{
    [Header("Movement")]
    public float accelerationMultiplier;
    public float decelerationMultiplier;
    public float turningMultiplier;
    public float maxSpeedMultiplier;
    public float jumpMultiplier;

    public static MovementMultipliers Default
    {
        get
        {
            return new MovementMultipliers
            {
                accelerationMultiplier = 1f,
                decelerationMultiplier = 1f,
                turningMultiplier = 1f,
                maxSpeedMultiplier = 1f,
                jumpMultiplier = 1f
            };
        }
    }
}