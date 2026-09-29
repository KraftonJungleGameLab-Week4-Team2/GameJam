using UnityEngine;

public class BossHp : MonoBehaviour, IDamageable
{
    private int _bossMaxHp = 5;
    public int bossCurrentHp;
    private bool _isBossDead;
    private void OnEnable()
    {
        bossCurrentHp = _bossMaxHp;
    }
    public void TakeDamage(int damage)
    {
        if (bossCurrentHp <= 0) return;
        bossCurrentHp -= damage;
        Debug.Log($"{nameof(BossHp)}보스 Hp:" + bossCurrentHp);
        if (bossCurrentHp <= 0)
        {
            Die();
        }

    }
    private void Die()
    {
        _isBossDead = true;
        gameObject.SetActive(false);
    }
}
