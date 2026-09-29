using UnityEngine;

[RequireComponent(typeof(Revolution))]
public class Item : MonoBehaviour
{
    private bool _isCollected;

    public event System.Action<Item> Collected;

    private void OnTriggerEnter(Collider other)
    {
        if (_isCollected || !other.transform.root.CompareTag("Player"))
        {
            return;
        }

        _isCollected = true;
        Collected?.Invoke(this);
    }
}
