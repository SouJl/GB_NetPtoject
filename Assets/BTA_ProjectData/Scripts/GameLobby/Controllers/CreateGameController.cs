using Abstraction;
using Configs;
using Enumerators;
using MultiplayerService;
using Prefs;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameLobby
{
    public class CreateGameController : BaseController
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/CreateGameMenu");

        private readonly CreateGameUI _view;
        private readonly GameConfig _gameConfig;
        private readonly GameLobbyPrefs _lobbyPrefs;
        private readonly GameNetManager _netManager;
        private readonly StateTransition _stateTransition;

        public CreateGameController(
            Transform placeForUI,
            GameConfig gameConfig,
            GameLobbyPrefs lobbyPrefs,
            GameNetManager netManager,
            StateTransition stateTransition)
        {
            _gameConfig = gameConfig;
            _lobbyPrefs = lobbyPrefs;
            _netManager = netManager;
            _stateTransition = stateTransition;

            _view = LoadView(placeForUI);

            _view.InitUI(gameConfig.RoomMinPlayers, gameConfig.RoomMaxPlayers);

            Subscribe();
        }

        private CreateGameUI LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<CreateGameUI>();
        }

        private void Subscribe()
        {
            _view.OnCreateRoomPressed += CreateRoom;
            _view.OnBackPressed += BackToLobbyBrowse;
        }

        private void Unsubscribe()
        {
            _view.OnCreateRoomPressed -= CreateRoom;
            _view.OnBackPressed -= BackToLobbyBrowse;
        }

        private void CreateRoom(CreationRoomData creationData)
        {
            _lobbyPrefs.ClickSound.Play();

            _lobbyPrefs.SetRoomData(creationData, true);
            _stateTransition.Invoke(() => _lobbyPrefs.ChangeState(GameLobbyState.InRoom));
        }

        private void BackToLobbyBrowse()
        {
            _lobbyPrefs.ClickSound.Play();

            _lobbyPrefs.ChangeState(GameLobbyState.Browse);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            Unsubscribe();
        }
    }
}
