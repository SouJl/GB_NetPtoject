using Abstraction;
using Enumerators;
using System;

namespace Prefs
{
    public interface IGamePrefs
    {
        public bool IsUserDataExist { get; }
        public bool IsSettedGameName { get; }
        public string SettedGamName { get; }
        public UserData Data { get; }

        public event Action<GameState> OnGameStateChange;

        public void Save();

        public bool Load();

        public void DeleteData();

        public void ChangeGameState(GameState gameState);

        public void SetUserData(UserData userData);

        public void SetGame(string gameName);

        public void SetUserProgression(int level, float progress);

        public void LoadUser();

        public IGameUser GetUser();
    }
}
