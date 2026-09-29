using DG.Tweening;
using UnityEngine;

public enum PlanetState
{
    None,
    Spawn,
    Idle,
    Break
}

public class Planet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshGlass _meshGlass;

    [Header("Settings")]
    [SerializeField] private MaterialType _materialType;

    [Header("InGame")]
    [SerializeField] private Vector3 _originScale;
    [SerializeField] private PlanetState _planetState;

    public PlanetState PlanetState
    {
        get => _planetState;
        set => _planetState = value;
    }

    void Start()
    {
        _originScale = transform.localScale;

        if (_meshGlass != null)
        {
            _meshGlass.OnMeshBroken += () =>
            {
                _planetState = PlanetState.Break;
            };
        }
    }

    public void Show()
    {
        _planetState = PlanetState.Spawn;

        transform.localScale = Vector3.zero;
        transform.DOScale(_originScale, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            transform.DOPunchScale(Vector3.one * 2f, 0.5f).OnComplete(() =>
            {
                transform.localScale = _originScale;
                _planetState = PlanetState.Idle;
            });
        });
    }
}
