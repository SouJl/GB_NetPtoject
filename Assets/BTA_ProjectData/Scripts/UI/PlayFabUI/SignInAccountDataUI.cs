using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace UI
{
    public class SignInAccountDataUI : BaseAccountDataUI
    {
        private LogInProgressSlider _logInProgress;

        public void InitUI(LogInProgressSlider logInProgress)
        {
            _logInProgress = logInProgress;
            _logInProgress.Init();
        }

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

            _logInProgress.StartProgress();
        }

        private void OnSuccess(LoginResult result)
        {
            var resultMessage = $"[{result.PlayFabId}] - Login Complete";
            Debug.Log(resultMessage);

            _logInProgress.Stop();
        }
        private void OnError(PlayFabError error)
        {
            var resultMessage = error.GenerateErrorReport();

            Debug.LogError(resultMessage);

            _logInProgress.Stop();
        }

        public override void UpdateUI(float deltaTime)
        {
            _logInProgress.UpdateProgress(deltaTime);
        }
    }
}
