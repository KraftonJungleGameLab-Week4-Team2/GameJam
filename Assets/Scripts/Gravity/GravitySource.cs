using UnityEngine;

public class GravitySource : MonoBehaviour
{
    [field: SerializeField] public float GravityRange { get; private set; } = 10f;

    // public (Vector3 dir, float magnitude) GetGravity(Vector3 targetPos)
    // {
    //     var dir = (transform.position - targetPos).normalized;
    //     return (dir, GravityConstants.GravityAccel);
    // }
}
