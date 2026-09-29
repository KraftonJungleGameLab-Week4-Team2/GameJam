using UnityEngine;

public class OffScreenIndicator : MonoBehaviour
{
    [Header("References")]
    public Transform targetTr;
    public RectTransform indicatorImageTr;
    public RectTransform canvasTr;

    [Header("Settings")]
    public float edgeBuffer = 50f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (targetTr == null)
        {
            return;
        }

        // 월드 좌표를 스크린 좌표로 변환
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(targetTr.position);

        // 화면 안에 있는지 체크
        bool isOffScreen = screenPoint.x < 0 || screenPoint.x > Screen.width ||
                           screenPoint.y < 0 || screenPoint.y > Screen.height;

        // 화면 밖
        if (isOffScreen)
        {
            if (indicatorImageTr.gameObject.activeSelf == false)
            {
                indicatorImageTr.gameObject.SetActive(true);
            }

            UpdateIndicatorPosition(screenPoint);
        }
        // 화면 안
        else
        {
            if (indicatorImageTr.gameObject.activeSelf)
            {
                indicatorImageTr.gameObject.SetActive(false);
            }
        }
    }

    void UpdateIndicatorPosition(Vector3 screenPoint)
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 screenPos = (Vector2)screenPoint - screenCenter;

        float scaleX = canvasTr.sizeDelta.x / Screen.width;
        float scaleY = canvasTr.sizeDelta.y / Screen.height;

        Vector2 canvasCenter = Vector2.zero;
        Vector2 pos = new Vector2(screenPos.x * scaleX, screenPos.y * scaleY);

        // 가장자리 위치 구하기
        float maxX = (canvasTr.sizeDelta.x / 2f) - edgeBuffer;
        float maxY = (canvasTr.sizeDelta.y / 2f) - edgeBuffer;

        float slope = pos.y / pos.x;
        if (pos.x > 0)
        {
            pos.x = maxX;
            pos.y = maxX * slope;
        }
        else
        {
            pos.x = -maxX;
            pos.y = -maxX * slope;
        }

        if (pos.y > maxY)
        {
            pos.y = maxY;
            pos.x = maxY / slope;
        }
        else if (pos.y < -maxY)
        {
            pos.y = -maxY;
            pos.x = -maxY / slope;
        }

        // 위치
        indicatorImageTr.anchoredPosition = pos;

        // 회전
        float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        indicatorImageTr.rotation = Quaternion.Euler(0, 0, angle);
    }
}
