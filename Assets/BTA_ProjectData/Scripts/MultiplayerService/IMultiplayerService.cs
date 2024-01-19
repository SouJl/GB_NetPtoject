﻿using Abstraction;
using System;

namespace MultiplayerService
{
    public interface IMultiplayerService
    {
        public event Action<string> OnLogInSucceed;
        public event Action<string> OnLogInError;

        public event Action OnCreateAccountSucceed;
        public event Action<string> OnCreateAccountError;

        public event Action<UserData> OnGetAccountSuccess;
        public event Action<string> OnGetAccountFailure;

        public void LogIn(UserData data);
        public void LogIn(string userId);

        public void CreateAccount(UserData data);

        public void GetAccountInfo(string userId);
    }
}
