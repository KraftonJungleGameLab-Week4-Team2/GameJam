using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScreen : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [Space]
    [SerializeField] private Slider _bossHpSlider;
    [SerializeField] private GameObject[] _playerHp;
    [SerializeField] private Image _retryImage;
    [SerializeField] private float _holdDuration = 1.5f;
    [Space]
    [SerializeField] private GameObject[] _dialogues;
    [SerializeField] private GameObject _nextButton;
    [SerializeField] private GameObject[] _afterDialougeActive;

    [Header("In Game")]
    [SerializeField] private int _currentDialogueIndex = 0;
    [SerializeField] private float _currentHoldTime = 0f;
    [SerializeField] private bool _isRetry = false;

    void Start()
    {
        _currentDialogueIndex = 0;
        SetDialogue(_currentDialogueIndex);

        PlayerStatus playerStatus = FindFirstObjectByType<PlayerStatus>();
        Boss boss = FindFirstObjectByType<Boss>();
        playerStatus.BindUI(this, boss);
        boss.BindUI(this);
    }

    public void SetDialogue(int index)
    {
        for (int i = 0; i < _dialogues.Length; i++)
        {
            if (i == index)
            {
                _dialogues[i].SetActive(true);
            }
            else
            {
                _dialogues[i].SetActive(false);
            }
        }
    }

    public void NextDialogue()
    {
        _currentDialogueIndex++;

        SetDialogue(_currentDialogueIndex);

        // 다이얼로그 대화창 끝이면
        if (_currentDialogueIndex > _dialogues.Length - 1)
        {
            _nextButton.SetActive(false);

            // 다 켜줌
            for (int i = 0; i < _afterDialougeActive.Length; i++)
            {
                _afterDialougeActive[i].SetActive(true);
            }
            return;
        }
    }

    public void SetPlayerHP(int hp)
    {
        for (int i = 0; i < _playerHp.Length; i++)
        {
            if (i < hp)
            {
                _playerHp[i].SetActive(true);
            }
            else
            {
                _playerHp[i].SetActive(false);
            }
        }
    }

    public void SetBossHp(float hp)
    {
        _bossHpSlider.value = hp;
    }

    public void SetBossHealth(float hp, float maxHp)
    {
        _bossHpSlider.minValue = 0f;
        _bossHpSlider.maxValue = maxHp;
        _bossHpSlider.value = hp;
    }

    void Update()
    {
        if (_isRetry)
        {
            return;
        }

        // R 키 홀드 하면 재시작
        if (Keyboard.current != null && Keyboard.current.rKey.isPressed)
        {
            _currentHoldTime += Time.deltaTime;

            if (_currentHoldTime >= _holdDuration)
            {
                _currentHoldTime = _holdDuration;
                RetryTrigger();
                _isRetry = true;
            }
        }
        else
        {
            _currentHoldTime = 0f;
            _retryImage.fillAmount = 0f;
        }

        _retryImage.fillAmount = _currentHoldTime / _holdDuration;
    }

    void RetryTrigger()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _animator.SetTrigger("Show");
    }

    public void Hide()
    {
        _animator.SetTrigger("Hide");
    }
}
