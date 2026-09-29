using System;
using UnityEngine;

public class GravitySource : MonoBehaviour
{
    [field: SerializeField] public float GravityRange { get; private set; } = 10f;
    public bool IsBroken => !_meshGlass == null || _meshGlass.IsBroken;

    private MeshGlass _meshGlass;
    private void Awake()
    {
        _meshGlass = GetComponent<MeshGlass>();
    }
}
