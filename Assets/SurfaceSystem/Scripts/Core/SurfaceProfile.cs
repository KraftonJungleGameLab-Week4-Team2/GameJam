using UnityEngine;

[CreateAssetMenu(fileName = "SurfaceProfile", menuName = "SurfaceSystem/Surface Profile")]
public class SurfaceProfile : ScriptableObject
{
    [Header("Surface Settings")]
    [SerializeField] private PhysicsMaterial _physicsMaterial;
    [SerializeField] private SurfaceEffect[] _effects = new SurfaceEffect[0];

    public PhysicsMaterial PhysicsMaterial
    {
        get { return _physicsMaterial; }
    }

    public void ProcessEnter(SurfaceInstance surface, Collision collision)
    {
        foreach (SurfaceEffect effect in _effects)
        {
            if (effect != null)
            {
                effect.OnEnter(surface, collision);
            }
        }
    }

    public void ProcessStay(SurfaceInstance surface, Collision collision)
    {
        foreach (SurfaceEffect effect in _effects)
        {
            if (effect != null)
            {
                effect.OnStay(surface, collision);
            }
        }
    }

    public void ProcessExit(SurfaceInstance surface, Collision collision)
    {
        foreach (SurfaceEffect effect in _effects)
        {
            if (effect != null)
            {
                effect.OnExit(surface, collision);
            }
        }
    }

    public void ProcessImpact(SurfaceInstance surface, Collision collision)
    {
        foreach (SurfaceEffect effect in _effects)
        {
            if (effect != null)
            {
                effect.OnImpact(surface, collision);
            }
        }
    }

    public T GetEffect<T>() where T : SurfaceEffect
    {
        foreach (SurfaceEffect effect in _effects)
        {
            if (effect is T typedEffect)
            {
                return typedEffect;
            }
        }
        return null;
    }

    public void ProcessSinkThresholdReached(SurfaceInstance surface, Rigidbody body, Vector3 outward)
    {
        foreach (SurfaceEffect effect in _effects)
        {
            if (effect != null)
            {
                effect.OnSinkThresholdReached(surface, body, outward);
            }
        }
    }

    public void ProcessSurfaceTriggerEnter(SurfaceInstance surface, Collider other)
    {
        foreach (SurfaceEffect effect in _effects)
        {
            if (effect != null)
            {
                effect.OnSurfaceTriggerEnter(surface, other);
            }
        }
    }

    public void ProcessSurfaceTriggerStay(SurfaceInstance surface, Collider other)
    {
        foreach (SurfaceEffect effect in _effects)
        {
            if (effect != null)
            {
                effect.OnSurfaceTriggerStay(surface, other);
            }
        }
    }

    public void ProcessSurfaceTriggerExit(SurfaceInstance surface, Collider other)
    {
        foreach (SurfaceEffect effect in _effects)
        {
            if (effect != null)
            {
                effect.OnSurfaceTriggerExit(surface, other);
            }
        }
    }

    public void ProcessSurfaceTriggerImpact(SurfaceInstance surface, Collider other)
    {
        foreach (SurfaceEffect effect in _effects)
        {
            if (effect != null)
            {
                effect.OnSurfaceTriggerImpact(surface, other);
            }
        }
    }
}
