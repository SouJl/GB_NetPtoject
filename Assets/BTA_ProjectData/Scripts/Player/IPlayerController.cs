using Abstraction;
using System;

namespace BTAPlayer
{
    public interface IPlayerController : IOnUpdate, IDisposable
    {
        public string PlayerId { get; }
        public float CurrentHealth { get; set; }
        public int PlayerLevel { get; set; }

        public void ExecuteFixedUpdate(float fixedTime);

        public void ChangeHealthValue(float value);
    }
}
