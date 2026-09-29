using UnityEngine;


public interface IScannable
{
    void OnScanEnter();
    void OnScanExit();
}

public class ScannablePlanet : MonoBehaviour, IScannable
{
    [Header("Materials")]
    public Material M_Default;
    public Material hologramMaterial;

    [Header("Child Ring Effect")]
    public GameObject[] ringObjects;     // 자식 오브젝트로 연결된 불의 고리

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {

        if (meshRenderer != null && M_Default != null)
        {
            meshRenderer.material = M_Default;
        }
    }

    // 스캐너 영향권에 들어왔을 때 실행
    public void OnScanEnter()
    {
        Debug.Log("스캔시작");
        if (meshRenderer != null && hologramMaterial != null)
        {
            meshRenderer.material = hologramMaterial;
        }


        if (ringObjects != null)
        {
            foreach (GameObject ringOb in ringObjects)
            {
                ringOb.SetActive(false);
            }
        }
    }

    // 스캐너 영향권에서 벗어났을 때 실행
    public void OnScanExit()
    {
        Debug.Log("스캔끝");
        if (meshRenderer != null && M_Default != null)
        {
            meshRenderer.material = M_Default;
        }


        if (ringObjects != null)
        {
            foreach (GameObject ringOb in ringObjects)
            {
                ringOb.SetActive(true);
            }
        }
    }


}