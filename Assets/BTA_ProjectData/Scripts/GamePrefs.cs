using Enumerators;
using System;

public class GamePrefs
{
    private GameState _gameState;
    private string _userId;

    public event Action<GameState> OnGameStateChange;

    public void ChangeGameState(GameState gameState)
    {
        _gameState = gameState;

        OnGameStateChange?.Invoke(_gameState);
    }

    public void SetUserId(string userId)
    {
        _userId = userId;
    }

    public string GetUserId() => _userId;
}
