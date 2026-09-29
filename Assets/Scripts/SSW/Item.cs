using UnityEngine;

[RequireComponent(typeof(Revolution))]
public class Item : MonoBehaviour
{
    private bool _isCollected;
    private int damage = 1;
    public event System.Action<Item> Collected;
    private GameObject boss;

    private IDamageable _bossTarget;

    private void Awake()
    {
        boss = GameObject.FindWithTag("Boss"); // 보스에 Tag 설정 필요
        if (boss != null)
        {
            boss.TryGetComponent<IDamageable>(out _bossTarget);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isCollected || !IsPlayer(other.transform))
        {

            return;
        }
        if (boss != null)
            _bossTarget.TakeDamage(damage);


        _isCollected = true;
        PlayerStatus player = other.GetComponentInParent<PlayerStatus>();
        player.AttackBoss();
        Collected?.Invoke(this);
    }

    private bool IsPlayer(Transform target)
    {
        while (target != null)
        {
            if (target.CompareTag("Player"))
            {
                return true;
            }

            target = target.parent;
        }

        return false;
    }
}

