using UnityEngine;

public class PlayerStatus : MonoBehaviour, ISurfaceDamageReceiver
{
    [SerializeField, Min(1)] private int _maxHealth = 5;

    private int _currentHealth;
    private MainScreen _mainScreen;
    private Boss _bossTarget;
    private PlayerMovement _playerMovement;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public bool IsDead => _currentHealth <= 0;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _playerMovement = GetComponent<PlayerMovement>();
    }

    public void BindUI(MainScreen mainScreen, Boss bossTarget)
    {
        _mainScreen = mainScreen;
        _bossTarget = bossTarget;
        _mainScreen.SetPlayerHP(_currentHealth);
    }

    public void AttackBoss()
    {
        if (!IsDead)
        {
            _bossTarget.TakeDamage(1f);
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f)
        {
            return;
        }

        _currentHealth = Mathf.Max(0, _currentHealth - Mathf.CeilToInt(amount));
        _mainScreen.SetPlayerHP(_currentHealth);

        if (IsDead)
        {
            _playerMovement.enabled = false;
            Debug.Log("플레이어 사망");
        }
    }

    public void TakeSurfaceDamage(float amount)
    {
        TakeDamage(amount);
    }
}
