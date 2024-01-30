using Abstraction;
using Configs;
using Enumerators;
using Prefs;
using Tools;
using UnityEngine;

namespace GameLobby
{
    public class GameLobbyController : BaseController, IOnUpdate
    {
        private readonly Transform _placeForUi;
        private readonly GameConfig _gameConfig;
        private readonly GamePrefs _gamePrefs;
        private readonly PhotonNetManager _netManager;
        private readonly GameLobbyPrefs _lobbyPrefs;

        private LoadingScreenController _loadingScreenController;
        private LobbyBrowseController _lobbyBrowseController;
        private CreateRoomController _createRoomController;
        private InRoomController _inRoomController;

        public GameLobbyController(
            Transform placeForUI,
            GameConfig gameConfig,
            GamePrefs gamePrefs,
            PhotonNetManager netManager)
        {
            _placeForUi = placeForUI;
            _gameConfig = gameConfig;
            _gamePrefs = gamePrefs;
            _netManager = netManager;

            _lobbyPrefs = new GameLobbyPrefs();

            _netManager.OnJoinInLobby += JoinedInLobby;
            _lobbyPrefs.OnStateChange += LobbyStateChanged;
            
            _netManager.JoinLobby();

            _lobbyPrefs.ChangeState(GameLobbyState.Loading);
        }

        private void JoinedInLobby()
        {
            _lobbyPrefs.ChangeState(GameLobbyState.Browse);
        }

        private void LobbyStateChanged(GameLobbyState state)
        {
            DisposeControllers();

            switch (state)
            {
                default:
                    break;
                case GameLobbyState.Loading:
                    {
                        _loadingScreenController = new LoadingScreenController(_placeForUi, LoadingScreenType.LobbyLoading);

                        break;
                    }
                case GameLobbyState.Browse:
                    {
                        _lobbyBrowseController
                            = new LobbyBrowseController(_placeForUi, _gameConfig, _lobbyPrefs, _netManager);
                        break;
                    }
                case GameLobbyState.CreateRoom:
                    {
                        _createRoomController 
                            = new CreateRoomController(_placeForUi, _gameConfig, _lobbyPrefs, _netManager);
                        break;
                    }
                case GameLobbyState.InRoom:
                    {
                        _inRoomController 
                            = new InRoomController(_placeForUi, _gameConfig, _lobbyPrefs, _netManager);
                        break;
                    }
                case GameLobbyState.Exit:
                    {
                        ExitFromLobby();
                        break;
                    }
            }
        }

        private void DisposeControllers()
        {
            _loadingScreenController?.Dispose();
            _lobbyBrowseController?.Dispose();
            _createRoomController?.Dispose();
            _inRoomController?.Dispose();
        }

        private void ExitFromLobby()
        {
            _netManager.OnLeftFromLobby += LeftedFromLobby;
            
            _netManager.LeaveLobby();
        }

        private void LeftedFromLobby()
        {
            _netManager.OnLeftFromLobby -= LeftedFromLobby;

            _gamePrefs.ChangeGameState(GameState.MainMenu);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            DisposeControllers();

            _netManager.OnJoinInLobby -= JoinedInLobby;
            _lobbyPrefs.OnStateChange -= LobbyStateChanged;
        }

        public void ExecuteUpdate(float deltaTime)
        {
            if(_loadingScreenController != null)
            {
                _loadingScreenController.ExecuteUpdate(deltaTime);
            }
        }
    }
}
