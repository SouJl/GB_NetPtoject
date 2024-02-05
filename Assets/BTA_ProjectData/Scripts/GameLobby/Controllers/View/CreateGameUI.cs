using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Abstraction;
using System.Collections.Generic;

namespace GameLobby
{
    public class CreateGameUI : MonoBehaviour
    {
        [SerializeField]
        private Toggle _isGamePublic;
        [SerializeField]
        private TMP_InputField _gameName;
        [SerializeField]
        private TMP_Text _minPlayersValue;
        [SerializeField]
        private TMP_Text _maxPlayersValue;
        [SerializeField]
        private TMP_Text _currentPlayersValue;
        [SerializeField]
        private Slider _playersSliderValue;

        [Space(10)]
        [Header("Whitelist settings")]
        [SerializeField]
        private Transform _whitelistContainer;
        [SerializeField]
        private GameObject _whitelistPrefab;
        [SerializeField]
        private Button _addReserveSlotButton;
        [SerializeField]
        private Button _removeReserveSlotButton;

        [Space(20)]
        [SerializeField]
        private Button _createGameButton;
        [SerializeField]
        private Button _backButton;

        private string _createRoomName;
        private byte _maxPlayersInRoom;
        private List<ReserveSlotObjectUI> _whitelistCollection;

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

            _isGamePublic.isOn = true;

            _whitelistCollection = new();
        }

        public void SubscribeUI()
        {
            _gameName.onValueChanged.AddListener(NameChanged);
            _playersSliderValue.onValueChanged.AddListener(PlayersValueChanged);

            _addReserveSlotButton.onClick.AddListener(AddReserveSlot);
            _removeReserveSlotButton.onClick.AddListener(RemoveReserveSlot);

            _backButton.onClick.AddListener(Back);
            _createGameButton.onClick.AddListener(CreateRoom);
        }

        public void UnsubscribeUI()
        {
            _gameName.onValueChanged.RemoveListener(NameChanged);
            _playersSliderValue.onValueChanged.RemoveListener(PlayersValueChanged);

            _addReserveSlotButton.onClick.RemoveListener(AddReserveSlot);
            _removeReserveSlotButton.onClick.RemoveListener(RemoveReserveSlot);

            _backButton.onClick.RemoveListener(Back);
            _createGameButton.onClick.RemoveListener(CreateRoom);
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
            var view = CreateReserveSlotView(_whitelistContainer);
            
            view.InitUI();

            _whitelistCollection.Add(view);
        }

        private ReserveSlotObjectUI CreateReserveSlotView(Transform placement)
        {
            GameObject objectView = Instantiate(_whitelistPrefab, placement, false);
            var view = objectView.GetComponent<ReserveSlotObjectUI>();

            return view;
        }

        private void RemoveReserveSlot()
        {
            if (_whitelistCollection.Count == 0)
                return;

            var view = _whitelistCollection[(_whitelistCollection.Count - 1)];

            view?.Dispose();

            Destroy(view.gameObject);

            _whitelistCollection.Remove(view);
        }

        private void Back()
        {
            OnBackPressed?.Invoke();
        }

        private void CreateRoom()
        {
            var reserveSlots = GetReseveSlots();
            var publishUserId = reserveSlots == null ? false : true;
            OnCreateRoomPressed?.Invoke(new CreationRoomData 
            {
                RoomName = _createRoomName,
                MaxPlayers = _maxPlayersInRoom,
                IsPublic = _isGamePublic.isOn,
                PublishUserId = publishUserId,
                Whitelist = reserveSlots
            });
        }

        private string[] GetReseveSlots()
        {
            if (_whitelistCollection.Count == 0)
                return null;

            var resultArr = new string[_whitelistCollection.Count];

            for (int i = 0; i < _whitelistCollection.Count; i++)
            {
                resultArr[i] = $"BtaPlayerId_{_whitelistCollection[i].CurrentName}";
            }

            return resultArr;
        }

        private void OnDestroy()
        {
            for(int i =0;i < _whitelistCollection.Count; i++)
            {
                var view = _whitelistCollection[i];
                
                view?.Dispose();

                Destroy(view.gameObject);
            }

            _whitelistCollection.Clear();

            UnsubscribeUI();

        }
    }
}
