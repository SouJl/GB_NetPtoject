namespace Abstraction
{
    public class BaseGamePlayer : IGamePlayer
    {
        public string Nickname { get; set; }

        public int CurrentLevel { get; set; }

        public float CurrentLevelProgress { get; set; }
    }
}
