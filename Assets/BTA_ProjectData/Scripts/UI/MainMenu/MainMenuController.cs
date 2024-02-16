using Abstraction;
using MultiplayerService;
using Prefs;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    public class MainMenuController : BaseController
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/MainMenu");

        private readonly IGamePrefs _gamePrefs;
        private readonly GameNetManager _netManager;
        private readonly MainMenuUI _view;
        private readonly StateTransition _stateTransition;
        public MainMenuController(
            Transform placeForUI, 
            IGamePrefs gamePrefs, 
            GameNetManager netManager,
            StateTransition stateTransition)
        {
            _gamePrefs = gamePrefs;
            _netManager = netManager;
            _stateTransition = stateTransition;

            _view = LoadView(placeForUI);

            var user = gamePrefs.GetUser();

            _view.InitUI(user.Name);

            _netManager.SetUserData(user);
            
            Subscribe();
        }

        private MainMenuUI LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<MainMenuUI>();
        }

        private void Subscribe()
        {
            _view.OnSwitchUserPressed += SwitchUser;
            _view.OnJoinGamePressed += JoinGame;
            _view.OnConnectToGame += ConnectToGame;
            _view.OnExitGamePressed += ExitGame;
        }

        private void Unsubscribe()
        {
            _view.OnSwitchUserPressed -= SwitchUser;
            _view.OnJoinGamePressed -= JoinGame;
            _view.OnConnectToGame -= ConnectToGame;
            _view.OnExitGamePressed -= ExitGame;
        }

        private void SwitchUser()
        {
            _gamePrefs.DeleteData();

            _netManager.Disconnect();

            _stateTransition.Invoke(
                () => _gamePrefs.ChangeGameState(Enumerators.GameState.Authentication));
        }

        private void JoinGame()
        {
            _gamePrefs.ChangeGameState(Enumerators.GameState.Lobby);
        }

        private void ConnectToGame(string name)
        {
            _gamePrefs.SetGame(name);

            _gamePrefs.ChangeGameState(Enumerators.GameState.Lobby);
        }

        private void ExitGame()
        {
            _gamePrefs.ChangeGameState(Enumerators.GameState.Exit);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            Unsubscribe();
        }
    }
}
