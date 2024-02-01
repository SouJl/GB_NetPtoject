using Abstraction;
using Enumerators;
using ParrelSync;
using System;
using UnityEngine;

namespace Prefs
{
    public class GamePrefs
    {
        private const string AuthUserId = "authorization_user_id";
        private const string AuthUserName = "authorization_user_name";
        private const string AuthUserPassword = "authorization_user_passw";

        private GameState _gameState;

        private bool _isUserDataExist;
        private string _userId;
        private string _userName;
        private string _userPassword;

        public bool IsUserDataExist => _isUserDataExist;
        public string UserId => _userId;
        public string UserName => _userName;
        public string UserPassword => _userPassword;

        public event Action<GameState> OnGameStateChange;

        private bool _isClone;

        public GamePrefs()
        {
            _isClone = ClonesManager.IsClone();
        }

        public void Save()
        {
            if (_isClone)
            {
                return;
            }

            PlayerPrefs.SetString(AuthUserId, _userId);
            PlayerPrefs.SetString(AuthUserName, _userName);
            PlayerPrefs.SetString(AuthUserPassword, _userPassword);
        }

        public bool Load()
        {
            if (_isClone)
            {
                return false;
            }

            _isUserDataExist = CheckDataExist();

            if (_isUserDataExist == false)
                return false;

            _userId = PlayerPrefs.GetString(AuthUserId);
            _userName = PlayerPrefs.GetString(AuthUserName);
            _userPassword = PlayerPrefs.GetString(AuthUserPassword);

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
            if (_isClone)
            {
                return;
            }

            PlayerPrefs.DeleteAll();

            _isUserDataExist = false;
        }

        public void ChangeGameState(GameState gameState)
        {
            _gameState = gameState;

            OnGameStateChange?.Invoke(_gameState);
        }

        public void SetUserData(UserData userData)
        {
            _userId = userData.Id;
            _userName = userData.UserName;
            _userPassword = userData.Password;

            Save();
        }

        public UserData GetUserData()
        {
            var userData = new UserData
            {
                Id = _userId,
                UserName = _userName,
                Password = _userPassword
            };

            return userData;
        }
    }
}

