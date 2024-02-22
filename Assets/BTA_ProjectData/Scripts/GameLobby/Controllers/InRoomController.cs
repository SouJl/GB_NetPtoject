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

        private List<BTAPlayer> _playersInRoom;
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
            _lobbyPrefs.ClickSound.Play();

            var player = _playersInRoom.Find(p => p.PlayerData.NickName == _lobbyPrefs.NickName);

            if (!player.PlayerData.IsMasterClient)
            {
                var props = new Hashtable
                {
                    {BTAConst.PLAYER_READY, "Ready"}
                };

                _netManager.SetPlayerProperties(props);
            }
            else
            {
                var isAllPlayersReady = true;

                for (int i = 0; i < _playersInRoom.Count; i++) 
                {
                    if (_playersInRoom[i].PlayerData.IsMasterClient)
                        continue;

                    if (_playersInRoom[i].IsReady == false)
                        isAllPlayersReady = false;
                }

                if (isAllPlayersReady)
                {
                    _netManager.IsRoomVisiable = false;

                    _lobbyPrefs.ChangeState(GameLobbyState.StartGame);
                }
                else
                {
                    Debug.Log("Not all of players is ready!");
                }
            }
        }

        private void ExitFromRoom()
        {
            _lobbyPrefs.ClickSound.Play();

            if (_netManager.CurrentPlayer.IsMasterClient)
            {
                for (int i = 0; i < _playersInRoom.Count; i++)
                {
                    var player = _playersInRoom[i];

                    if (player.PlayerData.IsMasterClient)
                        continue;

                    _netManager.CloseConnectionToClient(player.PlayerData);
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

            _playersInRoom = new List<BTAPlayer>();

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

            _playersInRoom.Add(new BTAPlayer(player));

            _netManager.IsRoomOpen = !IsExceededPlayersLimit();
        }

        private bool IsExceededPlayersLimit()
        {
            if (_netManager.IsCurrentRoomFull)
                return true;

            return false;
        }

        private void PlayerLeftedFromRoom(Player leftedPlayer)
        {
            var player
                = _playersInRoom.Find(p => p.PlayerData.ActorNumber == leftedPlayer.ActorNumber);

            _view.RemovePlayer(player.PlayerData);

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

            var targetPlayer = _playersInRoom.Find(p => p.PlayerData.ActorNumber == player.ActorNumber);

            if (targetPlayer != null)
            {
                object playerState;

                if (properties.TryGetValue(BTAConst.PLAYER_READY, out playerState))
                {
                    targetPlayer.IsReady = (string)playerState == "Ready" ? true : false;

                    _view.UpdatePlayerData(targetPlayer.PlayerData, (string)playerState);
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
