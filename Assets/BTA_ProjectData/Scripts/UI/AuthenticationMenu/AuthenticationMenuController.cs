using Abstraction;
using Configs;
using Enumerators;
using PlayFab;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    public class AuthenticationMenuController : BaseUIController, IOnUpdate
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/AuthenticationMenu");

        private readonly AuthenticationMenuView _view;
        public readonly GamePrefs _gamePrefs;
        private ConnectionProgressController _connectionProgress;

        public AuthenticationMenuController(Transform placeForUI, GameConfig gameConfig, GamePrefs gamePrefs)
        {
            _view = LoadView(placeForUI);
            
            _gamePrefs = gamePrefs;

            _view.InitView();
            
            InitializeServie(gameConfig._PlayFabTitleId);
     
            _connectionProgress = new ConnectionProgressController(_view.ConnetcionProgressPlacement);
            _view.SignInUI.OnConnectionStart += ConnectionStart;
            _view.SignInUI.OnConnectionEnd += ConnectionEnd;
        }

        private AuthenticationMenuView LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);
            
            AddGameObject(objectView);

            return objectView.GetComponent<AuthenticationMenuView>();
        }

        private void InitializeServie(string titleId)
        {
            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            {
                PlayFabSettings.staticSettings.TitleId = titleId;
            }
        }

        private void ConnectionStart()
        {
            _connectionProgress.Start();
        }
        private void ConnectionEnd()
        {
            _connectionProgress.Stop();

            _gamePrefs.ChangeGameState(GameState.Lobby);
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _connectionProgress.ExecuteUpdate(deltaTime);
        }

        protected override void OnDispose()
        {
            _view.SignInUI.OnConnectionStart -= ConnectionStart;
            _view.SignInUI.OnConnectionEnd -= ConnectionEnd;
        }
    }
}
