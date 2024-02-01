using Abstraction;
using Configs;
using Enumerators;
using Photon.Realtime;
using Prefs;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameLobby
{
    public class InRoomController : BaseController
    {
        private readonly ResourcePath _viewOwnerPath = new ResourcePath("Prefabs/UI/InRoomOwnerMenu");
        private readonly ResourcePath _viewClientPath = new ResourcePath("Prefabs/UI/InRoomClientMenu");

        private readonly Transform _placeForUI;
        private readonly GameConfig _gameConfig;
        private readonly GameLobbyPrefs _lobbyPrefs;
        private readonly PhotonNetManager _netManager;

        private IRoomMenuUI _view;

        private List<Player> _playersInRoom;
        private Room _roomData;

        public InRoomController(
            Transform placeForUI,
            GameConfig gameConfig,
            GameLobbyPrefs lobbyPrefs,
            PhotonNetManager netManager)
        {
            _placeForUI = placeForUI;
            _gameConfig = gameConfig;
            _lobbyPrefs = lobbyPrefs;
            _netManager = netManager;

            var prefabPath = lobbyPrefs.IsNeedRoomCreation 
                ? _viewOwnerPath 
                : _viewClientPath;

            _view = LoadView(_placeForUI, prefabPath);

            Subscribe();

            InitRoomData(_lobbyPrefs);        
        }

        private void InitRoomData(GameLobbyPrefs lobbyPrefs)
        {
            if (lobbyPrefs.IsNeedRoomCreation)
            {
                _netManager.CreateRoom(new CreationRoomData 
                {
                    RoomName = lobbyPrefs.RoomName,
                    MaxPlayers = lobbyPrefs.RoomMaxPlayers
                });
            }
            else
            {
                _netManager.JoinRoom(lobbyPrefs.RoomName);
            }
        }

        private IRoomMenuUI LoadView(Transform placeForUI, ResourcePath path)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(path), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<IRoomMenuUI>();
        }

        private void Subscribe()
        {
            _view.OnStartGamePressed += StartGame;
            _view.OnExitPressed += ExitFromRoom;

            _netManager.OnJoinInRoom += JoinedInRoom;
            _netManager.OnPlayerEnterInRoom += PlayerEnterInRoom;
            _netManager.OnPlayerLeftFromRoom += PlayerLeftedFromRoom;
            _netManager.OnLeftFromRoom += LeftedFromRoom;
        }

        private void Unsubscribe()
        {
            _view.OnStartGamePressed += StartGame;
            _view.OnExitPressed += ExitFromRoom;

            _netManager.OnJoinInRoom -= JoinedInRoom;
            _netManager.OnPlayerEnterInRoom -= PlayerEnterInRoom;
            _netManager.OnPlayerLeftFromRoom -= PlayerLeftedFromRoom;
            _netManager.OnLeftFromRoom -= LeftedFromRoom;
        }

        private void StartGame()
        {
            Debug.Log("StartGame");
        }

        private void ExitFromRoom()
        {
            _netManager.LeaveRoom();
        }

        private void JoinedInRoom(Room room)
        {
            _roomData = room;

            _view.InitUI(_roomData.Name);

            _playersInRoom = new List<Player>();

            var playersInRoom = _netManager.GetPlayerInRoom();

            for(int i =0; i< playersInRoom.Length; i++)
            {
                var player = playersInRoom[i];
                PlayerEnterInRoom(player);
            }
        }

        private void PlayerEnterInRoom(Player player)
        {
            _view.AddPlayer(player);
            
            _playersInRoom.Add(player);

            if (IsExceededPlayersLimit())
            {
                _netManager.ChangeRoomOpenState(false);
            }
        }

        private bool IsExceededPlayersLimit()
        {
            if (_playersInRoom.Count >= _roomData.MaxPlayers)
                return true;

            return false;
        }


        private void PlayerLeftedFromRoom(Player leftedPlayer)
        {
            _view.RemovePlayer(leftedPlayer);
        }

        private void LeftedFromRoom()
        {
            _lobbyPrefs.ChangeState(GameLobbyState.Exit);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            Unsubscribe();
        }
    }
}
