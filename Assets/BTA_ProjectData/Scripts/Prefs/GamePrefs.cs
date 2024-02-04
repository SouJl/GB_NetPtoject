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

        private GameState _gameState;

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
            
        }

        public void Save()
        {
            PlayerPrefs.SetString(AuthUserId, _data.Id);
            PlayerPrefs.SetString(AuthUserName, Data.UserName);
            PlayerPrefs.SetString(AuthUserPassword, Data.Password);
        }

        public bool Load()
        {

            _isUserDataExist = CheckDataExist();

            if (_isUserDataExist == false)
                return false;

            var userId = PlayerPrefs.GetString(AuthUserId);
            var userName = PlayerPrefs.GetString(AuthUserName);
            var userPassword = PlayerPrefs.GetString(AuthUserPassword);

            _data = new UserData
            {
                Id = userId,
                UserName = userName,
                Password = userPassword,
            };

            return true;
        }

        private bool CheckDataExist()
        {
            if (PlayerPrefs.HasKey(AuthUserId) == false)
                return false;
            if (PlayerPrefs.HasKey(AuthUserName) == false)
                return false;
            if (PlayerPrefs.HasKey(AuthUserPassword) == false)
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
    }
}

