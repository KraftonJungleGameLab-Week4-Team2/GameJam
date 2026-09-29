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
}

public class ScreenEffectManager : MonoBehaviour
{
    [SerializeField] private Volume _volume;
    private Vignette _vignette;

    [SerializeField] private float _buildUpSpeed = 1f;
    [SerializeField] private float _decaySpeed = 2f;
    [SerializeField] private float _maxIntensity = 0.6f;

    [Header("Color")]
    [SerializeField] private Color _iceColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color _jellyColor = new Color(0.2f, 1f, 0.4f);
    [SerializeField] private Color _bombColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color _swampColor = new Color(0.4f, 0.25f, 0.1f);
    [SerializeField] private Color _glassColor = new Color(0.8f, 0.9f, 1f);

    [Header("InGame")]
    [SerializeField] private MaterialType _currentMaterialType = MaterialType.None;
    [SerializeField] private float _effectProgress = 0f;

    [Header("Test / with ContextMenu")]
    [SerializeField] private MaterialType _testMaterialType = MaterialType.None;


    void Start()
    {
        if (_volume.profile.TryGet<Vignette>(out _vignette))
        {
            _vignette.intensity.value = 0f;
        }
    }

    [ContextMenu("TestSetEffect")]
    public void TestSetEffect()
    {
        SetEffect(_testMaterialType);
    }

    public void SetEffect(MaterialType type)
    {
        // 이미 같은 머티리얼 위에 있다면 중복으로 색상을 바꿀 필요가 없음
        if (_currentMaterialType == type)
        {
            return;
        }

        // 머터리얼 바뀌면 진행 초기화
        _effectProgress = 0f;
        _currentMaterialType = type;

        // 머티리얼 타입에 따라 비네트 색상 미리 변경하기
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
                    // None일 때는 색상을 굳이 바꿀 필요 없음 (어차피 Intensity가 0이 됨)
                    break;
            }
        }
    }

    public void Update()
    {
        // 1. 머티리얼이 None이면 효과 감소
        if (_currentMaterialType == MaterialType.None)
        {
            _effectProgress = Mathf.MoveTowards(_effectProgress, 0f, _decaySpeed * Time.deltaTime);
        }
        else
        {
            // 2. 무언가 밟고 있으면 효과 증가
            _effectProgress = Mathf.MoveTowards(_effectProgress, 1f, _buildUpSpeed * Time.deltaTime);
        }

        // 3. 강도(Intensity)만 실시간으로 반영
        if (_vignette != null)
        {
            _vignette.intensity.value = _effectProgress * _maxIntensity;
        }
    }
}
