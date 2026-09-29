using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public GameObject attackCube;
    [SerializeField] private Transform _playerPos;
    [SerializeField] private GameObject _fakeFireBall;
    [Tooltip("Fake Meteor 생성 기준점입니다. Boss 프리팹은 LeftLeg/FootStem으로 설정되어 있습니다.")]
    [SerializeField] private Transform _fakeMeteorSpawnPoint;
    [Tooltip("FootStem에서 이동할 거리입니다. Boss 방향에 맞춰 회전하며, 보스 크기 배율은 적용하지 않습니다.")]
    [SerializeField] private Vector3 _fakeMeteorSpawnOffset;
    public Vector3 playerRightUp;
    public bool nomalattack;
    public bool fireBallAttack;
    [SerializeField] private float _meteorCooldown = 5f;
    public Transform boss;
    private Vector3 _bossRight;
    [SerializeField] private Transform[] _planetAry;
    [SerializeField] private GameObject _WarningPrefab;
    [SerializeField] private GameObject _fireBallPrefab;

    private void Start()
    {
        nomalattack = true;
        fireBallAttack = true;

    }
    void Update()
    {
        if (nomalattack)
        {
            StartCoroutine(CallNomal()); //4초마다 발사 하기 위한 코루틴

        }
        if (fireBallAttack)
        {
            StartCoroutine(CallMeteorAttack());
        }

    }
    IEnumerator CallNomal()
    {
        NomalAttack();
        nomalattack = false;

        yield return new WaitForSeconds(4f); //4초마다 발사
        nomalattack = true;
    }
    private void NomalAttack()
    {
        Debug.Log("발사");

        //발사체 생성 위치
        _bossRight = boss.position;

        //플레이어 위치 저장
        Vector3 playerPoint = _playerPos.position;

        //랜덤위치 생성을 위한 배열
        Vector3[] waypoint = { playerPoint + new Vector3(13, 15, 0), playerPoint + new Vector3(-13, -15, 0), playerPoint + new Vector3(-13, 15, 0), playerPoint + new Vector3(13, -15, 0) };

        Transform playerNow = _playerPos;
        //Vector3 waypoint = playerNow + new Vector3(13, 15, 0);
        int randomIndex = Random.Range(0, 4);

        GameObject obj = Instantiate(attackCube, _bossRight, Quaternion.identity); //발사체를 저장하여 함수부여

        obj.GetComponent<Bullet>().Init(waypoint[randomIndex], playerNow);

    }
    private IEnumerator CallMeteorAttack()
    {
        fireBallAttack = false;

        Transform targetPlanet = GetRandomAvailablePlanet();
        while (targetPlanet == null)
        {
            yield return new WaitForSeconds(0.5f);
            targetPlanet = GetRandomAvailablePlanet();
        }

        yield return PlayFakeFireBall();
        yield return new WaitForSeconds(3f);

        GameObject warning = Instantiate(_WarningPrefab, targetPlanet.position, Quaternion.identity, targetPlanet);
        yield return new WaitForSeconds(3f);
        Destroy(warning);

        GameObject meteor = Instantiate(_fireBallPrefab, GetMeteorSpawnPosition(targetPlanet), Quaternion.identity);
        meteor.GetComponent<Meteor>().Launch(targetPlanet.position);

        yield return new WaitForSeconds(_meteorCooldown);
        fireBallAttack = true;
    }

    private Transform GetRandomAvailablePlanet()
    {
        List<Transform> availablePlanets = new List<Transform>();

        foreach (Transform planet in _planetAry)
        {
            MeshGlass glass = planet.GetComponent<MeshGlass>();
            if (!glass.IsBroken && !glass.IsFracturing)
            {
                availablePlanets.Add(planet);
            }
        }

        if (availablePlanets.Count == 0)
        {
            return null;
        }

        return availablePlanets[Random.Range(0, availablePlanets.Count)];
    }

    private IEnumerator PlayFakeFireBall()
    {
        Vector3 spawnPosition = GetFakeMeteorSpawnPosition();
        GameObject fakeMeteor = Instantiate(_fakeFireBall, spawnPosition, Quaternion.identity);
        Rigidbody fakeMeteorBody = fakeMeteor.GetComponent<Rigidbody>();
        fakeMeteorBody.isKinematic = true;
        fakeMeteorBody.useGravity = false;
        fakeMeteor.GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(0.75f);

        Vector3 launchOffset = new Vector3(0f, 100f, -300f);
        yield return fakeMeteor.transform.DOMove(spawnPosition + launchOffset, 1.5f).SetEase(Ease.Linear).WaitForCompletion();

        Destroy(fakeMeteor);
    }

    private Vector3 GetFakeMeteorSpawnPosition()
    {
        return _fakeMeteorSpawnPoint.position + transform.rotation * _fakeMeteorSpawnOffset;
    }

    private Vector3 GetMeteorSpawnPosition(Transform target)
    {
        Camera playerCamera = Camera.main;
        Vector3 targetViewport = playerCamera.WorldToViewportPoint(target.position);
        float depth = Vector3.Dot(target.position - playerCamera.transform.position, playerCamera.transform.forward);
        float margin = 0.35f;
        Vector3 spawnViewport = targetViewport;

        switch (Random.Range(0, 4))
        {
            case 0:
                spawnViewport.x = -margin;
                spawnViewport.y = Random.Range(0.1f, 0.9f);
                break;
            case 1:
                spawnViewport.x = 1f + margin;
                spawnViewport.y = Random.Range(0.1f, 0.9f);
                break;
            case 2:
                spawnViewport.x = Random.Range(0.1f, 0.9f);
                spawnViewport.y = -margin;
                break;
            default:
                spawnViewport.x = Random.Range(0.1f, 0.9f);
                spawnViewport.y = 1f + margin;
                break;
        }

        spawnViewport.z = depth;
        return playerCamera.ViewportToWorldPoint(spawnViewport);
    }
}
