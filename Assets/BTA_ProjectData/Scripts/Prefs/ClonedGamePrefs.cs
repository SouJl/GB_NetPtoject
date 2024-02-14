﻿using Abstraction;
using Enumerators;
using System;

namespace Prefs
{
    public class ClonedGamePrefs : IGamePrefs
    {
        private const string clonedUserId = "476DB455EF4F838A";
        private const string clonedUserName = "ClonedTester";
        private const string clonedUserPassword = "000000";

        private GameState _gameState;

        private bool _isSettedGameName;
        private bool _isPlayerDataExist;
        private UserData _data;
        private UserPrefs _userPrefs;
        private string _settedGamName;

        public string PlayFabId { get; set; }
        
        public bool IsUserDataExist => true;
        public bool IsPlayerDataExist => true;

        public bool IsSettedGameName => _isSettedGameName;
        public UserData Data => _data;
        public string SettedGamName => _settedGamName; 

        public event Action<GameState> OnGameStateChange;

        public void Save() { }

        public bool Load()
        {
            _data = new UserData
            {
                Id = clonedUserId,
                UserName = clonedUserName,
                Password = clonedUserPassword,
            };

            return true;
        }
        
        public void LoadUser()
        {

        }

        public IGameUser GetUser()
        {
            return _userPrefs;
        }

        public IGamePlayer GetPlayer()
        {
            return null;
        }

        public void DeleteData() { }

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

        public void LoadPlayer()
        {
            throw new NotImplementedException();
        }
    }
}
