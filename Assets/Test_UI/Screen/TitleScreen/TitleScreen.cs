using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CanvasGroup _canvasGroup;
    [Space]
    [SerializeField] private MenuButtonFocusKeeper _focusKeeper;
    [SerializeField] private HowToPlayScreen _howToPlayScreen;
    [SerializeField] private OptionsScreen _optionsScreen;
    [SerializeField] private MainScreen _mainScreen;

    public void Show()
    {
        _focusKeeper.SetupDefault();
        _canvasGroup.blocksRaycasts = true;
        _animator.SetTrigger("Show");
    }

    public void Hide()
    {
        _canvasGroup.blocksRaycasts = false;
        _animator.SetTrigger("Hide");
    }

    public void OnClickNewGameButton()
    {
        Debug.Log("New Game");

        Hide();
        _mainScreen.Show();
    }

    public void OnClickHowToPlayButton()
    {
        Debug.Log("How to Play");

        Hide();
        _howToPlayScreen.Show();
    }

    public void OnClickOptionsButton()
    {
        Debug.Log("Options");

        Hide();
        _optionsScreen.Show();
    }

    public void OnClickQuitButton()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
