using Abstraction;
using Enumerators;
using System;
using UnityEngine;

namespace Prefs
{
    public class GamePrefs : IGamePrefs
    {
        private UserPrefs _userPrefs;
        private BTAPlayerPrefs _playerPrefs;

        private GameState _gameState;

        private bool _isUserDataExist;
        private bool _isPlayerDataExist;
        private bool _isSettedGameName;
        
        private string _settedGamName;

        public string PlayFabId { get; set; }

        public bool IsUserDataExist => _isUserDataExist;
        public bool IsPlayerDataExist => _isPlayerDataExist;
        public bool IsSettedGameName => _isSettedGameName;

        public string SettedGamName => _settedGamName;

        public event Action<GameState> OnGameStateChange;

        public GamePrefs()
        {
            _userPrefs = new UserPrefs();
            _playerPrefs = new BTAPlayerPrefs();
        }

        public void ChangeGameState(GameState gameState)
        {
            _gameState = gameState;

            OnGameStateChange?.Invoke(_gameState);
        }

        public void SetGame(string gameName)
        {
            _isSettedGameName = true;
            _settedGamName = gameName;
        }

        public void LoadData()
        {
            LoadUser();
            LoadPlayer();
        }

        public void LoadUser()
        {
            _isUserDataExist = _userPrefs.Load();
        }

        public void SetUser(IGameUser user)
        {
            _userPrefs.Save(user);
        }

        public IGameUser GetUser()
        {
            return _userPrefs;
        }

        public void LoadPlayer()
        {
            _isPlayerDataExist = _playerPrefs.Load();
        }

        public void SetPlayer(IGamePlayer player)
        {
            _playerPrefs.Save(player);
        }

        public IGamePlayer GetPlayer()
        {
            return _playerPrefs;
        }

        public void DeleteData()
        {
            PlayerPrefs.DeleteAll();
            
            _isUserDataExist = false;
            _isPlayerDataExist = false;
        }
    }
}

