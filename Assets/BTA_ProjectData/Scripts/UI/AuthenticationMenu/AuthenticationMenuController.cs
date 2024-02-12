﻿using Abstraction;
using Enumerators;
using MultiplayerService;
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
            _view.InitView(_gamePrefs.IsUserDataExist, _gamePrefs.Data);

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
            _dataServerService.OnLogInSucceed += LogInProccessEndOnSucceed;
            _dataServerService.OnError += LogInProccessEndError;

            _view.CreateAccountUI.OnProceed += CreateAcountInMultiplayerService;
            _dataServerService.OnCreateAccountSucceed += CrateAccountEndOnSucceed;
            _dataServerService.OnError += CrateAccountEndError;
        }

        private void Unsubscribe()
        {
            _view.OnEnterTheLobby -= EtnterUserInLobby;
            _view.OnLogOut -= LogOutUser;

            _view.SignInUI.OnProceed -= LogInToMultiplayerService;
            _dataServerService.OnLogInSucceed -= LogInProccessEndOnSucceed;
            _dataServerService.OnError -= LogInProccessEndError;

            _view.CreateAccountUI.OnProceed -= CreateAcountInMultiplayerService;
            _dataServerService.OnCreateAccountSucceed -= CrateAccountEndOnSucceed;
            _dataServerService.OnError -= CrateAccountEndError;
        }

        private void EtnterUserInLobby()
        {
            _dataServerService.LogIn(_gamePrefs.Data);

            _connectionProgress.Start();
        }

        private void LogOutUser()
        {
            _gamePrefs.DeleteData();

            _gamePrefs.ChangeGameState(GameState.Authentication);
        }

        #region LogIn

        private void LogInToMultiplayerService(UserData data)
        {
            _connectionProgress.Start();

            _dataServerService.LogIn(data);
        }

        private void LogInProccessEndOnSucceed(UserData data)
        {
            _connectionProgress.Stop();

            _gamePrefs.SetUserData(data);

            var userData = new Dictionary<string, string>()
            {
                {BTAConst.USER_NICKNAME, $"{data.UserName}"},
                {BTAConst.USER_GAME_LVL, $"{1}"},
                {BTAConst.USER_LVL_PROGRESS, $"{0}"},
            };

            _dataServerService.SetUserData(userData);

            _gamePrefs.ChangeGameState(GameState.MainMenu);
        }

        private void LogInProccessEndError(string errorMessage)
        {
            _connectionProgress.Stop();

            Debug.LogError($"Get error in LogIn proccess: {errorMessage}");
        }

        #endregion

        #region Create Account

        private void CreateAcountInMultiplayerService(UserData data)
        {
            _connectionProgress.Start();

            _dataServerService.CreateAccount(data);
        }

        private void CrateAccountEndOnSucceed(UserData data)
        {
            _connectionProgress.Stop();

            _gamePrefs.SetUserData(data);

            _gamePrefs.ChangeGameState(GameState.MainMenu);
        }

        private void CrateAccountEndError(string errorMessage)
        {
            _connectionProgress.Stop();

            Debug.LogError($"Get error on account creation: {errorMessage}");
        }

        #endregion

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
