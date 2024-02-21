using BTAPlayer;
using System;
using Tools;
using UnityEngine;

namespace UI
{
    public class GameUIController : MonoBehaviour
    {
        [SerializeField]
        private PlayerViewUI _playerViewUI;
        [SerializeField]
        private PauseMenuUI _pauseMenuUI;

        private bool _isOnPause;

        public PlayerViewUI PlayerViewUI => _playerViewUI;

        public bool IsOnPause 
        { 
            get => _isOnPause;
            private set 
            { 
                _isOnPause = value;

                GameStateManager.ChangePauseGameState(value);
            }
        }

        public event Action OnReturnMainMenu;
        public event Action OnExitGame;

        private void Awake()
        {
            Subscribe();
        }
        private void Start()
        {
            ShowPlayerUI();

            IsOnPause = false;
        }

        private void Subscribe()
        {
            PlayerInput.OnPauseInput += Pause;
            _pauseMenuUI.ResumeButton.onClick.AddListener(ResumeGame);
            _pauseMenuUI.MainMenuButton.onClick.AddListener(ToMainMenu);
            _pauseMenuUI.ExitGameButton.onClick.AddListener(ExitFromGame);
        }
  
        private void Unsubscribe()
        {
            PlayerInput.OnPauseInput -= Pause;

            _pauseMenuUI.ResumeButton.onClick.RemoveListener(ResumeGame);
            _pauseMenuUI.MainMenuButton.onClick.RemoveListener(ToMainMenu);
            _pauseMenuUI.ExitGameButton.onClick.RemoveListener(ExitFromGame);
        }

        private void Pause()
        {
            if (!IsOnPause)
            {
                ShowPauseMenuUI();
            }
            else
            {
                ShowPlayerUI();
            }

            IsOnPause = !IsOnPause;
        }

        private void ResumeGame()
        {
            ShowPlayerUI();

            IsOnPause = false;
        }

        private void ToMainMenu()
        {
            OnReturnMainMenu?.Invoke();
        }

        private void ExitFromGame()
        {
            OnExitGame?.Invoke();
        }

        private void ShowPlayerUI()
        {
            _playerViewUI.Show();
            _pauseMenuUI.Hide();

            LockCursor(true);
        }

        private void ShowPauseMenuUI()
        {
            _pauseMenuUI.Show();
            _playerViewUI.Hide();

            LockCursor(false);
        }

        private void LockCursor(bool state)
        {
            Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;

            Cursor.visible = !state;
        }

        private void OnDestroy()
        {
            IsOnPause = false;

            LockCursor(false);

            Unsubscribe();
        }
    }
}
