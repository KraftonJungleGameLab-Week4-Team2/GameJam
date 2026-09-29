using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum MaterialType
{
    None,
    Ice,
    Jelly,
    Bomb,
    Swamp,
    Glass,
    Fire,
}

public class ScreenEffectManager : MonoBehaviour
{
    [SerializeField] private Volume _volume;
    private Vignette _vignette;

    [SerializeField] private float _buildUpSpeed = 1f;
    [SerializeField] private float _decaySpeed = 2f;
    [SerializeField] private float _maxIntensity = 0.6f;

    [Header("Use")]
    public bool IsUse = false;

    [Header("Color")]
    [SerializeField] private Color _iceColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color _jellyColor = new Color(0.2f, 1f, 0.4f);
    [SerializeField] private Color _bombColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color _swampColor = new Color(0.4f, 0.25f, 0.1f);
    [SerializeField] private Color _glassColor = new Color(0.8f, 0.9f, 1f);
    [SerializeField] private Color _fireColor;

    [Header("Binding")]
    [SerializeField] private PlayerMovement _playerMovement;

    [Header("InGame")]
    [SerializeField] private MaterialType _currentMaterialType = MaterialType.None;
    [SerializeField] private float _effectProgress = 0f;

    [Header("Test / with ContextMenu")]
    [SerializeField] private MaterialType _testMaterialType = MaterialType.None;


    void Start()
    {
        _volume.profile.TryGet<Vignette>(out _vignette);

        _playerMovement.PlanetChanged += (GravitySource gravitySource) =>
        {
            //SetEffect(gravitySource.MaterialType);
        };
    }

    [ContextMenu("TestSetEffect")]
    public void TestSetEffect()
    {
        SetEffect(_testMaterialType);
    }

    public void SetEffect(MaterialType type)
    {
        if (_currentMaterialType == type)
        {
            return;
        }

        _effectProgress = 0f;
        _currentMaterialType = type;

        if (_vignette != null)
        {
            switch (_currentMaterialType)
            {
                case MaterialType.Ice:
                    _vignette.color.value = _iceColor;
                    break;
                case MaterialType.Jelly:
                    _vignette.color.value = _jellyColor;
                    break;
                case MaterialType.Bomb:
                    _vignette.color.value = _bombColor;
                    break;
                case MaterialType.Swamp:
                    _vignette.color.value = _swampColor;
                    break;
                case MaterialType.Glass:
                    _vignette.color.value = _glassColor;
                    break;
                case MaterialType.None:
                    break;
            }
        }
    }

    public void Update()
    {
        if (IsUse == false)
        {
            return;
        }

        // 일반 행성
        if (_currentMaterialType == MaterialType.None)
        {
            _effectProgress = Mathf.MoveTowards(_effectProgress, 0f, _decaySpeed * Time.deltaTime);
        }
        else
        {
            // 효과 있는 행성
            _effectProgress = Mathf.MoveTowards(_effectProgress, 1f, _buildUpSpeed * Time.deltaTime);
        }

        // 효과 적용
        if (_vignette != null)
        {
            _vignette.intensity.value = _effectProgress * _maxIntensity;
        }
    }
}
