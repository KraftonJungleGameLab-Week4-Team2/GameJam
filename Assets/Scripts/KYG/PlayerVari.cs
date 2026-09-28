using System;
using UnityEngine;

[Serializable]
public class PlayerVari
{
    [Header("StateCheck")]
    public float airMultiplier;
    [Range(0f, 1000f)]

    [Header("Force")]
    public float moveSpeed = 5;
    public float jumpForce = 10;
    public float stompForce = 10;

    public float xDir;
    public Vector3 moveDir;
}
