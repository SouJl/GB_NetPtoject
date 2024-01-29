using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Realtime;
using Configs;

namespace UI
{
    public class GameLobbyMenuUI : MonoBehaviour
    {
        [SerializeField]
        private Button _joinRoomButton;
        [SerializeField]
        private Button _hostGameButton;
        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private Transform _roomsInfoContainer;
        [SerializeField]
        private GameObject _roomInfoPrefab;

        [Space(10)]
        [SerializeField]
        private GameObject _lobbyObjectsUI;
        [SerializeField]
        private CreateRoomUI _createRoomUI;

        public event Action<string> OnJoinRoomPressed;
        public event Action<CreationRoomData> OnCreateRoomPressed;
        public event Action OnClosePressed;

        private string _selectedRoomName;

        private List<RoomInfoObjectUI> _roomsCollection = new();

        public void InitUI(GameConfig gameConfig)
        {
            _createRoomUI.InitUI(gameConfig.RoomMinPlayers, gameConfig.RoomMaxPlayers);

            SubscribeUI();
        }

        private void SubscribeUI()
        {
            _joinRoomButton.onClick.AddListener(JoinSelectedRoom);
            _hostGameButton.onClick.AddListener(OpenRoomCreationMenu);
            _closeButton.onClick.AddListener(() => OnClosePressed?.Invoke());

            _createRoomUI.OnCreateRoomPressed += CreateRoom;
            _createRoomUI.OnBackPressed += BackFromCreationMenu;
        }

        private void UnsubscribeUI()
        {
            _joinRoomButton.onClick.RemoveListener(JoinSelectedRoom);
            _hostGameButton.onClick.RemoveListener(OpenRoomCreationMenu);
            _closeButton.onClick.RemoveAllListeners();

            _createRoomUI.OnCreateRoomPressed -= CreateRoom;
            _createRoomUI.OnBackPressed -= BackFromCreationMenu;
        }

        private void JoinSelectedRoom()
        {
            if (_selectedRoomName == null)
                return;

            OnJoinRoomPressed?.Invoke(_selectedRoomName);
        }

        private void OpenRoomCreationMenu()
        {
            _createRoomUI.Show();
            _lobbyObjectsUI.SetActive(false);
        }
     
        private void CreateRoom(CreationRoomData data)
        {
            _createRoomUI.Hide();
            OnCreateRoomPressed?.Invoke(data);
        }

        private void BackFromCreationMenu()
        {
            _createRoomUI.Hide();
            _lobbyObjectsUI.SetActive(true);
        }

        public void AddRooms(List<RoomInfo> _roomsInfo)
        {
            ClearRoomsData();

            for (int i = 0; i < _roomsInfo.Count; i++)
            {
                var roomInfo = _roomsInfo[i];
                var room = CreateRoomInfoView(roomInfo);

                room.OnSelected += RoomSelected;

                _roomsCollection.Add(room);
            }
        }

        private RoomInfoObjectUI CreateRoomInfoView(RoomInfo roomInfo)
        {
            GameObject objectView = Instantiate(_roomInfoPrefab, _roomsInfoContainer, false);
            var view = objectView.GetComponent<RoomInfoObjectUI>();

            view.InitUI(roomInfo);

            return view;
        }

        private void ClearRoomsData()
        {
            for (int i = 0; i < _roomsCollection.Count; i++)
            {
                var room = _roomsCollection[i];

                room.OnSelected -= RoomSelected;

                room?.Dispose();

                Destroy(room.gameObject);
            }

            _roomsCollection.Clear();
        }


        private void RoomSelected(string roomName)
        {
            _selectedRoomName = roomName;
        }

        private void OnDestroy()
        {
            _createRoomUI?.Dispose();

            ClearRoomsData();

            UnsubscribeUI();
        }
    }
}
