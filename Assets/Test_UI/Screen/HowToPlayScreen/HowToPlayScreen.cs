using UnityEngine;
using UnityEngine.InputSystem;

public class HowToPlayScreen : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CanvasGroup _canvasGroup;
    [Space]
    [SerializeField] private TitleScreen _titleScreen;
    [Space]
    [SerializeField] private bool _isShowing = false;

    void Update()
    {
        if (_isShowing && CheckCancelInput())
        {
            OnClickBackButton();
        }
    }

    private bool CheckCancelInput()
    {
        if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
            return true;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            return true;

        return false;
    }

    public void Show()
    {
        _isShowing = true;

        gameObject.SetActive(true);
        _canvasGroup.blocksRaycasts = true;
        _animator.SetTrigger("Show");
    }

    public void Hide()
    {
        _isShowing = false;

        _canvasGroup.blocksRaycasts = false;
        _animator.SetTrigger("Hide");
    }

    public void OnClickBackButton()
    {
        Hide();
        _titleScreen.Show();
    }
}
