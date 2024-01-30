using Abstraction;
using Configs;
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
            _netManager.OnJoinInRoom += JoinedInRoom;
        }

        private void Unsubscribe()
        {
            _netManager.OnJoinInRoom -= JoinedInRoom;
        }


        private void JoinedInRoom(Room room)
        {
            _view.SetRoomData(room);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            Unsubscribe();
        }
    }
}
