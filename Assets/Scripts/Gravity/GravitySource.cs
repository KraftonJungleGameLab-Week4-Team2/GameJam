using UnityEngine;

public class GravitySource : MonoBehaviour
{
    [field: SerializeField] public float GravityRange { get; private set; } = 10f;
}
