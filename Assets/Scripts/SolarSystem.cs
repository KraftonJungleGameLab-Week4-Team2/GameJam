using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform _spawnParentTr;
    [SerializeField] private Transform _centerTr;
    [SerializeField] private int _spawnCount = 6;
    [SerializeField] private float _spawnRadius = 6f;
    [Space]
    [SerializeField] private float _rotationSpeed = 20f;
    [SerializeField] private float _respawnCooldown = 3.0f;

    [Header("Prefabs")]
    [SerializeField] private PlanetSocket _socketPrefab;
    [SerializeField] private Planet _centerPrefab;
    [SerializeField] private Planet[] _planetPrefabs;

    // 생성된 행성 배열
    private Planet spawnedCenter;
    private float centerRespawnTimer = 0f;// 중심 오브젝트 리스폰 쿨타임 타이머

    private Planet[] spawnedPlanets;
    private float[] respawnTimers;

    void Start()
    {
        spawnedPlanets = new Planet[_spawnCount];
        respawnTimers = new float[_spawnCount];

        SetupInitialPlanets();
    }

    void Update()
    {
        // 관람차처럼 회전
        for (int i = 0; i < spawnedPlanets.Length; i++)
        {
            if (spawnedPlanets[i].PlanetState == PlanetState.Idle)
            {
                transform.RotateAround(_centerTr.position, Vector3.up, _rotationSpeed * Time.deltaTime);
            }
        }

        // 리스폰 쿨타임 체크
        CheckAndRespawnEmptySlots();
    }

    void SetupInitialPlanets()
    {
        // 중심 행성
        SpawnCenterObject();

        // 도는 행성
        float angleStep = 360f / _spawnCount;
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 spawnPos = CalculateSpawnPosition(i, angleStep);
            SpawnPlanetAt(i, spawnPos);
        }
    }

    void SpawnPlanetAt(int index, Vector3 spawnPos)
    {
        Planet selectedPrefab = GetRandomPlanetPrefab();

        if (selectedPrefab != null)
        {
            Planet spawnedObj = Instantiate(selectedPrefab, spawnPos, Quaternion.identity, _spawnParentTr);
            spawnedPlanets[index] = spawnedObj;
        }
    }

    void CheckAndRespawnEmptySlots()
    {
        // 중심 행성
        if (spawnedCenter == null)
        {
            centerRespawnTimer += Time.deltaTime;

            if (centerRespawnTimer >= _respawnCooldown)
            {
                centerRespawnTimer = 0f;
                SpawnCenterObject();
            }
        }

        // 도는 행성
        float angleStep = 360f / _spawnCount;
        for (int i = 0; i < _spawnCount; i++)
        {
            if (spawnedPlanets[i] == null)
            {
                respawnTimers[i] += Time.deltaTime;

                if (respawnTimers[i] >= _respawnCooldown)
                {
                    respawnTimers[i] = 0f;

                    Vector3 spawnPos = CalculateSpawnPosition(i, angleStep);
                    SpawnPlanetAt(i, spawnPos);
                }
            }
        }
    }

    void SpawnCenterObject()
    {
        spawnedCenter = Instantiate(_centerPrefab, _centerTr.position, _centerTr.rotation, _spawnParentTr);
    }

    Planet GetRandomPlanetPrefab()
    {
        int randomIndex = Random.Range(0, _planetPrefabs.Length);
        return _planetPrefabs[randomIndex];
    }

    Vector3 CalculateSpawnPosition(int index, float angleStep)
    {
        float currentAngle = index * angleStep;
        float radian = currentAngle * Mathf.Deg2Rad;

        float x = _centerTr.position.x + Mathf.Cos(radian) * _spawnRadius;
        float y = _centerTr.position.y + Mathf.Sin(radian) * _spawnRadius;
        return new Vector3(x, y, _centerTr.position.z);
    }
}
