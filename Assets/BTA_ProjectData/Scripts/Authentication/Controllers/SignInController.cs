using Abstraction;
using Authentication.View;
using Enumerators;
using MultiplayerService;
using PlayFab;
using Prefs;
using Tools;
using UnityEngine;

namespace Authentication
{
    public class SignInController : BaseController, IOnUpdate
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/Authentication/SigInUI");

        private readonly AuthenticationPrefs _prefs;
        private readonly DataServerService _dataServerService;
        private readonly GameNetManager _netManager;

        private readonly SigInUI _view;
        
        private ProgressController _connectionProgress;

        public SignInController(
            Transform _placeForUI, 
            AuthenticationPrefs prefs,
            DataServerService dataServerService,
            GameNetManager netManager)
        {
            _prefs = prefs;
            _dataServerService = dataServerService;
            _netManager = netManager;

            _view = LoadView(_placeForUI);
            _view.InitUI();

            _connectionProgress = new ProgressController(_view.ConnectionProgress);

            Subscribe();

            TryLogin(_prefs.UserData);
        }

        private SigInUI LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<SigInUI>();
        }

        private void Subscribe()
        {
            _dataServerService.OnLogInSucceed += UserPassAuthentication;
            _dataServerService.OnGetUserData += GettedPlayerData;
            _dataServerService.OnError -= ErrorHandler;

            _netManager.OnConnectedToServer += ConnectedToMainServer;
        }

        private void Unsubscribe()
        {
            _dataServerService.OnLogInSucceed -= UserPassAuthentication;
            _dataServerService.OnGetUserData -= GettedPlayerData;
            _dataServerService.OnError -= ErrorHandler;

            _netManager.OnConnectedToServer -= ConnectedToMainServer;
        }


        private void TryLogin(IGameUser user)
        {
            _dataServerService.LogIn(user);
            _view.UpdateConnectionState("CONNECT TO PLAYFAB");
            _connectionProgress.Start();
        }

        private void UserPassAuthentication(string userId)
        {
            _dataServerService.GetPlayerData(userId);
            _view.UpdateConnectionState("LOAD PLAYER DATA");
        }

        private void GettedPlayerData(PlayfabPlayerData playerData)
        {
            _prefs.SetPlayerData(playerData);

            _netManager.Connect(playerData.Nickname);
            _view.UpdateConnectionState("CONNECT TO PHOTON");
        }

        private void ConnectedToMainServer()
        {
            _connectionProgress.Stop();

            if (_dataServerService.IsLogIn)
            {
                _prefs.ChangeState(AuthenticationState.ToMainMenu);
            }
        }

        private void ErrorHandler(PlayFabErrorCode errorCode, string errorMessage)
        {
            Debug.Log($"Get error on startup[{errorCode}]: {errorMessage}");
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _connectionProgress.ExecuteUpdate(deltaTime);
        }

        protected override void OnDispose()
        {
            _view?.Dispose();

            _connectionProgress?.Dispose();

            Unsubscribe();

            base.OnDispose();
        }
    }
}
