using UnityEngine;

public abstract class SurfaceEffect : ScriptableObject
{
    public virtual void OnEnter(SurfaceInstance surface, Collision collision) {}

    public virtual void OnStay(SurfaceInstance surface, Collision collision) {}

    public virtual void OnExit(SurfaceInstance surface, Collision collision) {}

    public virtual void OnImpact(SurfaceInstance surface, Collision collision) {}
}