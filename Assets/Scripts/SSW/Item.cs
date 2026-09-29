using UnityEngine;

[RequireComponent(typeof(Revolution))]
public class Item : MonoBehaviour
{
    private bool _isCollected;

    public event System.Action<Item> Collected;

    private void OnTriggerEnter(Collider other)
    {
        if (_isCollected || !IsPlayer(other.transform))
        {
            return;
        }

        _isCollected = true;
        Collected?.Invoke(this);
    }

    private bool IsPlayer(Transform target)
    {
        while (target != null)
        {
            if (target.CompareTag("Player"))
            {
                return true;
            }

            target = target.parent;
        }

        return false;
    }
}
