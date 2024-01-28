using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _userName;
        [SerializeField]
        private Button _switchUserButton;
        [Space(10)]
        [SerializeField]
        private Button _joinGameButton;
        [SerializeField]
        private Button _createGameButton;
        [SerializeField]
        private Button _exitButton;

        public event Action OnSwitchUserPressed;
        public event Action OnJoinGamePressed;
        public event Action OnCreateGamePressed;
        public event Action OnExitGamePressed;

        public void InitUI(string userName)
        {
            _userName.text = userName;

            SubscibeUI();
        }

        private void SubscibeUI()
        {
            _switchUserButton.onClick.AddListener(() => OnSwitchUserPressed?.Invoke());
            _joinGameButton.onClick.AddListener(() => OnJoinGamePressed?.Invoke());
            _createGameButton.onClick.AddListener(() => OnCreateGamePressed?.Invoke());
            _exitButton.onClick.AddListener(() => OnExitGamePressed?.Invoke());
        }
        private void UnsubscibeUI()
        {
            _switchUserButton.onClick.RemoveAllListeners();
            _joinGameButton.onClick.RemoveAllListeners();
            _createGameButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            UnsubscibeUI();
        }
    }
}
