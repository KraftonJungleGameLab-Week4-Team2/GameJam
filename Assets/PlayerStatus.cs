using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int _playerHp = 5;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("플레이어 HP :" + _playerHp);
        if (_playerHp <= 0)
        {
            Debug.Log("플레이어 사망");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            _playerHp -= 1;
        }
        if (other.CompareTag("FireBall"))
        {
            _playerHp -= 1;
        }
    }
}
