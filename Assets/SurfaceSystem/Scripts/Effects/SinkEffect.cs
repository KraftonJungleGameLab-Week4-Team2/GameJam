using UnityEngine;

[CreateAssetMenu(fileName = "SinkEffect", menuName = "SurfaceSystem/Effects/Sink Effect")]
public class SinkEffect : SurfaceEffect
{
    [Header("Sinking")]
    [SerializeField, Min(0.01f)] private float _sinkSpeed = 0.45f;
    [SerializeField, Min(0.01f)] private float _maximumSinkDepth = 1.2f;
    [SerializeField, Range(0.05f, 1f)] private float _minimumColliderScale = 0.35f;
    [SerializeField, Min(0f)] private float _thresholdDepth = 0.9f;

    internal float SinkSpeed { get { return _sinkSpeed; } }
    internal float MaximumSinkDepth { get { return _maximumSinkDepth; } }
    internal float MinimumColliderScale { get { return _minimumColliderScale; } }
    internal float ThresholdDepth { get { return _thresholdDepth; } }

    public override void OnEnter(SurfaceInstance surface, Collision collision) => Begin(surface, collision?.collider);
    public override void OnSurfaceTriggerEnter(SurfaceInstance surface, Collider other) => Begin(surface, other);
    public override void OnStay(SurfaceInstance surface, Collision collision) => Begin(surface, collision?.collider);
    public override void OnSurfaceTriggerStay(SurfaceInstance surface, Collider other) => Begin(surface, other);
    public override void OnExit(SurfaceInstance surface, Collision collision) => End(surface, collision?.collider);
    public override void OnSurfaceTriggerExit(SurfaceInstance surface, Collider other) => End(surface, other);

    private void Begin(SurfaceInstance surface, Collider other)
    {
        if (surface == null || other == null)
        {
            return;
        }

        SwampSurfaceController controller = surface.GetComponent<SwampSurfaceController>();
        if (controller != null)
        {
            controller.Begin(this, other);
        }
    }

    private void End(SurfaceInstance surface, Collider other)
    {
        if (surface == null || other == null)
        {
            return;
        }

        SwampSurfaceController controller = surface.GetComponent<SwampSurfaceController>();
        if (controller != null)
        {
            controller.End(surface);
        }
    }
}
