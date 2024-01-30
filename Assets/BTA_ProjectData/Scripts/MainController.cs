using Configs;
using Enumerators;
using GameLobby;
using MultiplayerService;
using Prefs;
using System;
using UI;
using UnityEngine;

public class MainController : IDisposable
{
    private readonly Transform _placeForUi;
    private readonly GameConfig _gameConfig;
    private readonly LifeCycleController _lifeCycle;
    private readonly PhotonNetManager _netManager;

    private readonly GamePrefs _gamePrefs;

    private readonly IMultiplayerService _multiplayerService;

    private MainMenuController _mainMenuController;
    private AuthenticationMenuController _authenticationController;
    private GameLobbyController _gameLobbyController;

    public MainController(
        Transform placeForUi, 
        GameConfig gameConfig, 
        LifeCycleController lifeCycle,
        PhotonNetManager netManager)
    {
        _placeForUi = placeForUi;
        _gameConfig = gameConfig;
        _lifeCycle = lifeCycle;
        _netManager = netManager;

        _gamePrefs = new GamePrefs();
        _multiplayerService = new PlayFabMultiplayerService(_gameConfig);
        
        _netManager.Init(gameConfig);

        InitialGameLoad();
    }

    private void InitialGameLoad()
    {
        _gamePrefs.OnGameStateChange += GameStateChanged;

        var userSate = _gamePrefs.Load();

        if(userSate == false)
        {
            _gamePrefs.ChangeGameState(GameState.Authentication);
        }
        else
        {
            _gamePrefs.ChangeGameState(GameState.MainMenu);
        }
    }

    private void GameStateChanged(GameState state)
    {
        DisposeControllers();
       
        switch (state)
        {
            default:
                break;
            case GameState.MainMenu:
                {
                    _mainMenuController = new MainMenuController(_placeForUi, _gamePrefs, _netManager);
                    
                    _lifeCycle.AddController(_mainMenuController);

                    break;
                }
            case GameState.Authentication:
                {
                    _authenticationController = new AuthenticationMenuController(_placeForUi, _gamePrefs, _multiplayerService);
                    
                    _lifeCycle.AddController(_authenticationController);

                    break;
                }
            case GameState.EnterLobby:
                {
                    _gameLobbyController = new GameLobbyController(_placeForUi, _gameConfig, _gamePrefs, _netManager);
                    
                    _lifeCycle.AddController(_gameLobbyController);

                   /* _lobbyMenuController = new LobbyMenuController(_placeForUi, _gameConfig, _gamePrefs, _multiplayerService);
                    
                    _lifeCycle.AddController(_lobbyMenuController);*/

                    break;
                }
            case GameState.Exit:
                {
                    ExitFromGame();
                    break;
                }
        }
    }

    private void DisposeControllers()
    {
        _lifeCycle.Dispose();
    }

    private void ExitFromGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void Dispose()
    {
        DisposeControllers();

        _gamePrefs.OnGameStateChange -= GameStateChanged;
    }
}
