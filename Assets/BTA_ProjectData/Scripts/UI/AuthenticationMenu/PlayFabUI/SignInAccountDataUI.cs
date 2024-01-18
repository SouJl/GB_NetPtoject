using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

namespace UI
{
    public class SignInAccountDataUI : BaseAccountDataUI
    {
        public event Action OnConnectionStart;
        public event Action OnConnectionEnd;

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

            OnConnectionStart?.Invoke();
        }

        private void OnSuccess(LoginResult result)
        {
            var resultMessage = $"[{result.PlayFabId}] - Login Complete";
            Debug.Log(resultMessage);

            OnConnectionEnd?.Invoke();
        }
        private void OnError(PlayFabError error)
        {
            var resultMessage = error.GenerateErrorReport();

            Debug.LogError(resultMessage);

            OnConnectionEnd?.Invoke();
        }
    }
}
