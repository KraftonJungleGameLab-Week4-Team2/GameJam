using UnityEngine;

public class MainScreen : MonoBehaviour
{
    [SerializeField] private Animator _animator;

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
