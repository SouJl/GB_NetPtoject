using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Abstraction;
using System.Collections.Generic;

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
        private Toggle _isRoomClosed;

        [Space(10)]
        [Header("Reserve slots settings")]
        [SerializeField]
        private Transform _reserveSlotsContainer;
        [SerializeField]
        private GameObject _reserveSlotPrefab;
        [SerializeField]
        private Button _addReserveSlotButton;
        [SerializeField]
        private Button _removeReserveSlotButton;

        [Space(20)]
        [SerializeField]
        private Button _createRoomButton;
        [SerializeField]
        private Button _backButton;

        private string _createRoomName;
        private byte _maxPlayersInRoom;
        private List<ReserveSlotObjectUI> _reserveSlotCollection;

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

            _isRoomClosed.isOn = false;

            _reserveSlotCollection = new();
        }

        public void SubscribeUI()
        {
            _roomName.onValueChanged.AddListener(NameChanged);
            _playersSliderValue.onValueChanged.AddListener(PlayersValueChanged);

            _addReserveSlotButton.onClick.AddListener(AddReserveSlot);
            _removeReserveSlotButton.onClick.AddListener(RemoveReserveSlot);

            _backButton.onClick.AddListener(Back);
            _createRoomButton.onClick.AddListener(CreateRoom);
        }

        public void UnsubscribeUI()
        {
            _roomName.onValueChanged.RemoveListener(NameChanged);
            _playersSliderValue.onValueChanged.RemoveListener(PlayersValueChanged);

            _addReserveSlotButton.onClick.RemoveListener(AddReserveSlot);
            _removeReserveSlotButton.onClick.RemoveListener(RemoveReserveSlot);

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

        private void AddReserveSlot()
        {
            var view = CreateReserveSlotView(_reserveSlotsContainer);
            
            view.InitUI();

            _reserveSlotCollection.Add(view);
        }

        private ReserveSlotObjectUI CreateReserveSlotView(Transform placement)
        {
            GameObject objectView = Instantiate(_reserveSlotPrefab, placement, false);
            var view = objectView.GetComponent<ReserveSlotObjectUI>();

            return view;
        }

        private void RemoveReserveSlot()
        {
            if (_reserveSlotCollection.Count == 0)
                return;

            var view = _reserveSlotCollection[_reserveSlotCollection.Count];

            view?.Dispose();

            Destroy(view.gameObject);

            _reserveSlotCollection.Remove(view);
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
                MaxPlayers = _maxPlayersInRoom,
                IClosed = _isRoomClosed.isOn,
                ReserveSlots = GetReseveSlots()
            });
        }

        private string[] GetReseveSlots()
        {
            if (_reserveSlotCollection.Count == 0)
                return null;

            var resultArr = new string[_reserveSlotCollection.Count];

            for (int i = 0; i < _reserveSlotCollection.Count; i++)
            {
                resultArr[i] = _reserveSlotCollection[i].CurrentName;
            }

            return resultArr;
        }

        private void OnDestroy()
        {
            for(int i =0;i < _reserveSlotCollection.Count; i++)
            {
                var view = _reserveSlotCollection[i];
                
                view?.Dispose();

                Destroy(view.gameObject);
            }

            _reserveSlotCollection.Clear();

            UnsubscribeUI();

        }
    }
}
