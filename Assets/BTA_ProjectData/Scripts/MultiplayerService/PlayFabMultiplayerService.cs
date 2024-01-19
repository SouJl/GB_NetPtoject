using Abstraction;
using System;
using PlayFab;
using PlayFab.ClientModels;


namespace MultiplayerService
{
    public class PlayFabMultiplayerService : IMultiplayerService
    {
        public event Action<string> OnLogInSucceed;
        public event Action<string> OnLogInError;

        public event Action OnCreateAccountSucceed;
        public event Action<string> OnCreateAccountError;

        public event Action<UserData> OnGetAccountSuccess;
        public event Action<string> OnGetAccountFailure;

        public PlayFabMultiplayerService(string titleId)
        {
            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            {
                PlayFabSettings.staticSettings.TitleId = titleId;
            }
        }

        public void CreateAccount(UserData data)
        {
            var request = new RegisterPlayFabUserRequest
            {
                Username = data.UserName,
                Password = data.Password,
                Email = data.UserEmail
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
            var request = new LoginWithPlayFabRequest
            {
                Username = data.UserName,
                Password = data.Password,
            };

            PlayFabClientAPI.LoginWithPlayFab(request, LogInSuccess, LogInError);
        }

        public void LogIn(string userId)
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = userId,
            };

            PlayFabClientAPI.LoginWithCustomID(request, LogInSuccess, LogInError);
        }

        private void LogInSuccess(LoginResult result)
        {
            OnLogInSucceed?.Invoke(result.PlayFabId);
        }

        private void LogInError(PlayFabError error)
        {
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
    }
}
