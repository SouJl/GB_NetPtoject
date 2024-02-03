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
        private Button _connectGameButton;
        [SerializeField]
        private Button _exitButton;

        [Space(10)]
        [Header("Conenct to game settings")]
        [SerializeField]
        private GameObject _connectPanel;
        [SerializeField]
        private TMP_InputField _roomName;
        [SerializeField]
        private Button _proceedConnectionButton;
        [SerializeField]
        private Button _backButton;

        public event Action OnSwitchUserPressed;
        public event Action OnJoinGamePressed;
        public event Action OnExitGamePressed;
        public event Action<string> OnConnectToGame;

        private string _connectedName;


        public void InitUI(string userName)
        {
            _userName.text = userName;

            _connectPanel.SetActive(false);

            SubscibeUI();
        }

        private void SubscibeUI()
        {
            _switchUserButton.onClick.AddListener(() => OnSwitchUserPressed?.Invoke());
            _joinGameButton.onClick.AddListener(() => OnJoinGamePressed?.Invoke());
            _connectGameButton.onClick.AddListener(ShowConnectToGame);
            _exitButton.onClick.AddListener(() => OnExitGamePressed?.Invoke());

            _roomName.onValueChanged.AddListener(ChangeConnectedName);
            _proceedConnectionButton.onClick.AddListener(ConnectToGame);
            _backButton.onClick.AddListener(BackFromConnection);
        }
        private void UnsubscibeUI()
        {
            _switchUserButton.onClick.RemoveAllListeners();
            _joinGameButton.onClick.RemoveAllListeners();
            _connectGameButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();

            _roomName.onValueChanged.RemoveListener(ChangeConnectedName);
            _proceedConnectionButton.onClick.RemoveListener(ConnectToGame);
            _backButton.onClick.RemoveListener(BackFromConnection);
        }

        public void ShowConnectToGame()
        {
            _connectPanel.SetActive(true);
            _roomName.onValueChanged.AddListener(ChangeConnectedName);
            _proceedConnectionButton.onClick.AddListener(ConnectToGame);
            _backButton.onClick.AddListener(BackFromConnection);
        }

        private void ChangeConnectedName(string value)
        {
            _connectedName = value;
        }

        private void ConnectToGame()
        {
            _connectPanel.SetActive(false);

            OnConnectToGame?.Invoke(_connectedName);
        }

        private void BackFromConnection()
        {
            _connectPanel.SetActive(false);
        }

   
        private void OnDestroy()
        {
            UnsubscibeUI();
        }
    }
}
