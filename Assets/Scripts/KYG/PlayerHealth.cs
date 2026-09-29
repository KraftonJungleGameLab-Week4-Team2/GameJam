using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    private int _playerMaxHp = 5;
    public int playerCurrentHp;
    private int _damage = 1;
    private bool _isPlayerDead;

    // Update is called once per frame
    private void OnEnable()
    {
        playerCurrentHp = _playerMaxHp;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            TakeDamage(_damage);
        }
        if (other.CompareTag("FireBall"))
        {
            TakeDamage(_damage);
        }
    }
    public void TakeDamage(int damage)
    {
        if (playerCurrentHp <= 0) return;
        playerCurrentHp -= damage;
        Debug.Log($"{nameof(PlayerHealth)}플레이어 Hp:" + playerCurrentHp);
        if (playerCurrentHp <= 0)
        {
            Die();
        }

    }
    private void Die()
    {
        _isPlayerDead = true;
        gameObject.SetActive(false);
    }
}
