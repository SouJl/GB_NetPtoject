using Abstraction;
using Enumerators;
using MultiplayerService;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    public class AuthenticationMenuController : BaseUIController, IOnUpdate
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/AuthenticationMenu");

        private readonly AuthenticationMenuView _view;
        private readonly GamePrefs _gamePrefs;
        private readonly IMultiplayerService _multiplayerService;

        private ConnectionProgressController _connectionProgress;

        public AuthenticationMenuController(
            Transform placeForUI, 
            GamePrefs gamePrefs,
            IMultiplayerService multiplayerService)
        {                
            _gamePrefs = gamePrefs;
            _multiplayerService = multiplayerService;

            _view = LoadView(placeForUI);
            _view.InitView();
            
            _connectionProgress = new ConnectionProgressController(_view.ConnetcionProgressPlacement);

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
            _view.SignInUI.OnProceed += LogInToMultiplayerService;
            _multiplayerService.OnLogInSucceed += LogInProccessEndOnSucceed;
            _multiplayerService.OnLogInError += LogInProccessEndError;

            _view.CreateAccountUI.OnProceed += CreateAcountInMultiplayerService;
            _multiplayerService.OnCreateAccountSucceed += CrateAccountEndOnSucceed;
            _multiplayerService.OnCreateAccountError += CrateAccountEndError;
        }
      
        private void Unsubscribe()
        {
            _view.SignInUI.OnProceed -= LogInToMultiplayerService;
            _multiplayerService.OnLogInSucceed -= LogInProccessEndOnSucceed;
            _multiplayerService.OnLogInError -= LogInProccessEndError;

            _view.CreateAccountUI.OnProceed -= CreateAcountInMultiplayerService;    
            _multiplayerService.OnCreateAccountSucceed -= CrateAccountEndOnSucceed;
            _multiplayerService.OnCreateAccountError -= CrateAccountEndError;
        }

        #region LogIn

        private void LogInToMultiplayerService(UserData data)
        {
            _connectionProgress.Start();

            _multiplayerService.LogIn(data);
        }

        private void LogInProccessEndOnSucceed(string playfabId)
        {
            _connectionProgress.Stop();

            _gamePrefs.SetUserId(playfabId);

            _gamePrefs.ChangeGameState(GameState.Lobby);
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

            _multiplayerService.CreateAccount(data);
        }

        private void CrateAccountEndOnSucceed()
        {
            _connectionProgress.Stop();
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
