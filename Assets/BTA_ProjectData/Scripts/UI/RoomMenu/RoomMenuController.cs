using Abstraction;
using Configs;
using Photon.Realtime;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    public class RoomMenuController : BaseUIController
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/RoomMenu");

        private readonly RoomMenuUI _view;
        private readonly GameConfig _gameConfig;
        private readonly GamePrefs _gamePrefs;
        private readonly PhotonNetManager _netManager;

        public RoomMenuController(
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

        private RoomMenuUI LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<RoomMenuUI>();
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
