using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerVari _playerVari;

    [SerializeField] private IPlayerInput _playerInput;
    [SerializeField] private PlayerController _playerController;

    [SerializeField] private PlayerMovement _playerMovement;
    private void Start()
    {
        _playerMovement =  GetComponent<PlayerMovement>();
        _playerController = GetComponent<PlayerController>();
        var gravityInfo = FindAnyObjectByType<GravityManager>().GravityInfo;
        _playerController.Initialize(gravityInfo, _playerInput, _playerVari);
        _playerMovement.Initialize(_playerInput, _playerVari, gravityInfo); }
}
