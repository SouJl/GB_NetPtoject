using Abstraction;
using Configs;
using Enumerators;
using Photon.Realtime;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    public class GameLobbyMenuController : BaseUIController
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/GameLobbyMenu");

        private readonly GameLobbyMenuUI _view;
        private readonly GameConfig _gameConfig;
        private readonly GamePrefs _gamePrefs;
        private readonly PhotonNetManager _netManager;

        public GameLobbyMenuController(
            Transform placeForUI,
            GameConfig gameConfig,
            GamePrefs gamePrefs,
            PhotonNetManager netManager)
        {
            _gameConfig = gameConfig;
            _gamePrefs = gamePrefs;
            _netManager = netManager;

            _view = LoadView(placeForUI);
            _view.InitUI();

            Subscribe();
        }

        private GameLobbyMenuUI LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<GameLobbyMenuUI>();
        }

        private void Subscribe()
        {
            _view.OnJoinRoomPressed += JoinRoom;
            _view.OnCreateRoomPressed += CreateRoom;
            _view.OnClosePressed += Close;

            _netManager.OnLeftFromLobby += LeftedFromLobby;
            _netManager.OnJoinInRoom += JoinedInRoom;
            _netManager.OnRoomsUpdate += RefreshRoomData;
        }

        private void Unsubscribe()
        {
            _view.OnJoinRoomPressed -= JoinRoom;
            _view.OnCreateRoomPressed -= CreateRoom;
            _view.OnClosePressed -= Close;

            _netManager.OnLeftFromLobby -= LeftedFromLobby;
            _netManager.OnJoinInRoom -= JoinedInRoom;
            _netManager.OnRoomsUpdate -= RefreshRoomData;
        }

        private void JoinRoom(string roomName)
        {
            _netManager.JoinRoom(roomName);

          //  _gamePrefs.ChangeGameState(GameState.Room);
        }

        private void CreateRoom()
        {
            _netManager.CreateRoom();
        }

        private void Close()
        {
            _netManager.LeaveLobby();
        }


        private void LeftedFromLobby()
        {
            _gamePrefs.ChangeGameState(GameState.MainMenu);
        }
        private void JoinedInRoom(Room room)
        {
            _view.ShowInRoom(room.Name);
        }

        private void RefreshRoomData(List<RoomInfo> roomsInfo)
        {
            if (roomsInfo.Count == 0)
                return;

            _view.AddRooms(roomsInfo);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            Unsubscribe();
        }
    }
}
