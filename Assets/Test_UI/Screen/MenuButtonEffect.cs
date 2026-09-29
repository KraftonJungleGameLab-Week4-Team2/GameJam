using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonEffect : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private Image _image;
    [Space]
    [SerializeField] private Color _normalColor = new Color(203 / 255f, 1f, 1f, 70 / 255f);
    [SerializeField] private Color _highlightColor = new Color(203 / 255f, 1f, 1f, 1f);

    void Awake()
    {
        OnDeselect(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _button.Select();
    }

    public void OnSelect(BaseEventData eventData)
    {
        _buttonText.color = _highlightColor;

        _image.gameObject.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _buttonText.color = _normalColor;

        _image.gameObject.SetActive(false);
    }
}
