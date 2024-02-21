using Abstraction;
using Enumerators;
using System;

namespace BTAPlayer
{
    public interface IPlayerController : IOnUpdate, IDisposable
    {
        public string PlayerId { get; }

        public PlayerState State { get; }
        public float MaxHealth { get; }
        public float CurrentHealth { get; set; }
        public int PlayerLevel { get; set; }

        public float DamageDistance { get; }

        public void ExecuteFixedUpdate(float fixedTime);

        public void ChangeHealthValue(float value);

        public void ChangeState(PlayerState state);
    }
}
