using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionEffect", menuName = "SurfaceSystem/Effects/Explosion Effect")]
public class ExplosionEffect : SurfaceEffect
{
    [Header("Fracture Effect")]
    [SerializeField, Min(0f)] private float _countdownSeconds = 3f;
    [SerializeField] private FractureEffect _fractureEffect;

    [Header("Flash")]
    [SerializeField, Min(0.1f)] private float _startFrequency = 1f;
    [SerializeField, Min(0.1f)] private float _endFrequency = 9f;
    [SerializeField, Min(1f)] private float _maxBrightness = 2.5f;

    [Header("Explosions")]
    [SerializeField, Min(0f)] private float _explosionForce = 40f;
    [SerializeField, Min(0.1f)] private float _explosionRadius = 20f;
    [SerializeField, Min(0f)] private float _upwardsModifier = 1f;

    public float CountdownSeconds { get { return _countdownSeconds; } }
    public FractureEffect FractureEffect { get { return _fractureEffect; } }

    // 점멸 주파수와 밝기 모두 진행도에 따라 증가한다.
    public float GetBrightnessMultiplier(float progress, float elapsed)
    {
        progress = Mathf.Clamp01(progress);
        float averageFrequency = Mathf.Lerp(_startFrequency, _endFrequency, progress * 0.5f);
        float pulse = 0.5f + 0.5f * Mathf.Sin(elapsed * averageFrequency * Mathf.PI * 2f);
        float strength = Mathf.Lerp(0.35f, Mathf.Max(0f, _maxBrightness - 1f), progress);
        return 1f + pulse * strength;
    }

    public override void OnEnter(SurfaceInstance surface, Collision collision)
    {
        StartCountdown(surface, collision != null ? collision.collider : null);
    }

    public override void OnSurfaceTriggerEnter(SurfaceInstance surface, Collider other)
    {
        StartCountdown(surface, other);
    }

    private void StartCountdown(SurfaceInstance surface, Collider other)
    {
        if (surface == null || other == null)
        {
            return;
        }

        // SurfaceMovement는 프로젝트 플레이어에 이미 붙어 있는 SurfaceSystem 컴포넌트다.
        if (other.GetComponentInParent<SurfaceMovement>() == null)
        {
            return;
        }

        if (_fractureEffect == null)
        {
            Debug.LogError("ExplosionEffect requires a FractureEffect asset.", surface);
            return;
        }

        MeshBomb bomb = surface.GetComponent<MeshBomb>();
        if (bomb == null)
        {
            bomb = surface.gameObject.AddComponent<MeshBomb>();
        }

        bomb.StartCountdown(this, other.attachedRigidbody);
    }

    public void Explode(MeshBomb bomb, Rigidbody playerBody)
    {
        if (bomb == null)
        {
            return;
        }

        Vector3 explosionCenter = bomb.transform.position;
        if (playerBody != null && !playerBody.isKinematic)
        {
            SurfaceMovement movement = playerBody.GetComponent<SurfaceMovement>();
            if (movement != null)
            {
                Vector3 outward = playerBody.worldCenterOfMass - explosionCenter;
                float distance = outward.magnitude;
                if (distance < _explosionRadius)
                {
                    outward = distance > 0.001f ? outward / distance : playerBody.transform.up;
                    float impulse = _explosionForce * (1f - distance / _explosionRadius);
                    movement.ApplyExternalVelocity(playerBody.linearVelocity + outward * (impulse / playerBody.mass));
                }
            }
            else
            {
                playerBody.AddExplosionForce(_explosionForce, explosionCenter, _explosionRadius, _upwardsModifier, ForceMode.Impulse);
            }
        }

        MeshGlass glass = bomb.GetComponent<MeshGlass>();
        if (glass == null)
        {
            Debug.LogError("ExplosionEffect requires MeshGlass on the bomb planet.", bomb);
            return;
        }

        // 행성 중심을 OpenFracture 파편의 발산점으로 사용한다.
        glass.BeginFracture(_fractureEffect, explosionCenter);
    }
}
