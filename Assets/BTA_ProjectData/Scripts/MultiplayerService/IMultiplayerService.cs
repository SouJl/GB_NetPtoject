using Abstraction;
using Configs;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

namespace MultiplayerService
{
    public interface IMultiplayerService
    {
        public event Action<UserData> OnLogInSucceed;
        public event Action<UserData> OnCreateAccountSucceed;
        public event Action<UserData> OnGetAccountSuccess;
        public event Action<List<CatalogItem>> OnGetCatalogItemsSuccess;

        public event Action<string> OnError;

        public void LogIn(UserData data);

        public void CreateAccount(UserData data);

        public void GetAccountInfo(string userId);

        public void GetCatalogItems(string catalogId);

        public void SetUserData(Dictionary<string, string> data);

        public void GetUserData(string userId);
    }
}
