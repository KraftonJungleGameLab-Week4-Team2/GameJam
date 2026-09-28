using UnityEngine;

public abstract class SurfaceEffect : ScriptableObject
{
    public virtual void OnEnter(SurfaceInstance surface, Collision collision) {}

    public virtual void OnStay(SurfaceInstance surface, Collision collision) {}

    public virtual void OnExit(SurfaceInstance surface, Collision collision) {}

    public virtual void OnImpact(SurfaceInstance surface, Collision collision) {}

    public virtual void OnTriggerEnter(SurfaceInstance surface, Collider other) {}

    public virtual void OnTriggerStay(SurfaceInstance surface, Collider other) {}

    public virtual void OnTriggerExit(SurfaceInstance surface, Collider other) {}

    public virtual void OnTriggerImpact(SurfaceInstance surface, Collider other) {}
}
