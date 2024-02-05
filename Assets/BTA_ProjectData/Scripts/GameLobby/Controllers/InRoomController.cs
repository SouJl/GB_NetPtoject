using Abstraction;
using Configs;
using Enumerators;
using ExitGames.Client.Photon;
using MultiplayerService;
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
        private readonly GameNetManager _netManager;

        private IRoomMenuUI _view;

        private List<Player> _playersInRoom;
        private Room _roomData;

        public InRoomController(
            Transform placeForUI,
            GameConfig gameConfig,
            GameLobbyPrefs lobbyPrefs,
            GameNetManager netManager)
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
                _netManager.CreateRoom(lobbyPrefs.CreationData);
            }
            else
            {
                _netManager.JoinRoom(lobbyPrefs.CreationData.RoomName);
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
            _netManager.OnPlayerPropsUpdated += UpdatePlayersProperties;
        }


        private void Unsubscribe()
        {
            _view.OnStartGamePressed += StartGame;
            _view.OnExitPressed += ExitFromRoom;

            _netManager.OnJoinInRoom -= JoinedInRoom;
            _netManager.OnPlayerEnterInRoom -= PlayerEnterInRoom;
            _netManager.OnPlayerLeftFromRoom -= PlayerLeftedFromRoom;
            _netManager.OnLeftFromRoom -= LeftedFromRoom;
            _netManager.OnPlayerPropsUpdated -= UpdatePlayersProperties;
        }

        private void StartGame()
        {
            var player = _playersInRoom.Find(p => p.NickName == _lobbyPrefs.NickName);

            if (!player.IsMasterClient)
            {
                var props = new Hashtable
                {
                    {BTAConst.PLAYER_READY, "Ready"}
                };

                _netManager.SetPlayerProperties(props);
            }
            else
            {
                _view.UpdatePlayerData(player, "Ready");

                _netManager.IsRoomVisiable = false;

                _lobbyPrefs.ChangeState(GameLobbyState.StartGame);
            }
        }

        private void ExitFromRoom()
        {
            if (_netManager.CurrentPlayer.IsMasterClient)
            {
                for (int i = 0; i < _playersInRoom.Count; i++)
                {
                    if (_playersInRoom[i].IsMasterClient)
                        continue;

                    _netManager.CloseConnectionToClient(_playersInRoom[i]);
                }
            }

            _netManager.LeaveRoom();
        }

        private void JoinedInRoom(Room room)
        {
            _roomData = room;

            var playersInRoom = _netManager.GetPlayerInRoom();

            if (IsAcceptedInRoom(room, playersInRoom) == false)
            {
                ExitFromRoom();
                return;
            }

            _view.InitUI(_roomData.Name);

            _playersInRoom = new List<Player>();

            for (int i = 0; i < playersInRoom.Length; i++)
            {
                var player = playersInRoom[i];
                PlayerEnterInRoom(player);
            }
        }

        private bool IsAcceptedInRoom(Room room, Player[] players)
        {
            var player = _netManager.CurrentPlayer;

            if (player.IsMasterClient)
                return true;

            if (room.ExpectedUsers == null)
                return true;

            for (int i = 0; i < room.ExpectedUsers.Length; i++)
            {
                if (room.ExpectedUsers[i] == player.UserId)
                    return true;
            }

            return false;
        }

        private void PlayerEnterInRoom(Player player)
        {
            _view.AddPlayer(player);

            _playersInRoom.Add(player);

            _netManager.IsRoomOpen = !IsExceededPlayersLimit();
        }

        private bool IsExceededPlayersLimit()
        {
            if (_playersInRoom.Count >= _roomData.MaxPlayers)
                return true;

            return false;
        }

        private void PlayerLeftedFromRoom(Player leftedPlayer)
        {
            var player
                = _playersInRoom.Find(p => p.ActorNumber == leftedPlayer.ActorNumber);

            _view.RemovePlayer(player);

            _playersInRoom.Remove(player);

            _netManager.IsRoomOpen = !IsExceededPlayersLimit();
        }

        private void LeftedFromRoom()
        {
            _lobbyPrefs.ChangeState(GameLobbyState.Exit);
        }


        private void UpdatePlayersProperties(Player player, Hashtable properties)
        {
            if (_playersInRoom.Count == 0)
                return;

            var targetPlayer = _playersInRoom.Find(p => p.ActorNumber == player.ActorNumber);

            if (targetPlayer != null)
            {
                object playerState;

                if (properties.TryGetValue(BTAConst.PLAYER_READY, out playerState))
                {
                    _view.UpdatePlayerData(targetPlayer, (string)playerState);
                }
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (_playersInRoom != null)
                _playersInRoom.Clear();

            Unsubscribe();
        }
    }
}
