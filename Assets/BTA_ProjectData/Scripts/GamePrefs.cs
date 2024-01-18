using Enumerators;
using System;

public class GamePrefs
{
    private GameState _gameState;

    public event Action<GameState> OnGameStateChange;

    public void ChangeGameState(GameState gameState)
    {
        _gameState = gameState;

        OnGameStateChange?.Invoke(_gameState);
    }
}
