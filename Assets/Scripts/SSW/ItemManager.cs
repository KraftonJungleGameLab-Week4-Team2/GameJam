using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemManager : MonoBehaviour
{
    [FormerlySerializedAs("Planet Parent")]
    [SerializeField] private Transform _planetParent;
    [SerializeField] private Item _itemPrefab;
    [FormerlySerializedAs("Time")]
    [SerializeField, Min(0f)] private float _itemLifetime = 30f;
    [FormerlySerializedAs("Respawn")]
    [SerializeField, Min(0f)] private float _respawnDelay = 3f;

    private Item _spawnedItem;
    private Transform _itemOrbitTarget;
    private float _timer;
    private bool _isWaitingToRespawn;

    void Start()
    {
        SpawnAtRandomPlanet();
        _timer = _itemLifetime;
    }

    void Update()
    {
        if (_isWaitingToRespawn)
        {
            _timer -= Time.deltaTime;
            if (_timer > 0f)
            {
                return;
            }

            _isWaitingToRespawn = false;
            SpawnAtRandomPlanet();
            _timer = _itemLifetime;
            return;
        }

        if (_spawnedItem == null)
        {
            return;
        }

        _timer -= Time.deltaTime;
        if (_timer > 0f)
        {
            return;
        }

        Transform previousPlanet = _itemOrbitTarget;
        DespawnSpawnedItem();
        SpawnAtRandomPlanet(previousPlanet);
        _timer = _itemLifetime;
    }

    public Item SpawnAtRandomPlanet(Transform excludedPlanet = null)
    {
        List<Transform> planets = new List<Transform>();
        for (int i = 0; i < _planetParent.childCount; i++)
        {
            Transform planet = _planetParent.GetChild(i);
            if (planet.gameObject.activeInHierarchy)
            {
                planets.Add(planet);
            }
        }

        if (planets.Count > 1)
        {
            planets.Remove(excludedPlanet);
        }

        if (planets.Count == 0)
        {
            Debug.LogWarning("ItemManager found no active planets to spawn an item around.", this);
            return null;
        }

        Transform target = planets[Random.Range(0, planets.Count)];
        return SpawnItem(target);
    }

    private Item SpawnItem(Transform planet)
    {
        Item item = Instantiate(_itemPrefab, planet.position, Quaternion.identity);
        item.GetComponent<Revolution>().target = planet;
        item.Collected += HandleItemCollected;
        _spawnedItem = item;
        _itemOrbitTarget = planet;
        return item;
    }

    private void HandleItemCollected(Item collectedItem)
    {
        if (collectedItem != _spawnedItem)
        {
            return;
        }

        collectedItem.Collected -= HandleItemCollected;
        Destroy(collectedItem.gameObject);
        _spawnedItem = null;
        _itemOrbitTarget = null;

        _isWaitingToRespawn = true;
        _timer = _respawnDelay;
    }

    private void DespawnSpawnedItem()
    {
        _spawnedItem.Collected -= HandleItemCollected;
        Destroy(_spawnedItem.gameObject);
        _spawnedItem = null;
        _itemOrbitTarget = null;
    }
}
