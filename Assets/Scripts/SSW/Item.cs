using UnityEngine;

public class Item : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        gameObject.SetActive(false);
    }
}
