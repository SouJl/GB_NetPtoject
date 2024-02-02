using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace GameLobby
{
    public class PlayerInfoObjectUI : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private Button _selfButton;
        [SerializeField]
        private Image _playerIcon; 
        [SerializeField]
        private TMP_Text _playerName;
        [SerializeField]
        private TMP_Text _playerState;

        private string _name;

        public event Action<string> OnSelected;

        public void InitUI(string playerName, bool isOwner)
        {
            _name = playerName;

            _playerName.text = playerName;

            _playerIcon.color = isOwner ? Color.green : Color.white;

            _playerState.text = "Waiting";

            SubscribeUI();
        }

        private void SubscribeUI()
        {
            _selfButton.onClick.AddListener(Selected);
        }

        private void UnsubscribeUI()
        {
            _selfButton.onClick.RemoveListener(Selected);
        }

        private void Selected()
        {
            OnSelected?.Invoke(_name);
        }

        public void ChangePlayerState(string newState)
        {
            _playerState.text = newState;
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
