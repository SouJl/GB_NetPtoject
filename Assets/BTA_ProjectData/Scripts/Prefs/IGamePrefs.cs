using Abstraction;
using Enumerators;
using System;
using UnityEngine;

namespace Prefs
{
    public interface IGamePrefs
    {
        public string PlayFabId { get; set; }

        public AudioSource BacgroundMusic { get; set; }
        public AudioSource ClickSound { get; set; }
        public bool IsUserDataExist { get; }
        public bool IsPlayerDataExist { get; }

        public bool IsSettedGameName { get; }
        public string SettedGamName { get; }

        public event Action<GameState> OnGameStateChange;

        public void ChangeGameState(GameState gameState);

        public void SetGame(string gameName);
        public void LoadData();

        public void LoadUser();
        public void SetUser(IGameUser user);
        public IGameUser GetUser();

        public void LoadPlayer();
        public void SetPlayer(IGamePlayer player);
        public IGamePlayer GetPlayer();

        public void DeleteData();
    }
}
