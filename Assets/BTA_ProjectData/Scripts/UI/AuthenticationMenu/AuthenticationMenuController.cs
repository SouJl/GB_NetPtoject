using Abstraction;
using Enumerators;
using MultiplayerService;
using PlayFab;
using Prefs;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    public class AuthenticationMenuController : BaseController, IOnUpdate
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/AuthenticationMenu");

        private readonly AuthenticationMenuView _view;
        private readonly IGamePrefs _gamePrefs;
        private readonly DataServerService _dataServerService;

        private ProgressController _connectionProgress;

        public AuthenticationMenuController(
            Transform placeForUI,
            IGamePrefs gamePrefs,
            DataServerService dataServerService)
        {
            _gamePrefs = gamePrefs;
            _dataServerService = dataServerService;

            _view = LoadView(placeForUI);
            _view.InitView(_gamePrefs.IsUserDataExist, _gamePrefs.GetUser());

            _connectionProgress = new ProgressController(_view.ConnetcionProgressPlacement);

            Subscribe();
        }

        private AuthenticationMenuView LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<AuthenticationMenuView>();
        }

        private void Subscribe()
        {
            _view.OnEnterTheLobby += EtnterUserInLobby;
            _view.OnLogOut += LogOutUser;
            _view.SignInUI.OnProceed += LogInToMultiplayerService;

            _view.CreateAccountUI.OnProceed += CreateAcountInMultiplayerService;

            _dataServerService.OnCreateAccountSucceed += CrateAccountEndOnSucceed;
            _dataServerService.OnSetDataSucceed += UserDataSetted;
            _dataServerService.OnError += AuthEndByError;
        }

        private void Unsubscribe()
        {
            _view.OnEnterTheLobby -= EtnterUserInLobby;
            _view.OnLogOut -= LogOutUser;
            _view.SignInUI.OnProceed -= LogInToMultiplayerService;

            _view.CreateAccountUI.OnProceed -= CreateAcountInMultiplayerService;

            _dataServerService.OnCreateAccountSucceed -= CrateAccountEndOnSucceed;
            _dataServerService.OnSetDataSucceed -= UserDataSetted;
            _dataServerService.OnError -= AuthEndByError;
        }


        private void EtnterUserInLobby()
        {
            _dataServerService.LogIn(_gamePrefs.GetUser());

            _connectionProgress.Start();
        }

        private void LogOutUser()
        {
            _gamePrefs.DeleteData();

            _gamePrefs.ChangeGameState(GameState.Authentication);
        }

  

        private void LogInToMultiplayerService(UserAccountData data)
        {
            //_gamePrefs.SetUserData(data);

            _gamePrefs.ChangeGameState(GameState.Loading);
        }

    
        private void CreateAcountInMultiplayerService(UserAccountData data)
        {
            _connectionProgress.Start();

            _dataServerService.CreateAccount(data);
        }

        private void CrateAccountEndOnSucceed(string userId)
        {
            _connectionProgress.Stop();

            var user = _gamePrefs.GetUser();

            var userData = new Dictionary<string, string>()
            {
                {BTAConst.USER_NICKNAME, $"{user.Name}"},
                {BTAConst.USER_GAME_LVL, $"{1}"},
                {BTAConst.USER_LVL_PROGRESS, $"{0}"},
            };

            _dataServerService.SetUserData(userData);
        }

        private void UserDataSetted()
        {
            _gamePrefs.ChangeGameState(GameState.Loading);
        }

        private void AuthEndByError(PlayFabErrorCode errorCode, string errorMessage)
        {
            _connectionProgress.Stop();

            Debug.LogError($"Get error in authentication proccess: {errorMessage}");

            _gamePrefs.ChangeGameState(GameState.Authentication);
        }

 
        public void ExecuteUpdate(float deltaTime)
        {
            _connectionProgress.ExecuteUpdate(deltaTime);
        }

        protected override void OnDispose()
        {
            Unsubscribe();
        }
    }
}
