using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Realtime;
using Configs;

namespace UI
{
    public class LobbyBrowseMenuUI : MonoBehaviour
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

        public event Action<string> OnJoinRoomPressed;
        public event Action OnHostGamePressed;
        public event Action OnClosePressed;

        private string _selectedRoomName;

        private List<RoomInfoObjectUI> _roomsCollection = new();

        public void InitUI(GameConfig gameConfig)
        {
            SubscribeUI();

            Hide();
        }

        private void SubscribeUI()
        {
            _joinRoomButton.onClick.AddListener(JoinSelectedRoom);
            _hostGameButton.onClick.AddListener(() => OnHostGamePressed?.Invoke());
            _closeButton.onClick.AddListener(() => OnClosePressed?.Invoke());
        }

        private void UnsubscribeUI()
        {
            _joinRoomButton.onClick.RemoveListener(JoinSelectedRoom);
            _hostGameButton.onClick.RemoveAllListeners();
            _closeButton.onClick.RemoveAllListeners();
        }

        private void JoinSelectedRoom()
        {
            if (_selectedRoomName == null)
                return;

            OnJoinRoomPressed?.Invoke(_selectedRoomName);
        }
     
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
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
            ClearRoomsData();

            UnsubscribeUI();
        }
    }
}
