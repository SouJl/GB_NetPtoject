using Abstraction;
using MultiplayerService;
using PlayFab;
using Prefs;
using UnityEngine;

public class StartupController : BaseController
{
    private readonly IGamePrefs _gamePrefs;
    private readonly DataServerService _serverService;
    private readonly GameNetManager _netManager;

    public StartupController(
        IGamePrefs gamePrefs, 
        DataServerService serverService, 
        GameNetManager netManager)
    {
        _gamePrefs = gamePrefs;
        _serverService = serverService;
        _netManager = netManager;

        _gamePrefs.LoadData();

        Subscribe();

        LoadAuthenticationData(_gamePrefs);
    }

    private void Subscribe()
    {
        _serverService.OnLogInSucceed += UserPassAuthentication;
        _serverService.OnGetUserData += GettedPlayerData;
        _serverService.OnError -= ErrorHandler;

        _netManager.OnConnectedToServer += ConnectedToMainServer;
    }

    private void Unsubscribe()
    {
        _serverService.OnLogInSucceed -= UserPassAuthentication;
        _serverService.OnGetUserData -= GettedPlayerData;
        _serverService.OnError -= ErrorHandler;

        _netManager.OnConnectedToServer -= ConnectedToMainServer;
    }


    private void LoadAuthenticationData(IGamePrefs gamePrefs)
    {
        if (gamePrefs.IsUserDataExist)
        {
            TryLogin(gamePrefs.GetUser());
        }
        else
        {
            _gamePrefs.ChangeGameState(Enumerators.GameState.Authentication);
        }
    }

    private void TryLogin(IGameUser user)
    {
        _serverService.LogIn(user);
    }

    private void UserPassAuthentication(string userId)
    {
        _serverService.GetPlayerData(userId);  
    }

    private void GettedPlayerData(PlayfabPlayerData playerData)
    {
        _gamePrefs.SetPlayer(playerData);

        _netManager.Connect(playerData.Nickname);
    }

    private void ErrorHandler(PlayFabErrorCode errorCode, string errorMessage)
    {
        Debug.Log($"Get error on startup[{errorCode}]: {errorMessage}");
    }


    private void ConnectedToMainServer()
    {
        if (_serverService.IsLogIn)
        {
            _gamePrefs.ChangeGameState(Enumerators.GameState.MainMenu);
        }
    }

    protected override void OnDispose()
    {
        base.OnDispose();

        Unsubscribe();
    }
}
