using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GravityManager _gravityManager;
    [SerializeField]
    private PlayerMovement _playerMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerMovement.Initialize(_gravityManager.GravityInfo);
    }

}
