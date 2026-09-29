using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private float _orbitalRadius;

    [SerializeField] private Item _itemPrefab;

    public Item SpawnItem(Transform planet)
    {
        Vector3 spawnPosition = planet.position + Vector3.right * _orbitalRadius;
        Item item = Instantiate(_itemPrefab, spawnPosition, Quaternion.identity);

        Revolution orbit = item.GetComponent<Revolution>();

        orbit.target = planet;
        return item;
    }
}
