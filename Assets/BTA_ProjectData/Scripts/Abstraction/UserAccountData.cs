using System;

namespace Abstraction
{
    public class UserAccountData : IGameUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
