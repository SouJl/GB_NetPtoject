using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace UI
{
    public class CreateRoomUI : MonoBehaviour, IDisposable
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

        private CreationRoomData _roomData;

        public event Action OnBackPressed;
        public event Action<CreationRoomData> OnCreateRoomPressed;

        public void InitUI(int minPlayersValue, int maxPlayersValue)
        {
            _roomData = new CreationRoomData();

            SubscribeUI();

            _minPlayersValue.text = minPlayersValue.ToString();
            _maxPlayersValue.text = maxPlayersValue.ToString();
            _playersSliderValue.minValue = minPlayersValue;
            _playersSliderValue.maxValue = maxPlayersValue;

            _playersSliderValue.value = maxPlayersValue / 2;

            _currentPlayersValue.text = _playersSliderValue.value.ToString();

            Hide();
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
            _roomData.RoomName = name;
        }

        private void PlayersValueChanged(float value)
        {
            _roomData.MaxPlayers = (byte)value;

            _currentPlayersValue.text = value.ToString();
        }

        private void Back()
        {
            OnBackPressed?.Invoke();
        }
        private void CreateRoom()
        {
            OnCreateRoomPressed?.Invoke(_roomData);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        #region IDisposable
        
        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            UnsubscribeUI();

            Hide();
        }

        #endregion
    }
}
