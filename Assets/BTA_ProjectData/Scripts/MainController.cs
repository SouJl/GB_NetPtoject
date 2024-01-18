using Configs;
using Enumerators;
using System;
using UI;
using UnityEngine;

public class MainController : IDisposable
{
    private readonly Transform _placeForUi;
    private readonly GameConfig _gameConfig;
    private readonly LifeCycleController _lifeCycle;
    private readonly GamePrefs _gamePrefs;

    private AuthenticationMenuController _authenticationController;
    private LobbyMenuController _lobbyMenuController;

    public MainController(
        Transform placeForUi, 
        GameConfig gameConfig, 
        LifeCycleController lifeCycle)
    {
        _placeForUi = placeForUi;
        _gameConfig = gameConfig;
        _lifeCycle = lifeCycle;

        _gamePrefs = new GamePrefs();
        _gamePrefs.OnGameStateChange += GameStateChanged;

        _gamePrefs.ChangeGameState(GameState.Authentication);
    }


    private void GameStateChanged(GameState state)
    {
        DisposeControllers();
       
        switch (state)
        {
            default:
                break;
            case GameState.Authentication:
                {
                    _authenticationController = new AuthenticationMenuController(_placeForUi, _gameConfig, _gamePrefs);
                    
                    _lifeCycle.AddController(_authenticationController);
                    break;
                }
            case GameState.Lobby:
                {
                    _lobbyMenuController = new LobbyMenuController(_placeForUi, _gamePrefs);
                    
                    _lifeCycle.AddController(_lobbyMenuController);
                    break;
                }
        }
    }

    private void DisposeControllers()
    {
        _lifeCycle.Dispose();
    }

    public void Dispose()
    {
        DisposeControllers();

        _gamePrefs.OnGameStateChange -= GameStateChanged;
    }
}
