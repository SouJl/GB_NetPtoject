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

        public event Action<UserData> OnLogInSucceed;
        public event Action<UserData> OnCreateAccountSucceed;
        public event Action<UserData> OnGetAccountSuccess;
        public event Action<List<CatalogItem>> OnGetCatalogItemsSuccess;

        public event Action OnSetDataSucceed;
        public event Action<PlayfabUserData> OnGetUserData;

        public event Action<string> OnError;

        private UserData _tempUserData;

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

        public void CreateAccount(UserData data)
        {
            _tempUserData = data;

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
            if (_tempUserData != null)
            {
                var userData = new UserData
                {
                    Id = result.PlayFabId,
                    UserName = _tempUserData.UserName,
                    Password = _tempUserData.Password
                };

                OnCreateAccountSucceed?.Invoke(userData);

                _tempUserData = null;
            } 
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

        public void SetUserData(Dictionary<string, string> data)
        {
            if (PlayFabClientAPI.IsClientLoggedIn() == false)
                return;

            var request = new UpdateUserDataRequest
            {
                Data = data
            };

            PlayFabClientAPI.UpdateUserData(request, SetUserDataSuccess, OnGetError);
        }


        private void SetUserDataSuccess(UpdateUserDataResult result)
        {
            Debug.Log($"SetUserDataSuccess!");

            OnSetDataSucceed?.Invoke();
        }

        public void GetUserData(string userId)
        {
            var request = new GetUserDataRequest
            {
                PlayFabId = userId
            };

            PlayFabClientAPI.GetUserData(request, GetUserDataSuccess, OnGetError);
        }

        private void GetUserDataSuccess(GetUserDataResult result)
        {
            if (result.Data == null)
                return;

            var userNickName = result.Data[BTAConst.USER_NICKNAME].Value;
            var userlevel = int.Parse(result.Data[BTAConst.USER_GAME_LVL].Value);
            var userlevelProgress = float.Parse(result.Data[BTAConst.USER_LVL_PROGRESS].Value);

            OnGetUserData?.Invoke(new PlayfabUserData
            {
                Nickname = userNickName,
                Level = userlevel,
                LevelProgress = userlevelProgress
            });
        }

        private void OnGetError(PlayFabError error)
        {
            _tempUserData = null;

            var errorMessage = error.GenerateErrorReport();
            OnError?.Invoke(errorMessage);
        }
    }
}
