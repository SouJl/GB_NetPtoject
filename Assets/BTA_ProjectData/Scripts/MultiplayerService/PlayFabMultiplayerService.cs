﻿using Abstraction;
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
        public event Action OnCreateAccountSucceed;
        public event Action<UserData> OnGetAccountSuccess;
        public event Action<IList<CatalogItem>> OnGetCatalogItemsSuccess;

        public event Action<string> OnError;

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

            PlayFabClientAPI.RegisterPlayFabUser(request, CreateAccountSuccess, OnGetError);
        }

        private void CreateAccountSuccess(RegisterPlayFabUserResult result)
        {
            OnCreateAccountSucceed?.Invoke();
        }

        public void LogIn(UserData data)
        {
            _tempUserData = data;

            var request = new LoginWithPlayFabRequest
            {
                Username = data.UserName,
                Password = data.Password
            };

            PlayFabClientAPI.LoginWithPlayFab(request, LogInSuccess, OnGetError);
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

        public void GetAccountInfo(string userId)
        {
            var request = new GetAccountInfoRequest
            {
                PlayFabId = userId
            };

            PlayFabClientAPI.GetAccountInfo(request, GetAccountSuccess, OnGetError);
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

        public void GetCatalogItems()
        {
            var request = new GetCatalogItemsRequest
            {
                CatalogVersion = _mainCatalogVersion
            };

            PlayFabClientAPI.GetCatalogItems(request, GetCatalogItemsSuccess, OnGetError);
        }

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


        private void OnGetError(PlayFabError error)
        {
            _tempUserData = null;

            var errorMessage = error.GenerateErrorReport();
            OnError?.Invoke(errorMessage);
        }

    }
}
