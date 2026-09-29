using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameStateSO _gameStateSO;

    [SerializeField]
    private GravityManager _gravityManager;
    [SerializeField]
    private PlayerMovement _playerMovement;

    [SerializeField]
    private CinemachineCamera _playerCamera;

    [SerializeField]
    private ScreenEffectManager _screenEffectManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameStateSO.ApplyState(GameState.Mainmenu);
        _playerMovement.Initialize(_gravityManager.GravityInfo);
    }

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        // 초기화

        // 카메라 변경
        _playerCamera.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        _gameStateSO.ApplyState(GameState.Playing);


        // 스크린 효과 적용
        _screenEffectManager.IsUse = true;

        // 대화창 진행

        // 보스 시작

    }
}
