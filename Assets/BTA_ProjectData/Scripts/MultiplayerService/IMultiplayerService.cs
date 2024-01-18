using Abstraction;
using System;

namespace MultiplayerService
{
    public interface IMultiplayerService
    {
        public event Action OnLogInInitialize;
        public event Action OnLogInSucceed;
        public event Action OnLogInError;

        public event Action OnCreateAccountInitialize;
        public event Action OnCreateAccountSucceed;
        public event Action OnCreateAccountError;

        public void LogIn(UserData data);
        public void LogIn(string userId);

        public void CreateAccount(UserData data);
    }
}
