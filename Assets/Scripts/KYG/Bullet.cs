using System.Collections;
using UnityEngine;
public class Bullet : MonoBehaviour
{

    [SerializeField] private float speed = 60;
    //[SerializeField] private float waitAtWaypoint = 0.3f;
    [SerializeField] private float lifeTime = 5f;
    private Vector3 waypoint;
    private Transform target;
    private Vector3 dir;

    public void Init(Vector3 waypoint, Transform target) //BossAttack에서 첫번째 스탑 위치와 , 목표위치를 불러옴
    {
        this.waypoint = waypoint;
        this.target = target;
        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        while (Vector3.Distance(transform.position, waypoint) > 1f) //운석이 일정거리가 가까워질때까지 접근
        {
            var targetRot = Quaternion.LookRotation((waypoint - transform.position).normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 2f * Time.deltaTime);
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            //transform.position = Vector3.MoveTowards(transform.position, waypoint, speed * Time.deltaTime);
            yield return null; //한번에 도착하지 않기 위함
        }

        if (target)
            dir = (target.position - transform.position).normalized; //이동방향

        float time = 0f;
        while (time < lifeTime) // lifeTime 전까지 날라감
        {
            var targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
            transform.Translate(Vector3.forward * Time.deltaTime * speed);

            //transform.position += dir * speed * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Planet"))
        {
            Destroy(gameObject);
        }
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }

    }
}
