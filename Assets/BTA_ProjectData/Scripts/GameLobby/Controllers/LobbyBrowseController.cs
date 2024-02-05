using Abstraction;
using Configs;
using Enumerators;
using MultiplayerService;
using Photon.Realtime;
using Prefs;
using System.Collections.Generic;
using Tools;
using UI;
using UnityEngine;

namespace GameLobby
{
    public class LobbyBrowseController : BaseController
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/LobbyBrowseMenu");

        private readonly LobbyBrowseMenuUI _view;
        private readonly GameConfig _gameConfig;
        private readonly GameLobbyPrefs _lobbyPrefs;
        private readonly GameNetManager _netManager;
        private readonly StateTransition _stateTransition;


        private List<RoomInfo> _lobbyRoomsInfoCollection = new();

        public LobbyBrowseController(
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
            var room = _lobbyRoomsInfoCollection.Find(r => r.Name == roomName);

            if (!room.IsOpen)
            {
                Debug.LogWarning($"Romm {roomName} is closed");
                return;
            }

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
            _view.Show();
        }

        private void RefreshRoomData(List<RoomInfo> roomsInfo)
        {
            if (roomsInfo.Count == 0)
                return;

            _lobbyRoomsInfoCollection.Clear();

            for (int i =0; i< roomsInfo.Count; i++)
            {
                var roomInfo = roomsInfo[i];

                if (roomInfo.RemovedFromList)
                    continue;

                _lobbyRoomsInfoCollection.Add(roomInfo);
            }

            _view.AddRooms(_lobbyRoomsInfoCollection);
        }


        private void JoinInLobby()
        {
            _netManager.JoinLobby();
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            Unsubscribe();

        }
    }
}
