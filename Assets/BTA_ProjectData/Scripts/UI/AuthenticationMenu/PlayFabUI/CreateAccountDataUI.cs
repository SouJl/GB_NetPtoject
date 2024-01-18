using UnityEngine.UI;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

namespace UI
{
    public class CreateAccountDataUI : BaseAccountDataUI
    {
        [SerializeField]
        private InputField _userEmailField;

        private string _userEmail;

        protected override void SubscribeUI()
        {
            base.SubscribeUI();
            _userEmailField.onValueChanged.AddListener(ChangeUserEmail);
        }

        private void ChangeUserEmail(string userEmail)
        {
            _userEmail = userEmail;
        }

        protected override void AccountProceedAction()
        {
            base.AccountProceedAction();

            if (_userEmail == null || _userEmail == "")
            {
                Debug.Log("Email is null or empty");
                return;
            }

            var request = new RegisterPlayFabUserRequest
            {
                Username = _username,
                Password = _password,
                Email = _userEmail
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, OnSuccess, OnError);
        }

        private void OnSuccess(RegisterPlayFabUserResult result)
        {
            Debug.Log($"Account {_username} created successful");
        }

        private void OnError(PlayFabError error)
        {
            Debug.LogError($"Account creation failed : {error.ErrorMessage}");
        }

        protected override void OnUIDestoy()
        {
            base.OnUIDestoy();
            _userEmailField.onValueChanged.RemoveListener(ChangeUserEmail);
        }
    }
}
