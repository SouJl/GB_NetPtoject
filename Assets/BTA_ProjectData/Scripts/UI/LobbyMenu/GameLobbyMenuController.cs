﻿using Abstraction;
using Configs;
using Enumerators;
using Photon.Realtime;
using Prefs;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    public class GameLobbyMenuController : BaseController
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/GameLobbyMenu");

        private readonly LobbyBrowseMenuUI _view;
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
            _view.InitUI(gameConfig);

            Subscribe();
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
            _view.OnHostGamePressed += CreateRoom;
            _view.OnClosePressed += Close;

            _netManager.OnLeftFromLobby += LeftedFromLobby;
            _netManager.OnRoomsUpdate += RefreshRoomData;
        }

        private void Unsubscribe()
        {
            _view.OnJoinRoomPressed -= JoinRoom;
            _view.OnHostGamePressed -= CreateRoom;
            _view.OnClosePressed -= Close;

            _netManager.OnLeftFromLobby -= LeftedFromLobby;
            _netManager.OnRoomsUpdate -= RefreshRoomData;
        }

        private void JoinRoom(string roomName)
        {
            _netManager.JoinRoom(roomName);

            _gamePrefs.ChangeGameState(GameState.Room);
        }

        private void CreateRoom()
        {
           // _netManager.CreateRoom(data);

            _gamePrefs.ChangeGameState(GameState.Room);
        }

        private void Close()
        {
            _netManager.LeaveLobby();
        }

        private void LeftedFromLobby()
        {
            _gamePrefs.ChangeGameState(GameState.MainMenu);
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
