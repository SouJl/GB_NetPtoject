using Abstraction;
using Enumerators;
using System;

namespace Prefs
{
    public class ClonedGamePrefs : IGamePrefs
    {
        private const string clonedUserId = "28B6E54FE89BE10E";
        private const string clonedUserName = "UserTest";
        private const string clonedUserPassword = "qwe123";

        private GameState _gameState;

        private bool _isSettedGameName;

        private UserData _data;
        private string _settedGamName;

        public bool IsUserDataExist => true;
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
    }
}
