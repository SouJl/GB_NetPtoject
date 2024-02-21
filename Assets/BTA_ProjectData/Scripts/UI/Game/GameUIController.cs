using BTAPlayer;
using System;
using Tools;
using UnityEngine;

namespace UI
{
    public class GameUIController : MonoBehaviour
    {
        [SerializeField]
        private PlayerViewUI _playerView;
        [SerializeField]
        private PlayerDeadViewUI _playerDeadView;
        [SerializeField]
        private PauseMenuUI _pauseMenu;
        [SerializeField]
        private GameOverScreenUI _gameOverScreen;
        [SerializeField]
        private GameWonScreenUI _gameWonScreen;

        private bool _isOnPause;

        public PlayerViewUI PlayerViewUI => _playerView;

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

            _pauseMenu.ResumeButton.onClick.AddListener(ResumeGame);
            _pauseMenu.MainMenuButton.onClick.AddListener(ToMainMenu);
            _pauseMenu.ExitGameButton.onClick.AddListener(ExitFromGame);

            _gameOverScreen.MainMenuButton.onClick.AddListener(ToMainMenu);
            _gameOverScreen.ExitGameButton.onClick.AddListener(ExitFromGame);

            _gameWonScreen.MainMenuButton.onClick.AddListener(ToMainMenu);
            _gameWonScreen.ExitGameButton.onClick.AddListener(ExitFromGame);
        }
  
        private void Unsubscribe()
        {
            PlayerInput.OnPauseInput -= Pause;

            _pauseMenu.ResumeButton.onClick.RemoveListener(ResumeGame);
            _pauseMenu.MainMenuButton.onClick.RemoveListener(ToMainMenu);
            _pauseMenu.ExitGameButton.onClick.RemoveListener(ExitFromGame);

            _gameOverScreen.MainMenuButton.onClick.RemoveListener(ToMainMenu);
            _gameOverScreen.ExitGameButton.onClick.RemoveListener(ExitFromGame);

            _gameWonScreen.MainMenuButton.onClick.RemoveListener(ToMainMenu);
            _gameWonScreen.ExitGameButton.onClick.RemoveListener(ExitFromGame);
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
            _playerView.Show();

            _playerDeadView.Hide();
            _pauseMenu.Hide();
            _gameOverScreen.Hide();
            _gameWonScreen.Hide();

            LockCursor(true);
        }

        private void ShowPauseMenuUI()
        {
            _pauseMenu.Show();

            _playerView.Hide();
            _playerDeadView.Hide();
            _gameOverScreen.Hide();
            _gameWonScreen.Hide();

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
