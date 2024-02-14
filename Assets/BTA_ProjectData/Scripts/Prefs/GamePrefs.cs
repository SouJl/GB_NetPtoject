using Abstraction;
using Enumerators;
using System;
using UnityEngine;

namespace Prefs
{
    public class GamePrefs : IGamePrefs
    {
        private const string AuthUserId = "authorization_user_id";
        private const string AuthUserName = "authorization_user_name";
        private const string AuthUserPassword = "authorization_user_passw";

        private const string UserCurrentLevel = "user_current_lvl";
        private const string UserCurrLevelProgress = "user_currlvl_progress";

        private GameState _gameState;

        private UserPrefs _userPrefs;

        private bool _isUserDataExist;
        private bool _isSettedGameName;

        private UserData _data;
        private string _settedGamName;

        public bool IsUserDataExist => _isUserDataExist;
        public bool IsSettedGameName => _isSettedGameName;
        public UserData Data => _data;
        public string SettedGamName => _settedGamName;

   
        public event Action<GameState> OnGameStateChange;

        public GamePrefs()
        {
            _userPrefs = new UserPrefs();
        }

        public void Save()
        {
            PlayerPrefs.SetString(AuthUserId, _data.Id);
            PlayerPrefs.SetString(AuthUserName, Data.UserName);
            PlayerPrefs.SetString(AuthUserPassword, Data.Password);
            PlayerPrefs.SetInt(UserCurrentLevel, Data.CurrentLevel);
            PlayerPrefs.SetFloat(UserCurrLevelProgress, Data.CurrLevelProgress);
        }

        public bool Load()
        {

            _isUserDataExist = CheckDataExist();

            if (_isUserDataExist == false)
                return false;

            var userId = PlayerPrefs.GetString(AuthUserId);
            var userName = PlayerPrefs.GetString(AuthUserName);
            var userPassword = PlayerPrefs.GetString(AuthUserPassword);
            var userLevel = PlayerPrefs.GetInt(UserCurrentLevel);
            var userLevelProgress = PlayerPrefs.GetFloat(UserCurrLevelProgress);

            _data = new UserData
            {
                Id = userId,
                UserName = userName,
                Password = userPassword,
                CurrentLevel = userLevel,
                CurrLevelProgress = userLevelProgress
            };

            return true;
        }

        public void LoadUser()
        {
            _isUserDataExist = _userPrefs.Load();
        }

        public IGameUser GetUser()
        {
            return _userPrefs;
        }

        private bool CheckDataExist()
        {
            if (PlayerPrefs.HasKey(AuthUserId) == false)
                return false;
            if (PlayerPrefs.HasKey(AuthUserName) == false)
                return false;
            if (PlayerPrefs.HasKey(AuthUserPassword) == false)
                return false;
            if (PlayerPrefs.HasKey(UserCurrentLevel) == false)
                return false;
            if (PlayerPrefs.HasKey(UserCurrLevelProgress) == false)
                return false;
            return true;
        }

        public void DeleteData()
        {
            PlayerPrefs.DeleteAll();
            
            _data = null;

            _isUserDataExist = false;
        }

        public void ChangeGameState(GameState gameState)
        {
            _gameState = gameState;

            OnGameStateChange?.Invoke(_gameState);
        }

        public void SetUserData(UserData userData)
        {
            _data = userData;
            Save();
        }

        public void SetGame(string gameName)
        {
            _isSettedGameName = true;
            _settedGamName = gameName;
        }

        public void SetUserProgression(int level, float progress)
        {
            _data.CurrentLevel = level;
            _data.CurrLevelProgress = progress;
        }
    }
}

