using UnityEngine;

public class GravitySource : MonoBehaviour
{
    [field: SerializeField] public MaterialType MaterialType { get; private set; }
    [field: SerializeField] public float GravityRange { get; set; } = 10f;
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

    private GravityRangeVisual _gravityRangeVisual;
    private void Awake()
    {
        _meshGlass = GetComponent<MeshGlass>();
        _gravityRangeVisual = GetComponentInChildren<GravityRangeVisual>();
    }

    public void SetRange(float range)
    {
        _gravityRangeVisual?.SetRange(range);
    }

    public void SetHighlight(bool isHighlighted)
    {
        _gravityRangeVisual?.SetHighlight(isHighlighted);
    }
}
