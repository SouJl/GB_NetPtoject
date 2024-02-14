using Abstraction;
using Enumerators;
using System;
using UnityEngine;

namespace Prefs
{
    public class GamePrefs : IGamePrefs
    {
        private const string UserCurrentLevel = "user_current_lvl";
        private const string UserCurrLevelProgress = "user_currlvl_progress";

        private GameState _gameState;

        private UserPrefs _userPrefs;
        private BTAPlayerPrefs _playerPrefs;

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
        }

        public void Save()
        {
          
        }

        public bool Load()
        {

            _isUserDataExist = CheckDataExist();

            if (_isUserDataExist == false)
                return false;


            return true;
        }

        public void LoadUser()
        {
            _isUserDataExist = _userPrefs.Load();
        }

        public void LoadPlayer()
        {
            _isPlayerDataExist = _playerPrefs.Load();
        }

        public IGameUser GetUser()
        {
            return _userPrefs;
        }

        public IGamePlayer GetPlayer()
        {
            return _playerPrefs;
        }

        private bool CheckDataExist()
        {
            if (PlayerPrefs.HasKey(UserCurrentLevel) == false)
                return false;
            if (PlayerPrefs.HasKey(UserCurrLevelProgress) == false)
                return false;
            return true;
        }

        public void DeleteData()
        {
            PlayerPrefs.DeleteAll();
            

            _isUserDataExist = false;
            _isPlayerDataExist = false;
        }

        public void ChangeGameState(GameState gameState)
        {
            _gameState = gameState;

            OnGameStateChange?.Invoke(_gameState);
        }

        public void SetUserData(UserData userData)
        {
            Save();
        }

        public void SetGame(string gameName)
        {
            _isSettedGameName = true;
            _settedGamName = gameName;
        }

        public void SetUserProgression(int level, float progress)
        {
         
        }
    }
}

