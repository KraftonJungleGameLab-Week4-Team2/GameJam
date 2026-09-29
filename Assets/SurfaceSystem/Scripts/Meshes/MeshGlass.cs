using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(SurfaceInstance))]
public class MeshGlass : MonoBehaviour
{
    private bool _isFracturing;
    private bool _isBroken;
    private GameObject _fragments;
    private readonly List<Mesh> _ownedMeshes = new List<Mesh>();

    public event Action OnMeshBroken;
    public event Action RestoreRequested;

    public bool IsBroken { get { return _isBroken; } }
    public bool IsFracturing { get { return _isFracturing; } }

    // 공유 Effect 대신 이 행성에서 중복 실행과 생성한 메시의 수명을 관리한다.
    public void BeginFracture(FractureEffect effect, Vector3 burstOrigin)
    {
        if (_isFracturing || _isBroken)
        {
            return;
        }

        _isFracturing = true;
        StartCoroutine(RunFracture(effect, burstOrigin));
    }


    // 알고리즘 실패 시 원본 행성을 유지하고 생성 중이던 파편을 정리한다.
    private IEnumerator RunFracture(FractureEffect effect, Vector3 burstOrigin)
    {
        IEnumerator operation = effect.Fracture(this, burstOrigin);
        while (true)
        {
            bool hasNext;
            try
            {
                hasNext = operation.MoveNext();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                ClearFragments();
                _isFracturing = false;
                yield break;
            }

            if (!hasNext)
            {
                break;
            }

            yield return operation.Current;
        }

        _isFracturing = false;
        if (_isBroken)
        {
            yield return new WaitForSeconds(effect.FragmentLifetime);
            ClearFragments();
        }

        var beforeScale = transform.localScale;
        transform.localScale = Vector3.zero;

        // 파괴 후 다시 복구하는 처리
        yield return new WaitForSeconds(effect.RestoreTime);

        if (RestoreRequested != null)
        {
            RestoreRequested.Invoke();
            yield break;
        }

        GetComponent<Renderer>().enabled = true;

        foreach (Collider sourceCollider in GetComponentsInChildren<Collider>())
        {
            sourceCollider.enabled = true;
        }

        transform.DOScale(beforeScale, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            transform.DOPunchScale(Vector3.one * 2f, 0.5f).OnComplete(() =>
            {
                transform.localScale = beforeScale;
                _isBroken = false;
            });
        });
    }

    // 생성한 런타임 메시만 소유하며 프로젝트 원본 메시를 삭제하지 않는다.
    public void TrackMesh(Mesh mesh)
    {
        _ownedMeshes.Add(mesh);
    }

    // 파편을 먼저 비활성 상태로 만들고 완성된 뒤 한 번에 공개한다.
    public GameObject CreateFragmentRoot()
    {
        _fragments = new GameObject(name + " Fragments");
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(_fragments, gameObject.scene);
        _fragments.SetActive(false);
        return _fragments;
    }

    // 원본의 렌더링과 충돌만 끈다. 행성 중심과 다른 협업자의 컴포넌트는 유지한다.
    public void CompleteFracture()
    {
        GetComponent<MeshRenderer>().enabled = false;
        // Planet trigger colliders are commonly placed on child objects. Disable
        // those together with the root collider so the broken planet stops
        // receiving collision and trigger callbacks.
        foreach (Collider sourceCollider in GetComponentsInChildren<Collider>())
        {
            sourceCollider.enabled = false;
        }

        _isBroken = true;
        OnMeshBroken?.Invoke();
        _fragments.SetActive(true);
    }

    // 파편 오브젝트와 런타임 메시를 함께 해제한다.
    private void ClearFragments()
    {
        if (_fragments != null)
        {
            Destroy(_fragments);
        }

        foreach (Mesh mesh in _ownedMeshes)
        {
            Destroy(mesh);
        }

        _ownedMeshes.Clear();
    }

    void OnDisable()
    {
        StopAllCoroutines();
        ClearFragments();
        _isFracturing = false;
    }
}
