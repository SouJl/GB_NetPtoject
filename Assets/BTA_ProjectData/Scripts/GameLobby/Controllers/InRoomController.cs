using Abstraction;
using Configs;
using Enumerators;
using Photon.Realtime;
using Prefs;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameLobby
{
    public class InRoomController : BaseController
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/InRoomMenu");

        private readonly InRoomMenuUI _view;
        private readonly GameConfig _gameConfig;
        private readonly GameLobbyPrefs _lobbyPrefs;
        private readonly PhotonNetManager _netManager;

        public InRoomController(
            Transform placeForUI,
            GameConfig gameConfig,
            GameLobbyPrefs lobbyPrefs,
            PhotonNetManager netManager)
        {
            _gameConfig = gameConfig;
            _lobbyPrefs = lobbyPrefs;
            _netManager = netManager;

            _view = LoadView(placeForUI);
            _view.InitUI();

            Subscribe();

            InitLobbyData(_lobbyPrefs);        
        }

        private void InitLobbyData(GameLobbyPrefs lobbyPrefs)
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

        private InRoomMenuUI LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<InRoomMenuUI>();
        }

        private void Subscribe()
        {
            _view.OnStartGamePressed += StartGame;
            _view.OnExitPressed += ExitFromRoom;

            _netManager.OnJoinInRoom += JoinedInRoom;
            _netManager.OnPlayerEnterInRoom += PlayerEnterInRoom;
            _netManager.OnLeftFromRoom += LeftedFromRoom;
        }

  
        private void Unsubscribe()
        {
            _view.OnStartGamePressed += StartGame;
            _view.OnExitPressed += ExitFromRoom;

            _netManager.OnJoinInRoom -= JoinedInRoom;
            _netManager.OnPlayerEnterInRoom -= PlayerEnterInRoom;
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
            _view.SetRoomData(room);

            var playersInRoom = _netManager.GetPlayerInRomm();

            for(int i =0; i< playersInRoom.Length; i++)
            {
                var player = playersInRoom[i];
                _view.AddPlayer(player);
            }
        }

        private void PlayerEnterInRoom(Player player)
        {
            _view.AddPlayer(player);
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
