using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using Photon.Realtime;

namespace GameLobby
{
    public abstract class InRoomBaseMenuUI : MonoBehaviour, IRoomMenuUI
    {
        [SerializeField]
        private GameObject _roomElelemts;
        [SerializeField]
        private TMP_Text _roomName;
        [SerializeField]
        private Transform _playersInfoContainer;
        [SerializeField]
        private GameObject _playerInfoPrefab;

        protected string selectedPlayerName;
        
        private Dictionary<string, PlayerInfoObjectUI> _playerCollection = new();

        public abstract event Action OnStartGamePressed;
        public abstract event Action OnExitPressed;

        private void Awake()
        {
            _roomElelemts.SetActive(false);
        }

        public void InitUI(string roomName)
        {
            _roomName.text = roomName;

            _roomElelemts.SetActive(true);

            SubscribeUI();
        }

        protected abstract void SubscribeUI();

        protected abstract void UnsubscribeUI();

        public void AddPlayer(Player player)
        {
            var playerUI = CreatePlayerInfoView(player);

            _playerCollection[player.NickName] = playerUI;
        }

        public void RemovePlayer(Player player)
        {
            var playerUI = _playerCollection[player.NickName];

            RemovePlayerUI(playerUI);

            _playerCollection.Remove(player.NickName);
        }

        private void RemovePlayerUI(PlayerInfoObjectUI playerUI)
        {
            playerUI?.Dispose();

            Destroy(playerUI.gameObject);
        }

        private PlayerInfoObjectUI CreatePlayerInfoView(Player player)
        {
            GameObject objectView = Instantiate(_playerInfoPrefab, _playersInfoContainer, false);
            var view = objectView.GetComponent<PlayerInfoObjectUI>();

            view.InitUI(player.NickName, player.IsMasterClient);

            return view;
        }

        public void UpdatePlayerData(Player targetPlayer, string state)
        {
            var playerUI = _playerCollection[targetPlayer.NickName];
            playerUI.ChangePlayerState(state);
        }

        private void OnDestroy()
        {
            OnDestroyUI();
        }

        protected virtual void OnDestroyUI()
        {
            foreach (var playerInfo in _playerCollection)
            {
                var playerUI = playerInfo.Value;

                RemovePlayerUI(playerUI);
            }

            _playerCollection.Clear();

            UnsubscribeUI();
        }
    }
}
