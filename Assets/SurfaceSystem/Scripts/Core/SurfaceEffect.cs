using UnityEngine;

public abstract class SurfaceEffect : ScriptableObject
{
    public virtual void OnEnter(SurfaceInstance surface, Collision collision) {}

    public virtual void OnStay(SurfaceInstance surface, Collision collision) {}

    public virtual void OnExit(SurfaceInstance surface, Collision collision) {}

    public virtual void OnImpact(SurfaceInstance surface, Collision collision) {}

    public virtual void OnSurfaceTriggerEnter(SurfaceInstance surface, Collider other) {}

    public virtual void OnSurfaceTriggerStay(SurfaceInstance surface, Collider other) {}

    public virtual void OnSurfaceTriggerExit(SurfaceInstance surface, Collider other) {}

    public virtual void OnSurfaceTriggerImpact(SurfaceInstance surface, Collider other) {}

    public virtual void OnSinkThresholdReached(SurfaceInstance surface, Rigidbody body, Vector3 outward) {}
}
