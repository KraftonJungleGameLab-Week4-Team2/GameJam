using System.Collections;
using UnityEngine;
public class BossAttack : MonoBehaviour
{
    public GameObject attackCube;
    [SerializeField] private Transform _playerPos;
    [SerializeField] private GameObject _fakeFireBall;
    public Vector3 playerRightUp;
    public bool nomalattack;
    public bool fireBallAttack;
    private float _fakeFireBallTimer = 5f; //5초는 안할겁니다
    public Transform boss;
    private Vector3 _bossRight;
    private Vector3 _bossLeft;
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
            StartCoroutine(CallFake());
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
    IEnumerator CallFake()
    {
        yield return StartCoroutine(FireBall());

        yield return new WaitForSeconds(_fakeFireBallTimer);
        fireBallAttack = true;
    }
    private IEnumerator FireBall()
    {
        _bossLeft = boss.position;
        GameObject fakeFireBall = Instantiate(_fakeFireBall, _bossLeft, Quaternion.identity);
        fireBallAttack = false;
        Rigidbody fakeRb = fakeFireBall.GetComponent<Rigidbody>();

        yield return new WaitForSeconds(0.5f); //발사전 대기

        fakeRb.AddForce(new Vector3(0, 100, -30), ForceMode.Impulse);

        yield return new WaitForSeconds(1f); //발사후 화면 밖으로 나가면 삭제

        Destroy(fakeFireBall);

        yield return new WaitForSeconds(3f); //
        int randomIndex = Random.Range(0, _planetAry.Length);

        Transform ramdomPlanet = _planetAry[randomIndex];

        GameObject fireballWarning = Instantiate(_WarningPrefab, ramdomPlanet.position, Quaternion.identity, ramdomPlanet);
        yield return new WaitForSeconds(3f); //원래는 경고등 개념이였던것
        Destroy(fireballWarning);


        //하나의 메테오를 생성시켜서 추락 X,Y 조정한 축위에서
        Vector3 fireBallPosition = ramdomPlanet.position + new Vector3(0, 50, 0);
        GameObject fireBall = Instantiate(_fireBallPrefab, fireBallPosition, Quaternion.identity);
    }


}
