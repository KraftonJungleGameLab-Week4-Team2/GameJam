using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class __TEST_PLAYER__ : MonoBehaviour
{
    private IGravityInfo _gravityInfo;
    private Rigidbody _rigidbody;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _gravityInfo = FindAnyObjectByType<GravityManager>().GravityInfo;
    }

    public void FixedUpdate()
    {
        var dir = (_gravityInfo.PlanetPos - transform.position).normalized;
        _rigidbody.AddForce(dir * _gravityInfo.Gravity, ForceMode.Force);
    }
}
