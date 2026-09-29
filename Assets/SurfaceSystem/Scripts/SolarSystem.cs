using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    [Header("Planets")]
    [SerializeField] private GameObject _centralFirePlanet;
    [SerializeField] private GameObject _normalPlanetPrefab;
    [SerializeField] private GameObject[] _specialPlanetPrefabs;

    [Header("Orbit")]
    [SerializeField, Min(2)] private int _orbitingPlanetCount = 8;
    [SerializeField, Min(0f)] private float _orbitRadius = 40f;
    [SerializeField] private float _orbitSpeed = 0.05f;

    [Header("Scene References")]
    [SerializeField] private GravityManager _gravityManager;
    [SerializeField] private BossAttack _bossAttack;

    private GameObject[] _orbitingPlanets;
    private GameObject[] _orbitingPrefabs;

    private void Awake()
    {
        _orbitingPlanets = new GameObject[_orbitingPlanetCount];
        _orbitingPrefabs = new GameObject[_orbitingPlanetCount];

        // Slot 0 always uses Planet_Normal, so its generated name is always Planet_Normal_1.
        float firstAngle = Mathf.PI * 2f / _orbitingPlanetCount;
        SpawnPlanet(0, _normalPlanetPrefab, firstAngle);

        int secondNormalSlot = Random.Range(1, _orbitingPlanetCount);
        for (int i = 1; i < _orbitingPlanetCount; i++)
        {
            GameObject prefab = i == secondNormalSlot ? _normalPlanetPrefab : PickRandomPrefab();
            float angle = (i + 1) * Mathf.PI * 2f / _orbitingPlanetCount;
            SpawnPlanet(i, prefab, angle);
        }

        RefreshReferences();
    }

    private GameObject PickRandomPrefab()
    {
        int choice = Random.Range(0, _specialPlanetPrefabs.Length + 1);
        return choice == 0 ? _normalPlanetPrefab : _specialPlanetPrefabs[choice - 1];
    }

    private void SpawnPlanet(int slot, GameObject prefab, float angle)
    {
        Vector3 center = _centralFirePlanet.transform.position;
        Vector3 position = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * _orbitRadius;
        GameObject planet = Instantiate(prefab, position, Quaternion.identity, transform);
        planet.name = prefab.name + "_" + (slot + 1);

        Rigidbody body = planet.GetComponent<Rigidbody>();
        body.isKinematic = false;
        Revolution revolution = planet.AddComponent<Revolution>();
        revolution.Configure(_centralFirePlanet.transform, _orbitRadius, angle, _orbitSpeed);

        _orbitingPlanets[slot] = planet;
        _orbitingPrefabs[slot] = prefab;
        MeshGlass glass = planet.GetComponent<MeshGlass>();
        glass.RestoreRequested += () => ReplacePlanet(slot);
    }

    private void ReplacePlanet(int slot)
    {
        GameObject oldPlanet = _orbitingPlanets[slot];
        float angle = oldPlanet.GetComponent<Revolution>().CurrentAngle;
        int otherNormalPlanets = 0;
        for (int i = 0; i < _orbitingPrefabs.Length; i++)
        {
            if (i != slot && _orbitingPrefabs[i] == _normalPlanetPrefab)
            {
                otherNormalPlanets++;
            }
        }

        GameObject nextPrefab = otherNormalPlanets < 2 ? _normalPlanetPrefab : PickRandomPrefab();
        SpawnPlanet(slot, nextPrefab, angle);
        Destroy(oldPlanet);
        RefreshReferences();
    }

    private void RefreshReferences()
    {
        Transform[] targets = new Transform[_orbitingPlanets.Length + 1];
        GravitySource[] gravitySources = new GravitySource[targets.Length];
        targets[0] = _centralFirePlanet.transform;
        gravitySources[0] = _centralFirePlanet.GetComponent<GravitySource>();

        for (int i = 0; i < _orbitingPlanets.Length; i++)
        {
            targets[i + 1] = _orbitingPlanets[i].transform;
            gravitySources[i + 1] = _orbitingPlanets[i].GetComponent<GravitySource>();
        }

        _gravityManager.SetGravitySources(gravitySources);
        _bossAttack.SetTargetPlanets(targets);
    }
}
