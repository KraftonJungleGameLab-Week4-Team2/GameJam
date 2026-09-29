using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GravityManager _gravityManager;
    [SerializeField]
    private PlayerMovement _playerMovement;

    [SerializeField]
    private CinemachineCamera _playerCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerMovement.Initialize(_gravityManager.GravityInfo);
    }

    public void StartGame()
    {
        // 초기화

        // 카메라 변경
        _playerCamera.gameObject.SetActive(true);

        // 게임 시작 지점
    }
}
