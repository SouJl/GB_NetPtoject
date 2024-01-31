using Abstraction;
using Configs;
using Enumerators;
using Photon.Realtime;
using Prefs;
using System.Collections.Generic;
using Tools;
using UI;
using UnityEngine;

namespace GameLobby
{
    public class LobbyBrowseController : BaseController, IOnUpdate
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/LobbyBrowseMenu");

        private readonly LobbyBrowseMenuUI _view;
        private readonly GameConfig _gameConfig;
        private readonly GameLobbyPrefs _lobbyPrefs;
        private readonly PhotonNetManager _netManager;
        private readonly StateTransition _stateTransition;

        private readonly LoadingScreenController _loadingScreenController;

        public LobbyBrowseController(
           Transform placeForUI,
           GameConfig gameConfig,
           GameLobbyPrefs lobbyPrefs,
           PhotonNetManager netManager,
           StateTransition stateTransition)
        {
            _gameConfig = gameConfig;
            _lobbyPrefs = lobbyPrefs;
            _netManager = netManager;
            _stateTransition = stateTransition;

            _loadingScreenController = new LoadingScreenController(placeForUI, LoadingScreenType.LobbyLoading);

            _view = LoadView(placeForUI);
            _view.InitUI(gameConfig);

            Subscribe();

            JoinInLobby();
        }

        private LobbyBrowseMenuUI LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<LobbyBrowseMenuUI>();
        }

        private void Subscribe()
        {
            _view.OnJoinRoomPressed += JoinRoom;
            _view.OnHostGamePressed += OpenHostGameMenu;
            _view.OnClosePressed += Close;

            _netManager.OnJoinInLobby += JoinedInLobby;
            _netManager.OnRoomsUpdate += RefreshRoomData;
        }

        private void Unsubscribe()
        {
            _view.OnJoinRoomPressed -= JoinRoom;
            _view.OnHostGamePressed -= OpenHostGameMenu;
            _view.OnClosePressed -= Close;
          
            _netManager.OnJoinInLobby -= JoinedInLobby;
            _netManager.OnRoomsUpdate -= RefreshRoomData;
        }

        private void JoinRoom(string roomName)
        {
            _lobbyPrefs.SetRoomData(new CreationRoomData
            {
                RoomName = roomName
            });

            _stateTransition.Invoke(() => _lobbyPrefs.ChangeState(GameLobbyState.InRoom)); 
        }

        private void OpenHostGameMenu()
        {
            _lobbyPrefs.ChangeState(GameLobbyState.CreateRoom);
        }

        private void Close()
        {
            _lobbyPrefs.ChangeState(GameLobbyState.Exit);
        }

        private void JoinedInLobby()
        {
            _loadingScreenController.Stop();
            _view.Show();
        }

        private void RefreshRoomData(List<RoomInfo> roomsInfo)
        {
            if (roomsInfo.Count == 0)
                return;

            _view.AddRooms(roomsInfo);
        }


        private void JoinInLobby()
        {
            _netManager.JoinLobby();
            _loadingScreenController.Start();
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            Unsubscribe();

            _loadingScreenController?.Dispose();
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _loadingScreenController.ExecuteUpdate(deltaTime);
        }
    }
}
