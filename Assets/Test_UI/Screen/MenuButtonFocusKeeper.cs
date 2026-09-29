using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonFocusKeeper : MonoBehaviour
{
    [SerializeField] private Button _defaultButton;

    private GameObject _lastSelectedGameObject;

    void Start()
    {
        SetupDefault();
    }

    public void SetupDefault()
    {
        _defaultButton.Select();
        _lastSelectedGameObject = _defaultButton.gameObject;
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            _lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
        }
        else if (_lastSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(_lastSelectedGameObject);
        }
    }
}
