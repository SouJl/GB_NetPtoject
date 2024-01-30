using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Abstraction;

namespace GameLobby
{
    public class CreateRoomUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _roomName;
        [SerializeField]
        private TMP_Text _minPlayersValue;
        [SerializeField]
        private TMP_Text _maxPlayersValue;
        [SerializeField]
        private TMP_Text _currentPlayersValue;
        [SerializeField]
        private Slider _playersSliderValue;

        [SerializeField]
        private Button _createRoomButton;
        [SerializeField]
        private Button _backButton;

        private string _createRoomName;
        private byte _maxPlayersInRoom;

        public event Action OnBackPressed;
        public event Action<CreationRoomData> OnCreateRoomPressed;

        public void InitUI(int minPlayersValue, int maxPlayersValue)
        {
            SubscribeUI();

            _minPlayersValue.text = minPlayersValue.ToString();
            _maxPlayersValue.text = maxPlayersValue.ToString();
            _playersSliderValue.minValue = minPlayersValue;
            _playersSliderValue.maxValue = maxPlayersValue;

            _playersSliderValue.value = maxPlayersValue / 2;

            _currentPlayersValue.text = _playersSliderValue.value.ToString();
        }

        public void SubscribeUI()
        {
            _roomName.onValueChanged.AddListener(NameChanged);
            _playersSliderValue.onValueChanged.AddListener(PlayersValueChanged);
            _backButton.onClick.AddListener(Back);
            _createRoomButton.onClick.AddListener(CreateRoom);
        }

        public void UnsubscribeUI()
        {
            _roomName.onValueChanged.RemoveListener(NameChanged);
            _playersSliderValue.onValueChanged.RemoveListener(PlayersValueChanged);
            _backButton.onClick.RemoveListener(Back);
            _createRoomButton.onClick.RemoveListener(CreateRoom);
        }

        private void NameChanged(string name)
        {
            _createRoomName = name;
        }

        private void PlayersValueChanged(float value)
        {
            _maxPlayersInRoom = (byte)value;

            _currentPlayersValue.text = value.ToString();
        }

        private void Back()
        {
            OnBackPressed?.Invoke();
        }

        private void CreateRoom()
        {
            OnCreateRoomPressed?.Invoke(new CreationRoomData 
            {
                RoomName = _createRoomName,
                MaxPlayers = _maxPlayersInRoom
            });
        }

        private void OnDestroy()
        {
            UnsubscribeUI();
        }
    }
}
