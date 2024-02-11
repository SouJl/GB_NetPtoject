using Configs;
using Enumerators;
using GameLobby;
using MultiplayerService;
using Prefs;
using System;
using Tools;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

#if UNITY_EDITOR
using ParrelSync;
#endif

public class MainController : IDisposable
{
    private readonly Transform _placeForUi;
    private readonly GameConfig _gameConfig;
    private readonly LifeCycleController _lifeCycle;
    private readonly GameNetManager _netManager;
    private readonly StateTransition _stateTransition;

    private readonly IGamePrefs _gamePrefs;

    private readonly IMultiplayerService _multiplayerService;

    private LoadingScreenController _loadingScreenController;
    private MainMenuController _mainMenuController;
    private AuthenticationMenuController _authenticationController;
    private GameLobbyController _gameLobbyController;

    public MainController(
        Transform placeForUi, 
        GameConfig gameConfig, 
        LifeCycleController lifeCycle,
        GameNetManager netManager,
        StateTransition stateTransition)
    {
        _placeForUi = placeForUi;
        _gameConfig = gameConfig;
        _lifeCycle = lifeCycle;
        _netManager = netManager;
        _stateTransition = stateTransition;

#if UNITY_EDITOR
        if (ClonesManager.IsClone())
        {
            _gamePrefs = new ClonedGamePrefs();
        }
        else
        {
            _gamePrefs = new GamePrefs();
        }
#else
        _gamePrefs = new GamePrefs();
#endif
        _multiplayerService = new PlayFabMultiplayerService(_gameConfig);
        
        _netManager.Init(gameConfig);

        InitialGameLoad();
    }

    private void InitialGameLoad()
    {
        Subscribe();

        var userSate = _gamePrefs.Load();

        if (userSate == false)
        {
            _gamePrefs.ChangeGameState(GameState.Authentication);
        }
        else
        {
            _gamePrefs.ChangeGameState(GameState.Loading);
        }      
    }

    private void Subscribe()
    {
        _gamePrefs.OnGameStateChange += GameStateChanged;
        _netManager.OnDisConnectedFromServer += Disconnected;
    }

    private void Unsubscribe()
    {
        _gamePrefs.OnGameStateChange -= GameStateChanged;
        _netManager.OnDisConnectedFromServer -= Disconnected;
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
                    _loadingScreenController 
                        = new LoadingScreenController(
                            _placeForUi,
                            _gamePrefs,  
                            _netManager, 
                            _stateTransition);

                    _lifeCycle.AddController(_loadingScreenController);

                    _loadingScreenController.Start();

                    break;
                }
            case GameState.MainMenu:
                {
                    _mainMenuController = new MainMenuController(_placeForUi, _gamePrefs, _netManager, _stateTransition);
                    
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
                    _gameLobbyController = new GameLobbyController(_placeForUi, _gameConfig, _gamePrefs, _netManager, _stateTransition);
                    
                    _lifeCycle.AddController(_gameLobbyController);

                    break;
                }
            case GameState.Game:
                {
                    LoadGame();
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


    private void LoadGame()
    {
        Dispose();

        PhotonNetwork.LoadLevel(1);
    }


    private void ExitFromGame()
    {
        Dispose();

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
