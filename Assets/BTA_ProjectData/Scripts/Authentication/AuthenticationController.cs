using Abstraction;
using Enumerators;
using MultiplayerService;
using Prefs;
using UnityEngine;

namespace Authentication
{
    public class AuthenticationController : BaseController, IOnUpdate
    {
        private readonly Transform _placeForUI;
        private readonly IGamePrefs _gamePrefs;
        private readonly DataServerService _dataServerService;
        private readonly GameNetManager _netManager;

        private readonly AuthenticationPrefs _prefs;

        private BaseController _currentController;

        public AuthenticationController(
            Transform placeForUI, 
            IGamePrefs gamePrefs,
            DataServerService dataServerService,
            GameNetManager netManager) 
        {
            _placeForUI = placeForUI;
            _gamePrefs = gamePrefs;
            _dataServerService = dataServerService;
            _netManager = netManager;

            _prefs = new AuthenticationPrefs();

            _prefs.OnStateChange += StateChanged;

            if (_gamePrefs.IsUserDataExist)
            {
                var user = _gamePrefs.GetUser();
                
                _prefs.SetUserdata(user);

                _prefs.ChangeState(AuthenticationState.SigIn);
            }
            else
            {
                _prefs.ChangeState(AuthenticationState.DataNotFound);
            }
        }

        private void StateChanged(AuthenticationState state)
        {
            DisposeCurrentController();
            
            switch (state)
            {
                case AuthenticationState.SigIn:
                    {
                        _currentController = new SignInController(_placeForUI, _prefs, _dataServerService, _netManager);
                        break;
                    }
                case AuthenticationState.DataNotFound:
                    {
                        _currentController = new NoDataFoundController(_placeForUI, _prefs);
                        break;
                    }
                case AuthenticationState.Register:
                    {
                        _currentController = new RegisterController(_placeForUI, _prefs, _dataServerService);
                        break;
                    }
                case AuthenticationState.ToMainMenu:
                    {
                        GoToMainMenu();
                        break;
                    }
            }
        }

        private void DisposeCurrentController()
        {
            _currentController?.Dispose();
        }

        private void GoToMainMenu()
        {
            _gamePrefs.SetUser(_prefs.UserData);
            _gamePrefs.SetPlayer(_prefs.PlayerData);

            _gamePrefs.ChangeGameState(GameState.MainMenu);
        }

        public void ExecuteUpdate(float deltaTime)
        {
            if(_currentController is IOnUpdate onUpdate)
            {
                onUpdate.ExecuteUpdate(deltaTime);
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            DisposeCurrentController();

            _prefs.OnStateChange -= StateChanged;
        }
    }
}
