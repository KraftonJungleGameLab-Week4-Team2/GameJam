using UnityEngine;

public class GravityRangeVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer _renderer;

    [SerializeField] private Color _normalColor = new Color(203 / 255f, 1f, 1f, 70 / 255f);
    [SerializeField] private Color _highlightColor = new Color(203 / 255f, 1f, 1f, 1f);

    private Material material;

    void Start()
    {
        material = _renderer.material;
    }

    void OnDestroy()
    {
        if (material != null)
        {
            Destroy(material);
        }
    }

    public void SetHighlight(bool isHighlight)
    {
        if (isHighlight)
        {
            material.SetColor("_Color", _highlightColor);
        }
        else
        {
            material.SetColor("_Color", _normalColor);
        }
    }

    public void SetRange(float radius)
    {
        material.SetFloat("_Radius", radius);
    }
}
