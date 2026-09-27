using UnityEngine;

// 추후 메시 변형 컴포넌트가 구현한다. 좌표와 힘은 월드 기준이며, deltaTime은 적용 시간이다.
public interface IMeshDeformationTarget
{
    void ApplyDeformationForce(Vector3 worldPoint, Vector3 worldForce, float deltaTime);
}
