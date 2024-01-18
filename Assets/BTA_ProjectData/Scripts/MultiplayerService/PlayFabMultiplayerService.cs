using Abstraction;
using System;
using PlayFab;
using PlayFab.ClientModels;


namespace MultiplayerService
{
    public class PlayFabMultiplayerService : IMultiplayerService
    {
        public event Action OnLogInInitialize;
        public event Action OnLogInSucceed;
        public event Action OnLogInError;

        public event Action OnCreateAccountInitialize;
        public event Action OnCreateAccountSucceed;
        public event Action OnCreateAccountError;

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
            
            OnCreateAccountInitialize?.Invoke();
        }

        private void CreateAccountSuccess(RegisterPlayFabUserResult result)
        {
            OnCreateAccountSucceed?.Invoke();
        }

        private void CreateAccountError(PlayFabError error)
        {
            OnCreateAccountError?.Invoke();
        }

        public void LogIn(UserData data)
        {
            var request = new LoginWithPlayFabRequest
            {
                Username = data.UserName,
                Password = data.Password,
            };

            PlayFabClientAPI.LoginWithPlayFab(request, LogInSuccess, LogInError);

            OnLogInInitialize?.Invoke();
        }

        private void LogInSuccess(LoginResult result)
        {
            OnLogInSucceed?.Invoke();
        }

        private void LogInError(PlayFabError error)
        {
            OnLogInError?.Invoke();
        }

        public void LogIn(string userId)
        {
            OnLogInInitialize?.Invoke();
        }
    }
}
