using Abstraction;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    public class MainMenuController : BaseUIController
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/MainMenu");

        private readonly GamePrefs _gamePrefs;
        private readonly PhotonNetManager _netManager;
        private readonly MainMenuUI _view;

        public MainMenuController(
            Transform placeForUI, 
            GamePrefs gamePrefs, 
            PhotonNetManager netManager)
        {
            _gamePrefs = gamePrefs;
            _netManager = netManager;

            _view = LoadView(placeForUI);

            _view.InitUI(gamePrefs.UserName);

            Subscribe();

            _netManager.Connect();
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
            _view.OnCreateGamePressed += CreateGame;
            _view.OnExitGamePressed += ExitGame;
        }

        private void Unsubscribe()
        {
            _view.OnSwitchUserPressed -= SwitchUser;
            _view.OnJoinGamePressed -= JoinGame;
            _view.OnCreateGamePressed -= CreateGame;
            _view.OnExitGamePressed -= ExitGame;
        }


        private void SwitchUser()
        {
            _gamePrefs.DeleteData();

            _gamePrefs.ChangeGameState(Enumerators.GameState.Authentication);
        }


        private void JoinGame()
        {
            _gamePrefs.ChangeGameState(Enumerators.GameState.Lobby);
        }

        private void CreateGame()
        {
            Debug.Log("CreateGame");
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
