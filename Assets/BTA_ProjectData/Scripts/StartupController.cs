using Abstraction;
using MultiplayerService;
using Prefs;

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

        Subscribe();

        LoadAuthenticationData(_gamePrefs);
    }

    private void Subscribe()
    {
        _serverService.OnLogInSucceed += UserPassAuthentication;
        _serverService.OnGetUserData += GettedUserData;
    }



    private void Unsubscribe()
    {
        _serverService.OnLogInSucceed -= UserPassAuthentication;

    }

    private void LoadAuthenticationData(IGamePrefs gamePrefs)
    {
        gamePrefs.LoadUser();

        if (gamePrefs.IsUserDataExist)
        {
            TryLogin(gamePrefs.GetUser());
        }
    }

    private void TryLogin(IGameUser user)
    {
        _serverService.LogIn(user);
    }

    private void UserPassAuthentication(string userId)
    {
        _serverService.GetUserData(userId);
    }

    private void GettedUserData(PlayfabPlayerData userData)
    {
       
    }

    protected override void OnDispose()
    {
        base.OnDispose();

        Unsubscribe();
    }
}
