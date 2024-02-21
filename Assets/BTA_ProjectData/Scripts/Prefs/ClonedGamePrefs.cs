using Abstraction;
using Enumerators;
using System;

namespace Prefs
{
    public class ClonedGamePrefs : IGamePrefs
    {
        private const string clonedUserName = "ClonedTester";
        private const string clonedUserPassword = "000000";

        private GameState _gameState;

        private bool _isSettedGameName;
        private string _settedGamName;

        public string PlayFabId { get; set; }
        
        public bool IsUserDataExist => true;
        public bool IsPlayerDataExist => true;
        public bool IsSettedGameName => _isSettedGameName;
        
        public string SettedGamName => _settedGamName; 

        public event Action<GameState> OnGameStateChange;

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

        }
        public void SetUser(IGameUser user)
        {

        }

        public IGameUser GetUser()
        {
            return new BaseGameUser
            {
                Name = clonedUserName,
                Password = clonedUserPassword
            };
        }

        public void LoadPlayer()
        {
            
        }

        public void SetPlayer(IGamePlayer player)
        {

        }

        public IGamePlayer GetPlayer()
        {
            return null;
        }


        public void DeleteData() { } 
    }
}
