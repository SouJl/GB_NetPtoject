using Abstraction;
using System;
using PlayFab;
using PlayFab.ClientModels;
using Configs;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Prefs;

namespace MultiplayerService
{
    public class DataServerService 
    {
        private string _titleId;

        public event Action<string> OnLogInSucceed;
        public event Action<string> OnCreateAccountSucceed;
        public event Action<UserAccountData> OnGetAccountSuccess;
        public event Action<List<CatalogItem>> OnGetCatalogItemsSuccess;

        public event Action OnSetDataSucceed;
        public event Action<PlayfabPlayerData> OnGetUserData;

        public event Action<PlayFabErrorCode, string> OnError;

        public bool IsLogIn 
            => PlayFabClientAPI.IsClientLoggedIn();

        public DataServerService()
        {

        }

        public DataServerService(GameConfig gameConfig)
        {
            _titleId = gameConfig.PlayFabTitleId;

            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            {
                PlayFabSettings.staticSettings.TitleId = _titleId;
            }
        }

        public void CreateAccount(UserAccountData data)
        {
            var request = new RegisterPlayFabUserRequest
            {
                Username = data.Name,
                Password = data.Password,
                Email = data.Email,
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, CreateAccountSuccess, OnGetError);
        }

        private void CreateAccountSuccess(RegisterPlayFabUserResult result)
        {
            OnCreateAccountSucceed?.Invoke(result.PlayFabId);
        }

        public void LogIn(IGameUser user)
        {
            var request = new LoginWithPlayFabRequest
            {
                Username = user.Name,
                Password = user.Password
            };

            PlayFabClientAPI.LoginWithPlayFab(request, LogInSuccess, OnGetError);
        }

        public void LogIn(UserAccountData data)
        {
            var request = new LoginWithPlayFabRequest
            {
                Username = data.Name,
                Password = data.Password
            };

            PlayFabClientAPI.LoginWithPlayFab(request, LogInSuccess, OnGetError);
        }

        private void LogInSuccess(LoginResult result)
        {
            OnLogInSucceed?.Invoke(result.PlayFabId);
        }

        public void GetAccountData(string userId)
        {
            var request = new GetAccountInfoRequest
            {
                PlayFabId = userId
            };

            PlayFabClientAPI.GetAccountInfo(request, GetAccountSuccess, OnGetError);
        }


        private void GetAccountSuccess(GetAccountInfoResult result)
        {
            var userData = new UserAccountData
            {
                Id = result.AccountInfo.PlayFabId,
                Name = result.AccountInfo.Username,
                CreatedTime = result.AccountInfo.Created
            };

            OnGetAccountSuccess?.Invoke(userData);
        }

        public void GetCatalogItems(string catalogId)
        {
            var request = new GetCatalogItemsRequest
            {
                CatalogVersion = catalogId
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

        public void SetPlayerData(Dictionary<string, string> data)
        {
            if (PlayFabClientAPI.IsClientLoggedIn() == false)
                return;

            var request = new UpdateUserDataRequest
            {
                Data = data
            };

            PlayFabClientAPI.UpdateUserData(request, SetplayerDataSuccess, OnGetError);
        }


        private void SetplayerDataSuccess(UpdateUserDataResult result)
        {
            Debug.Log($"SetUserDataSuccess!");

            OnSetDataSucceed?.Invoke();
        }

        public void GetPlayerData()
        {
            if (PlayFabClientAPI.IsClientLoggedIn() == false)
                return;

            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), GetUserDataSuccess, OnGetError);
        }

        private void GetUserDataSuccess(GetUserDataResult result)
        {
            if (result.Data == null)
                return;

            var userNickName = result.Data[BTAConst.USER_NICKNAME].Value;
            var userlevel = int.Parse(result.Data[BTAConst.USER_GAME_LVL].Value);
            var userlevelProgress = float.Parse(result.Data[BTAConst.USER_LVL_PROGRESS].Value);

            OnGetUserData?.Invoke(new PlayfabPlayerData
            {
                Nickname = userNickName,
                CurrentLevel = userlevel,
                CurrentLevelProgress = userlevelProgress
            });
        }

        private void OnGetError(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            OnError?.Invoke(error.Error, errorMessage);
        }
    }
}
