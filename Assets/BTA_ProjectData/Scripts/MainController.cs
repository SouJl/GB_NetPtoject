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
using Abstraction;
using Authentication;

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

    private readonly DataServerService _dataServerService;

    private MainMenuController _mainMenuController;
    private AuthenticationController _authenticationController;
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
        _dataServerService = new DataServerService(_gameConfig);
        
        _netManager.Init(gameConfig);

        InitialGameLoad();
    }

    private void InitialGameLoad()
    {
        Subscribe();

        _gamePrefs.LoadUser();

        _gamePrefs.ChangeGameState(GameState.Authentication);
    }

    private void Subscribe()
    {
        _gamePrefs.OnGameStateChange += GameStateChanged;
        _netManager.OnDisconnectedFromServer += Disconnected;
    }

    private void Unsubscribe()
    {
        _gamePrefs.OnGameStateChange -= GameStateChanged;
        _netManager.OnDisconnectedFromServer -= Disconnected;
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
                    _authenticationController
                        = new AuthenticationController(_placeForUi, _gamePrefs, _dataServerService, _netManager);

                    _lifeCycle.AddController(_authenticationController);

                    break;
                }
            case GameState.MainMenu:
                {
                    _mainMenuController 
                        = new MainMenuController(_placeForUi, _gamePrefs, _netManager, _stateTransition);
                    
                    _lifeCycle.AddController(_mainMenuController);

                    break;
                }
            case GameState.Lobby:
                {
                    _gameLobbyController 
                        = new GameLobbyController(_placeForUi, _gameConfig, _gamePrefs, _netManager, _stateTransition);
                    
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
        _lifeCycle?.Dispose();
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

    private void Disconnected()
    {
        Debug.Log("Disconnected From Photon Server");
       // _gamePrefs.ChangeGameState(GameState.Exit);
    }


    public void Dispose()
    {
        DisposeControllers();

        Unsubscribe();
    }
}
