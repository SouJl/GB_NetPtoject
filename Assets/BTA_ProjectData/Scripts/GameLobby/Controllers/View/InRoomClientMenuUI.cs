﻿using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLobby
{
    public class InRoomClientMenuUI : MonoBehaviour, IRoomMenuUI
    {
        [SerializeField]
        private TMP_Text _roomName;
        [SerializeField]
        private Transform _playersInfoContainer;
        [SerializeField]
        private GameObject _playerInfoPrefab;

        [SerializeField]
        private Button _exitButton;

        private Dictionary<string, PlayerInfoObjectUI> _playerCollection = new();

        public event Action OnStartGamePressed;
        public event Action OnExitPressed;

        public void InitUI(string roomName)
        {
            _roomName.text = roomName;
            SubscribeUI();
        }

        private void SubscribeUI()
        {
            _exitButton.onClick.AddListener(() => OnExitPressed?.Invoke());
        }
        private void UnsubscribeUI()
        {
            _exitButton.onClick.RemoveAllListeners();
        }

        public void AddPlayer(Player player)
        {
            var playerUI = CreatePlayerInfoView(player);

            _playerCollection[player.NickName] = playerUI;
        }

        public void RemovePlayer(Player player)
        {
            var playerUI = _playerCollection[player.NickName];

            Destroy(playerUI.gameObject);

            _playerCollection.Remove(player.NickName);
        }

        private PlayerInfoObjectUI CreatePlayerInfoView(Player player)
        {
            GameObject objectView = Instantiate(_playerInfoPrefab, _playersInfoContainer, false);
            var view = objectView.GetComponent<PlayerInfoObjectUI>();

            view.InitUI(player.NickName, player.IsMasterClient);

            return view;
        }

        private void OnDestroy()
        {
            foreach (var playerInfo in _playerCollection)
            {
                Destroy(playerInfo.Value.gameObject);
            }

            _playerCollection.Clear();

            UnsubscribeUI();
        }
    }
}
