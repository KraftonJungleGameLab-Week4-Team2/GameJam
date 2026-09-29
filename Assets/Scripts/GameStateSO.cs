using System;
using UnityEngine;

public enum GameState
{
    Mainmenu,
    Playing
}

[CreateAssetMenu(fileName = "GameStateSO", menuName = "ScriptableObjects/GameState", order = 1)]
public class GameStateSO : ScriptableObject
{
    public event Action<GameState> StateChanged;
    public GameState State { get; private set; }

    public void ApplyState(GameState state)
    {
        State = state;
        StateChanged?.Invoke(state);
    }
}
