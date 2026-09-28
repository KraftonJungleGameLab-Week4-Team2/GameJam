using UnityEngine;

public class SurfaceMovement : MonoBehaviour
{
    private SurfaceInstance _source;

    public MovementMultipliers Current { get; private set; } = MovementMultipliers.Default;

    public void Apply(SurfaceInstance source, MovementMultipliers modifiers)
    {
        _source = source;
        Current = modifiers;
    }

    public void Clear(SurfaceInstance source)
    {
        if (_source != source)
        {
            return;
        }

        _source = null;
        Current = MovementMultipliers.Default;
    }

    private void OnDisable()
    {
        _source = null;
        Current = MovementMultipliers.Default;
    }
}