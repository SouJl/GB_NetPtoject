using Abstraction;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

namespace MultiplayerService
{
    public interface IMultiplayerService
    {
        public event Action<UserData> OnLogInSucceed;
        public event Action OnCreateAccountSucceed;
        public event Action<UserData> OnGetAccountSuccess;
        public event Action<IList<CatalogItem>> OnGetCatalogItemsSuccess;

        public event Action<string> OnError;

        public void LogIn(UserData data);

        public void CreateAccount(UserData data);

        public void GetAccountInfo(string userId);

        public void GetCatalogItems();
    }
}
