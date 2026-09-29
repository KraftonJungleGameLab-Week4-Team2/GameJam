using UnityEngine;

public class GravitySource : MonoBehaviour
{
    [field: SerializeField] public float GravityRange { get; private set; } = 10f;
    public bool IsBroken
    {
        get
        {
            if (_meshGlass == null)
                return false;

            return _meshGlass.IsBroken;
        }
    }


    private MeshGlass _meshGlass;
    private void Awake()
    {
        _meshGlass = GetComponent<MeshGlass>();
    }
}
