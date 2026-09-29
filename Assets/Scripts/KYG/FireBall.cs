using System.Collections;
using UnityEngine;
public class FireBall : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 _fireBallPosition;
    public float speed = 5;
    public Vector3 target;
    void Start()
    {
        /*        _rigidbody = GetComponent<Rigidbody>();
                _rigidbody.AddForce(Vector3.down * 100, ForceMode.Impulse);  후보 1번 메테오 */

        _fireBallPosition = transform.position;
        target = new Vector3(_fireBallPosition.x, _fireBallPosition.y - 300, _fireBallPosition.z);

        StartCoroutine(MoveFireBall(_fireBallPosition, speed));



        StartCoroutine(FireBallDestroyTimer());
    }

    IEnumerator FireBallDestroyTimer()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
    IEnumerator MoveFireBall(Vector3 fireBallPosition, float speed)
    {
        float distance = (target - transform.position).magnitude;
        while (distance >= 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }

}//y -100
