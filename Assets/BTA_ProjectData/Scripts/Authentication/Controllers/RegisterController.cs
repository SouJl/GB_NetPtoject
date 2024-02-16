using Abstraction;
using Authentication.View;
using Enumerators;
using MultiplayerService;
using Prefs;
using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace Authentication
{
    public class RegisterController : BaseController
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/Authentication/RegisterAccountUI");

        private readonly RegisterAccountUI _view;
        private readonly AuthenticationPrefs _prefs;
        private readonly DataServerService _dataServerService;
        public RegisterController(
            Transform placeForUI, 
            AuthenticationPrefs prefs,
            DataServerService dataServerService)
        {
            _view = LoadView(placeForUI);
            _view.InitUI();

            _prefs = prefs;
            _dataServerService = dataServerService;

            Subscribe();
        }

        private RegisterAccountUI LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<RegisterAccountUI>();
        }

        private void Subscribe()
        {
            _view.OnCreateAccount += RegisterAccount;
            _view.OnBack += ReturnBack;

            _dataServerService.OnCreateAccountSucceed += AccountCreated;
            _dataServerService.OnSetDataSucceed += PlayerDataSetted;
        }

        private void Unsubscribe()
        {
            _view.OnCreateAccount -= RegisterAccount;
            _view.OnBack -= ReturnBack;

            _dataServerService.OnCreateAccountSucceed -= AccountCreated;
            _dataServerService.OnSetDataSucceed -= PlayerDataSetted;
        }

        private void RegisterAccount(UserAccountData data)
        {
            _prefs.SetUserdata(new BaseGameUser
            {
                Name = data.Name,
                Password = data.Password
            }); 

            _dataServerService.CreateAccount(data); 
        }

        private void ReturnBack()
        {
            _prefs.ChangeState(AuthenticationState.DataNotFound);
        }

        private void AccountCreated(string userId)
        {
            var userData = new Dictionary<string, string>()
            {
                {BTAConst.USER_NICKNAME, $"{_prefs.UserData.Name}"},
                {BTAConst.USER_GAME_LVL, $"{1}"},
                {BTAConst.USER_LVL_PROGRESS, $"{0}"},
            };

            _dataServerService.SetPlayerData(userData);
        }

        private void PlayerDataSetted()
        {
            _prefs.ChangeState(AuthenticationState.SigIn);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            _view?.Dispose();

            Unsubscribe();
        }
    }
}
