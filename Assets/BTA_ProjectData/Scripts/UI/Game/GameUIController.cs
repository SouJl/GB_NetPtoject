using BTAPlayer;
using Photon.Pun;
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
        private GameOverScreenUI _gameOverMasterScreen;
        [SerializeField]
        private GameOverScreenUI _gameOverClientScreen;
        [SerializeField]
        private GameWonScreenUI _gameWonScreen;

        private bool _isOnPause;
        private bool _isGameOver;
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

        public event Action OnRestartGame;
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

            _gameOverMasterScreen.RestartButton.onClick.AddListener(RestartGame);
            _gameOverMasterScreen.MainMenuButton.onClick.AddListener(ToMainMenu);
            _gameOverMasterScreen.ExitGameButton.onClick.AddListener(ExitFromGame);

            _gameWonScreen.MainMenuButton.onClick.AddListener(ToMainMenu);
            _gameWonScreen.ExitGameButton.onClick.AddListener(ExitFromGame);

            GameStateManager.OnGameOver += ShowGameOver;
        }

        private void Unsubscribe()
        {
            PlayerInput.OnPauseInput -= Pause;

            _pauseMenu.ResumeButton.onClick.RemoveListener(ResumeGame);
            _pauseMenu.MainMenuButton.onClick.RemoveListener(ToMainMenu);
            _pauseMenu.ExitGameButton.onClick.RemoveListener(ExitFromGame);

            _gameOverMasterScreen.RestartButton.onClick.RemoveListener(RestartGame);
            _gameOverMasterScreen.MainMenuButton.onClick.RemoveListener(ToMainMenu);
            _gameOverMasterScreen.ExitGameButton.onClick.RemoveListener(ExitFromGame);

            _gameWonScreen.MainMenuButton.onClick.RemoveListener(ToMainMenu);
            _gameWonScreen.ExitGameButton.onClick.RemoveListener(ExitFromGame);

            GameStateManager.OnGameOver -= ShowGameOver;
        }

 
        private void Pause()
        {
            if (_isGameOver)
                return;

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

        public void ShowDeadScreen()
        {
            if (_isGameOver)
                return;

            _playerDeadView.Show();

            _pauseMenu.Hide();
            _playerView.Hide();
            GameOverScreenHide();
            _gameWonScreen.Hide();

            LockCursor(true);
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
        
        private void RestartGame()
        {
            OnRestartGame?.Invoke();
        }

        public void ShowPlayerUI()
        {
            if (_isGameOver)
                return;

            _playerView.Show();

            _playerDeadView.Hide();
            _pauseMenu.Hide();
            GameOverScreenHide();
            _gameWonScreen.Hide();

            LockCursor(true);
        }

        public void ShowPauseMenuUI()
        {
            if (_isGameOver)
                return;

            _pauseMenu.Show();

            _playerView.Hide();
            _playerDeadView.Hide();
            GameOverScreenHide();
            _gameWonScreen.Hide();

            LockCursor(false);
        }

        private void ShowGameOver()
        {
            _isGameOver = true;

            GameOverScreenShow();

            _pauseMenu.Hide();
            _playerView.Hide();
            _playerDeadView.Hide();
            _gameWonScreen.Hide();

            LockCursor(false);
        }

        private void GameOverScreenShow()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _gameOverMasterScreen.Show();
            }
            else
            {
                _gameOverClientScreen.Show();
            }

        }
        private void GameOverScreenHide()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _gameOverMasterScreen.Hide();
            }
            else
            {
                _gameOverClientScreen.Hide();
            }
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
