using System.Collections;
using UnityEngine;
public class BossAttack : MonoBehaviour
{
    public GameObject attackCube;
    [SerializeField] private Transform _playerPos;
    public Vector3 playerRightUp;
    public bool nomalattack;
    public Transform boss;
    public Vector3 bossRight;

    private void Start()
    {
        nomalattack = true;
    }
    void Update()
    {
        if (nomalattack)
        {
            StartCoroutine(CallNomal()); //4초마다 발사 하기 위한 코루틴

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
        bossRight = boss.position + new Vector3(50f, 0, 0);

        //플레이어 위치 저장
        Vector3 playerPoint = _playerPos.position;

        //랜덤위치 생성을 위한 배열
        Vector3[] waypoint = { playerPoint + new Vector3(13, 15, 0), playerPoint + new Vector3(-13, -15, 0), playerPoint + new Vector3(-13, 15, 0), playerPoint + new Vector3(13, -15, 0) };

        Transform playerNow = _playerPos;
        //Vector3 waypoint = playerNow + new Vector3(13, 15, 0);
        int randomIndex = Random.Range(0, 4);

        GameObject obj = Instantiate(attackCube, bossRight, Quaternion.identity); //발사체를 저장하여 함수부여

        obj.GetComponent<Bullet>().Init(waypoint[randomIndex], playerNow);

    }


}
