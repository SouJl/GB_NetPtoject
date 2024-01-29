using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Photon.Realtime;

namespace UI
{
    public class RoomInfoObjectUI : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private TMP_Text _roomName;
        [SerializeField]
        private TMP_Text _roomState;
        [SerializeField]
        private TMP_Text _roomPlayers;

        [SerializeField]
        private Button _selfButton;

        private string _name;

        public event Action<string> OnSelected;

        public void InitUI(RoomInfo roomInfo)
        {
            SubscribeUI();
            SetupRoomUI(roomInfo);
        }

        private void SubscribeUI()
        {
            _selfButton.onClick.AddListener(RoomSelected);
        }

        private void UnsubscribeUI()
        {
            _selfButton.onClick.RemoveListener(RoomSelected);
        }

        private void RoomSelected()
        {
            OnSelected?.Invoke(_name);
        }

        private void SetupRoomUI(RoomInfo roomInfo)
        {
            _name = roomInfo.Name;

            _roomName.text = _name;
            _roomState.text = roomInfo.IsOpen ? "Open" : "Close";
            _roomPlayers.text = $"{roomInfo.PlayerCount}/{roomInfo.MaxPlayers}";
        }

        private bool _isDisposed = false;

        public void Dispose()
        {
            if (_isDisposed)
                return;
            
            _isDisposed = true;

            UnsubscribeUI();
        }
     
    }
}
