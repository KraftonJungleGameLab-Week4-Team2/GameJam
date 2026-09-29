using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour
{
    [Header("References")]
    public Transform targetTr;
    public RectTransform indicatorImageTr;
    public RectTransform meteorIndicatorImageTr;
    public RectTransform canvasTr;

    [Header("Settings")]
    public float edgeBuffer = 50f;
    [SerializeField] private Color _meteorColor = Color.red;
    [SerializeField, Min(0f)] private float _alertOffset = 42f;

    private static readonly List<Transform> _meteorTargets = new List<Transform>();
    private Camera _mainCamera;
    private Image _itemArrowImage;
    private Image _meteorArrowImage;
    private TMP_Text _meteorAlertText;
    private Color _itemArrowColor;

    private void Awake()
    {
        _mainCamera = Camera.main;

        _itemArrowImage = indicatorImageTr.GetComponentInChildren<Image>(true);
        _itemArrowColor = _itemArrowImage.color;
        _meteorArrowImage = meteorIndicatorImageTr.GetComponentInChildren<Image>(true);
        _meteorAlertText = meteorIndicatorImageTr.GetComponentInChildren<TMP_Text>(true);

        _meteorArrowImage.color = _meteorColor;

        indicatorImageTr.gameObject.SetActive(false);
        meteorIndicatorImageTr.gameObject.SetActive(false);
    }

    public static void RegisterMeteor(Transform meteor)
    {
        if (!_meteorTargets.Contains(meteor))
        {
            _meteorTargets.Add(meteor);
        }
    }

    public static void UnregisterMeteor(Transform meteor)
    {
        _meteorTargets.Remove(meteor);
    }

    private void Update()
    {
        UpdateItemIndicator();
        UpdateMeteorIndicator();
    }

    private void UpdateItemIndicator()
    {
        Vector2 position = default;
        bool isVisible = targetTr != null && TryGetOffScreenPosition(targetTr, out position);
        indicatorImageTr.gameObject.SetActive(isVisible);
        _itemArrowImage.color = _itemArrowColor;

        if (isVisible)
        {
            SetIndicatorPosition(indicatorImageTr, position);
        }
    }

    private void UpdateMeteorIndicator()
    {
        Transform meteor = GetOffScreenMeteor();
        Vector2 position = default;
        bool isVisible = meteor != null && TryGetOffScreenPosition(meteor, out position);
        meteorIndicatorImageTr.gameObject.SetActive(isVisible);

        if (!isVisible)
        {
            return;
        }

        SetIndicatorPosition(meteorIndicatorImageTr, position);
        RectTransform alertRect = _meteorAlertText.rectTransform;
        alertRect.position = meteorIndicatorImageTr.position + Vector3.up * _alertOffset;
        alertRect.rotation = Quaternion.identity;
    }

    private Transform GetOffScreenMeteor()
    {
        for (int i = _meteorTargets.Count - 1; i >= 0; i--)
        {
            Transform meteor = _meteorTargets[i];
            if (meteor == null)
            {
                _meteorTargets.RemoveAt(i);
                continue;
            }

            if (TryGetOffScreenPosition(meteor, out _))
            {
                return meteor;
            }
        }

        return null;
    }

    private bool TryGetOffScreenPosition(Transform target, out Vector2 position)
    {
        Vector3 screenPoint = _mainCamera.WorldToScreenPoint(target.position);
        bool isOffScreen = screenPoint.z <= 0f || screenPoint.x < 0f || screenPoint.x > Screen.width
            || screenPoint.y < 0f || screenPoint.y > Screen.height;

        if (!isOffScreen)
        {
            position = default;
            return false;
        }

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 direction = (Vector2)screenPoint - screenCenter;
        if (screenPoint.z <= 0f)
        {
            direction = -direction;
        }

        direction = Vector2.Scale(direction, new Vector2(canvasTr.sizeDelta.x / Screen.width,
            canvasTr.sizeDelta.y / Screen.height));

        float maxX = canvasTr.sizeDelta.x / 2f - edgeBuffer;
        float maxY = canvasTr.sizeDelta.y / 2f - edgeBuffer;
        float scaleX = Mathf.Abs(direction.x) > 0.001f ? maxX / Mathf.Abs(direction.x) : float.PositiveInfinity;
        float scaleY = Mathf.Abs(direction.y) > 0.001f ? maxY / Mathf.Abs(direction.y) : float.PositiveInfinity;
        position = direction * Mathf.Min(scaleX, scaleY);
        return true;
    }

    private static void SetIndicatorPosition(RectTransform indicator, Vector2 position)
    {
        indicator.anchoredPosition = position;
        float angle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
        if (indicator.name == "MeteorArrow")
        {
            angle -= 90f;
        }
        indicator.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
