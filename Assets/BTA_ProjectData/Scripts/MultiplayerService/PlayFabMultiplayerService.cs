using Abstraction;
using System;
using PlayFab;
using PlayFab.ClientModels;
using Configs;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerService
{
    public class PlayFabMultiplayerService : IMultiplayerService
    {
        private readonly string _titleId;
        private readonly string _mainCatalogVersion;

        public event Action<UserData> OnLogInSucceed;
        public event Action<string> OnLogInError;

        public event Action OnCreateAccountSucceed;
        public event Action<string> OnCreateAccountError;

        public event Action<UserData> OnGetAccountSuccess;
        public event Action<string> OnGetAccountFailure;

        private UserData _tempUserData;

        public PlayFabMultiplayerService(GameConfig gameConfig)
        {
            _titleId = gameConfig.PlayFabTitleId;
            _mainCatalogVersion = gameConfig.ItemsInfoConfig.CatalogVersion;

            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            {
                PlayFabSettings.staticSettings.TitleId = _titleId;
            }
        }

        public void CreateAccount(UserData data)
        {
            var request = new RegisterPlayFabUserRequest
            {
                Username = data.UserName,
                Password = data.Password,
                Email = data.UserEmail,
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, CreateAccountSuccess, CreateAccountError);
        }

        private void CreateAccountSuccess(RegisterPlayFabUserResult result)
        {
            OnCreateAccountSucceed?.Invoke();
        }

        private void CreateAccountError(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            OnCreateAccountError?.Invoke(errorMessage);
        }

        public void LogIn(UserData data)
        {
            _tempUserData = data;

            var request = new LoginWithPlayFabRequest
            {
                Username = data.UserName,
                Password = data.Password
            };

            PlayFabClientAPI.LoginWithPlayFab(request, LogInSuccess, LogInError);
        }

        public void LogIn(string userId, bool needCreation)
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = userId,
                CreateAccount = needCreation,
            };

            PlayFabClientAPI.LoginWithCustomID(request, LogInSuccess, LogInError);
        }

        private void LogInSuccess(LoginResult result)
        {
            if(_tempUserData != null)
            {
                var userData = new UserData
                {
                    Id = result.PlayFabId,
                    UserName = _tempUserData.UserName,
                    Password = _tempUserData.Password
                };

                OnLogInSucceed?.Invoke(userData);

                _tempUserData = null;
            } 
        }

        private void LogInError(PlayFabError error)
        {
            _tempUserData = null;

            var errorMessage = error.GenerateErrorReport();
            OnLogInError?.Invoke(errorMessage);
        }


        public void GetAccountInfo(string userId)
        {
            var request = new GetAccountInfoRequest
            {
                PlayFabId = userId
            };

            PlayFabClientAPI.GetAccountInfo(request, GetAccountSuccess, GetAccountFailure);
        }


        private void GetAccountSuccess(GetAccountInfoResult result)
        {
            var userData = new UserData
            {
                Id = result.AccountInfo.PlayFabId,
                UserName = result.AccountInfo.Username,
                CreatedTime = result.AccountInfo.Created
            };

            OnGetAccountSuccess?.Invoke(userData);
        }

        private void GetAccountFailure(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            OnGetAccountFailure?.Invoke(errorMessage);
        }

        public void GetAvilableUserItems(string playfabId)
        {
            var authContex = new PlayFabAuthenticationContext
            {
                PlayFabId = playfabId
            };

            var request = new GetCatalogItemsRequest
            {
                 CatalogVersion = _mainCatalogVersion,
            };

            PlayFabClientAPI.GetCatalogItems(request, GetCatalogItemsSuccess, GetCatalogFailure);
        }

        public event Action<IList<CatalogItem>> OnGetCatalogItemsSuccess;
        public event Action<string> OnGetCatalogFailure;
        private void GetCatalogItemsSuccess(GetCatalogItemsResult result)
        {
            Debug.Log($"Catalog was loaded successfully!");

            var catalogItems = new List<CatalogItem>();

            for(int i =0; i < result.Catalog.Count; i++)
            {
                var item = result.Catalog[i];
                catalogItems.Add(item);
            }

            OnGetCatalogItemsSuccess?.Invoke(catalogItems);
        }

        private void GetCatalogFailure(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            OnGetCatalogFailure?.Invoke(errorMessage);
        }  
    }
}
