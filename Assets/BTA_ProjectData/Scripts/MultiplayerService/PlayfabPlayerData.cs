using Abstraction;

namespace MultiplayerService
{
    public class PlayfabPlayerData : IGamePlayer
    {
        public string Nickname { get; set; }
        public int CurrentLevel { get; set; }
        public float CurrentLevelProgress { get; set; }
    }
}
