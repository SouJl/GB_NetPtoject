using Configs;
using Enumerators;
using GameLobby;
using MultiplayerService;
using Prefs;
using System;
using Tools;
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

    private LoadingScreenController _loadingScreenController;
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
        Subscribe();

        _netManager.Connect();

        _gamePrefs.ChangeGameState(GameState.Loading);
    }

    private void Subscribe()
    {
        _gamePrefs.OnGameStateChange += GameStateChanged;
        _netManager.OnConnectedToServer += Connected;
        _netManager.OnDisConnectedFromServer += Disconnected;
    }

    private void Unsubscribe()
    {
        _gamePrefs.OnGameStateChange -= GameStateChanged;
        _netManager.OnConnectedToServer -= Connected;
        _netManager.OnDisConnectedFromServer -= Disconnected;
    }

    private void Connected()
    {
        var userSate = _gamePrefs.Load();

        if (userSate == false)
        {
            _gamePrefs.ChangeGameState(GameState.Authentication);
        }
        else
        {
            _gamePrefs.ChangeGameState(GameState.MainMenu);
        }
    }

    private void Disconnected()
    {
        _gamePrefs.ChangeGameState(GameState.Exit);
    }

    private void GameStateChanged(GameState state)
    {
        DisposeControllers();
       
        switch (state)
        {
            default:
                break;
            case GameState.Loading:
                {
                    _loadingScreenController = new LoadingScreenController(_placeForUi);

                    _lifeCycle.AddController(_loadingScreenController);

                    break;
                }
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

        Unsubscribe();
    }
}
