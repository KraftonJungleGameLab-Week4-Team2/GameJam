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

    public void ProcessTriggerEnter(SurfaceInstance surface, Collider other)
    {
        foreach (SurfaceEffect effect in _effects)
        {
            if (effect != null)
            {
                effect.OnTriggerEnter(surface, other);
            }
        }
    }

    public void ProcessTriggerStay(SurfaceInstance surface, Collider other)
    {
        foreach (SurfaceEffect effect in _effects)
        {
            if (effect != null)
            {
                effect.OnTriggerStay(surface, other);
            }
        }
    }

    public void ProcessTriggerExit(SurfaceInstance surface, Collider other)
    {
        foreach (SurfaceEffect effect in _effects)
        {
            if (effect != null)
            {
                effect.OnTriggerExit(surface, other);
            }
        }
    }

    public void ProcessTriggerImpact(SurfaceInstance surface, Collider other)
    {
        foreach (SurfaceEffect effect in _effects)
        {
            if (effect != null)
            {
                effect.OnTriggerImpact(surface, other);
            }
        }
    }
}
