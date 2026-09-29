using UnityEngine;

public class ScannerTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 닿은 오브젝트(또는 그 자식/부모)에서 IScannable을 찾아 실행
        var scannables = other.GetComponents<IScannable>();
        foreach (var scannable in scannables)
        {
            scannable.OnScanEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var scannables = other.GetComponents<IScannable>();
        foreach (var scannable in scannables)
        {
            scannable.OnScanExit();
        }
    }

    private void Update()
    {
        // 셰이더 전역 변수 업데이트
        Shader.SetGlobalVector("_ScanCenter", transform.position);
        Shader.SetGlobalFloat("_ScanRadius", transform.localScale.x * 0.5f);
    }
}