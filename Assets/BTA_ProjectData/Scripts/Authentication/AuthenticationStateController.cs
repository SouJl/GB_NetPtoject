using Abstraction;
using Enumerators;
using Prefs;
using UnityEngine;

namespace Authentication
{
    public class AuthenticationStateController : BaseController
    {
        private readonly Transform _placeForUI;
        private readonly IGamePrefs _gamePrefs;

        private readonly AuthenticationPrefs _prefs;

        private CreateAccountController _createAccountController;

        public AuthenticationStateController(
            Transform placeForUI, 
            IGamePrefs gamePrefs)
        {
            _placeForUI = placeForUI;
            _gamePrefs = gamePrefs;

            _prefs = new AuthenticationPrefs();

            _prefs.OnStateChange += StateChanged;
        }

        private void StateChanged(AuthenticationState state)
        {
            DisposeAllControllers();
            
            switch (state)
            {
                case AuthenticationState.Main:
                    {
                        break;
                    }
                case AuthenticationState.CreateAccount:
                    {
                        _createAccountController = new CreateAccountController(_placeForUI, _prefs);
                        break;
                    }
            }
        }

        private void DisposeAllControllers()
        {
            _createAccountController?.Dispose();
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            DisposeAllControllers();
            
            _prefs.OnStateChange -= StateChanged;
        }
    }
}
