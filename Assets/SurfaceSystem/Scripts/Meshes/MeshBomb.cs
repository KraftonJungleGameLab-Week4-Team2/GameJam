using System.Collections;

using UnityEngine;

[RequireComponent(typeof(MeshGlass), typeof(SurfaceInstance))]
public class MeshBomb : MonoBehaviour
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private bool _countdownStarted;
    private bool _detonated;
    private Renderer _flashRenderer;
    private MaterialPropertyBlock _propertyBlock;
    private int _colorPropertyId;
    private Color _originalColor;
    private bool _canFlash;

    public bool CountdownStarted { get { return _countdownStarted; } }
    public bool Detonated { get { return _detonated; } }

    // 타이머 상태는 각 행성이 보유해 공유 ExplosionEffect 에셋끼리 섞이지 않게 한다.
    public void StartCountdown(ExplosionEffect effect, Rigidbody playerBody)
    {
        if (_countdownStarted || _detonated)
        {
            return;
        }

        if (effect == null || effect.FractureEffect == null)
        {
            Debug.LogError("MeshBomb requires an ExplosionEffect with a FractureEffect to detonate.", this);
            return;
        }

        _countdownStarted = true;
        StartCoroutine(DetonateAfterDelay(effect, playerBody));
    }

    private IEnumerator DetonateAfterDelay(ExplosionEffect effect, Rigidbody playerBody)
    {
        float duration = Mathf.Max(0f, effect.CountdownSeconds);
        float elapsed = 0f;
        PrepareFlash();

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            ApplyFlash(effect.GetBrightnessMultiplier(progress, elapsed));
            yield return null;
            elapsed += Time.deltaTime;
        }

        RestoreFlash();
        if (!isActiveAndEnabled)
        {
            yield break;
        }

        _detonated = true;
        effect.Explode(this, playerBody);
    }

    private void PrepareFlash()
    {
        _flashRenderer = GetComponent<Renderer>();
        if (_flashRenderer == null || _flashRenderer.sharedMaterial == null)
        {
            return;
        }

        Material material = _flashRenderer.sharedMaterial;
        if (material.HasProperty(BaseColorId))
        {
            _colorPropertyId = BaseColorId;
        }
        else if (material.HasProperty(ColorId))
        {
            _colorPropertyId = ColorId;
        }
        else
        {
            Debug.LogWarning("Planet material has no _BaseColor or _Color property; countdown flashing is disabled.", this);
            return;
        }

        _originalColor = material.GetColor(_colorPropertyId);
        _propertyBlock = new MaterialPropertyBlock();
        _flashRenderer.GetPropertyBlock(_propertyBlock);
        _canFlash = true;
    }

    private void ApplyFlash(float brightness)
    {
        if (!_canFlash || _flashRenderer == null)
        {
            return;
        }

        Color color = _originalColor * brightness;
        color.a = _originalColor.a;
        _propertyBlock.SetColor(_colorPropertyId, color);
        _flashRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void RestoreFlash()
    {
        if (!_canFlash || _flashRenderer == null)
        {
            return;
        }

        _propertyBlock.SetColor(_colorPropertyId, _originalColor);
        _flashRenderer.SetPropertyBlock(_propertyBlock);
        _canFlash = false;
    }

    private void OnDisable()
    {
        RestoreFlash();
        if (!_detonated)
        {
            _countdownStarted = false;
        }
    }
}
