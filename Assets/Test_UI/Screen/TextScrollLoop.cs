using TMPro;
using UnityEngine;

public class TextScrollLoop : MonoBehaviour
{
    public enum ScrollDirection
    {
        Left,
        Right
    }

    [Header("References")]
    public RectTransform[] textList;

    [Header("Settings")]
    public ScrollDirection scrollDirection = ScrollDirection.Left;
    public float scrollSpeed = 200f;

    private float[] textWidths; // 각 텍스트의 실제 너비 저장용

    void Start()
    {
        if (textList == null || textList.Length == 0) return;

        textWidths = new float[textList.Length];

        // 1. 각 텍스트의 실제 너비를 계산하고 초기 위치를 나란히 배치합니다.
        float currentX = 0f;
        for (int i = 0; i < textList.Length; i++)
        {
            TextMeshProUGUI tmp = textList[i].GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                textWidths[i] = tmp.preferredWidth;
            }
            else
            {
                textWidths[i] = textList[i].rect.width;
            }

            // 초기 X 위치 설정
            textList[i].anchoredPosition = new Vector2(currentX, textList[i].anchoredPosition.y);

            // 다음 텍스트가 놓일 X 위치
            currentX += textWidths[i];
        }
    }

    void Update()
    {
        if (textList == null || textList.Length == 0) return;

        if (scrollDirection == ScrollDirection.Left)
        {
            // --- [왼쪽으로 흐를 때] ---
            for (int i = 0; i < textList.Length; i++)
            {
                textList[i].anchoredPosition += Vector2.left * scrollSpeed * Time.deltaTime;
            }

            // 맨 앞 텍스트가 왼쪽 끝으로 완전히 나갔을 때 -> 맨 뒤로 보냄
            if (textList[0].anchoredPosition.x <= -textWidths[0])
            {
                int lastIndex = textList.Length - 1;
                float newX = textList[lastIndex].anchoredPosition.x + textWidths[lastIndex];

                RectTransform firstText = textList[0];
                float firstWidth = textWidths[0];

                for (int i = 0; i < textList.Length - 1; i++)
                {
                    textList[i] = textList[i + 1];
                    textWidths[i] = textWidths[i + 1];
                }

                textList[lastIndex] = firstText;
                textWidths[lastIndex] = firstWidth;
                textList[lastIndex].anchoredPosition = new Vector2(newX, textList[lastIndex].anchoredPosition.y);
            }
        }
        else
        {
            // --- [오른쪽으로 흐를 때] ---
            for (int i = 0; i < textList.Length; i++)
            {
                textList[i].anchoredPosition += Vector2.right * scrollSpeed * Time.deltaTime;
            }

            int lastIndex = textList.Length - 1;

            // 맨 뒤 텍스트가 오른쪽으로 완전히 나갔을 때 -> 맨 앞으로 보냄
            // (기준 좌표 계산은 캔버스 환경에 따라 다를 수 있으므로, 오른쪽 끝 탈출 조건 체크)
            if (textList[lastIndex].anchoredPosition.x >= textWidths[lastIndex] * textList.Length) // 대략적인 우측 경계 예시
            {
                // 오른쪽 스크롤 시의 순환 로직 처리
                RectTransform lastText = textList[lastIndex];
                float lastWidth = textWidths[lastIndex];

                float newX = textList[0].anchoredPosition.x - lastWidth;

                for (int i = textList.Length - 1; i > 0; i--)
                {
                    textList[i] = textList[i - 1];
                    textWidths[i] = textWidths[i - 1];
                }

                textList[0] = lastText;
                textWidths[0] = lastWidth;
                textList[0].anchoredPosition = new Vector2(newX, textList[0].anchoredPosition.y);
            }
        }
    }
}
