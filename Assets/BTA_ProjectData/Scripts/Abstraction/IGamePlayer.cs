namespace Abstraction
{
    public interface IGamePlayer
    {
        public string Nickname { get; }
        public int CurrentLevel { get; }
        public float CurrentLevelProgress { get; }
    }
}
