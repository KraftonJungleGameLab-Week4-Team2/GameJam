using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class ScannerController : MonoBehaviour
{
    [Header("Speed")]
    public float speed = 5.0f;

    [Header("Destroy Time")]
    public float delay_destroy_time = 3.0f;

    // 스캔 범위 내 들어온 오브젝트들을 추적 (파괴 시 원복용)
    private HashSet<ScannablePlanet> scannedObjects = new HashSet<ScannablePlanet>();

    private void Reset()
    {
        // 물리 충돌 설정 자동화
        SphereCollider col = GetComponent<SphereCollider>();
        col.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // 물리 힘 연산 제외 (트리거 감지만 수행)
    }

    void Start()
    {
        //destroy_object();
    }

    void Update()
    {
        // 구체 스케일 키우기
        float growing = this.speed * Time.deltaTime;
        this.transform.localScale += new Vector3(growing, growing, growing);

        // 셰이더 전역 변수 업데이트
        Shader.SetGlobalVector("_ScanCenter", transform.position);
        Shader.SetGlobalFloat("_ScanRadius", transform.localScale.x * 0.5f);
    }

    private void destroy_object()
    {
        Destroy(this.gameObject, delay_destroy_time);
    }

    private void OnTriggerEnter(Collider other)
    {

        var scannables = other.GetComponentsInParent<ScannablePlanet>();
        foreach (var scannable in scannables)
        {
            scannable.OnScanEnter();
            scannedObjects.Add(scannable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var scannables = other.GetComponentsInParent<ScannablePlanet>();
        foreach (var scannable in scannables)
        {
            scannable.OnScanExit();
            scannedObjects.Remove(scannable);
        }
    }

    // 스캐너 구체가 Destroy될 때 남아있는 스캔 대상들 원상복구
    private void OnDestroy()
    {
        foreach (var scannable in scannedObjects)
        {
            if (scannable != null)
            {
                scannable.OnScanExit();
            }
        }
        scannedObjects.Clear();
    }
}