using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace UI
{
    public class SignInAccountDataUI : BaseAccountDataUI
    {
        protected override void SubscribeUI()
        {
            base.SubscribeUI();
        }

        protected override void AccountProceedAction()
        {
            base.AccountProceedAction();

            var request = new LoginWithPlayFabRequest
            {
                Username = _username,
                Password = _password
};

            PlayFabClientAPI.LoginWithPlayFab(request, OnSuccess, OnError);
        }

        private void OnSuccess(LoginResult result)
        {
            var resultMessage = $"[{result.InfoResultPayload.AccountInfo.Username}] - Login Complete";
            Debug.Log(resultMessage);

        }
        private void OnError(PlayFabError error)
        {
            var resultMessage = error.GenerateErrorReport();

            Debug.LogError(resultMessage);
        }
    }
}
