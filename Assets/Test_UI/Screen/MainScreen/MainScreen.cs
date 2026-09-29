using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScreen : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [Space]
    [SerializeField] private Image _retryImage;
    [SerializeField] private float _holdDuration = 1.5f;

    [Header("In Game")]
    [SerializeField] private float _currentHoldTime = 0f;
    [SerializeField] private bool _isRetry = false;

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
