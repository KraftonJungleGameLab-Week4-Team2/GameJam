using UnityEngine;
using UnityEngine.Events;

public class Boss : MonoBehaviour, ISurfaceDamageReceiver
{
    [SerializeField, Min(1f)] private float _maxHealth = 5f;

    private float _currentHealth;
    private MainScreen _mainScreen;
    private BossAttack _bossAttack;

    public UnityEvent OnBossDieEvent;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public bool IsDead => _currentHealth <= 0f;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _bossAttack = GetComponent<BossAttack>();
    }

    public void BindUI(MainScreen mainScreen)
    {
        _mainScreen = mainScreen;
        _mainScreen.SetBossHealth(_currentHealth, _maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f)
        {
            return;
        }

        _currentHealth = Mathf.Max(0f, _currentHealth - amount);
        _mainScreen.SetBossHealth(_currentHealth, _maxHealth);

        if (IsDead)
        {
            _bossAttack.enabled = false;
            gameObject.SetActive(false);

            OnBossDieEvent?.Invoke();
        }
    }

    public void TakeSurfaceDamage(float amount)
    {
        TakeDamage(amount);
    }
}
