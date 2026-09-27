using UnityEngine;

public class GravityRangeCircle : MonoBehaviour
{
    [Header("범위 설정")]
    public float radius = 5f;

    [Header("도트 설정")]
    [Tooltip("도트 하나가 차지하는 실제 월드 간격 (반지름이 바뀌어도 이 값은 고정)")]
    public float dotSpacing = 0.5f;

    [Header("원 매끄러움")]
    [Tooltip("원을 이루는 점 개수. 48~64면 육안으로 충분히 매끄러움")]
    public int segments = 64;

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.loop = true;
        lr.useWorldSpace = true; // 부모 Scale 영향 안 받게, 항상 실제 월드 단위로 계산
        lr.textureMode = LineTextureMode.Tile; // 인스펙터에서도 같은 값으로 보일 것
    }

    void Start()
    {
        SetRadius(radius);
    }

    /// <summary>
    /// 반지름을 바꾸고 원을 다시 그림. 스킬 등으로 범위가 실시간으로
    /// 커지거나 작아질 때 이 함수만 호출하면 됨.
    /// </summary>
    public void SetRadius(float newRadius)
    {
        radius = newRadius;

        lr.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = (2f * Mathf.PI * i) / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius; // 바닥에 까는 원이면 z 사용
            lr.SetPosition(i, transform.position + new Vector3(x, 0f, z));
        }

        // 핵심: 원 둘레가 커진 만큼 Tiling도 같이 늘려서
        // 도트 하나의 실제 크기(간격)는 항상 dotSpacing으로 고정되게 함
        float circumference = 2f * Mathf.PI * radius;
        float tileCount = circumference / dotSpacing;

        Material mat = lr.material; // 런타임에 인스턴스가 하나 생성됨
        mat.mainTextureScale = new Vector2(tileCount, 1f);
    }

#if UNITY_EDITOR
    // 인스펙터에서 값 바꿀 때 바로 미리보기 갱신
    void OnValidate()
    {
        if (lr == null) lr = GetComponent<LineRenderer>();
        if (Application.isPlaying) SetRadius(radius);
    }
#endif
}
