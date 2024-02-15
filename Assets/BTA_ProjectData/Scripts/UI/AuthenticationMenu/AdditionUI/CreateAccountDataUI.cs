using UnityEngine;
using Abstraction;
using TMPro;

namespace UI
{
    public class CreateAccountDataUI : BaseAccountDataUI
    {
        [SerializeField]
        private TMP_InputField _userEmailField;

        private string _userEmail;

        protected override void SubscribeUI()
        {
            base.SubscribeUI();
            _userEmailField.onValueChanged.AddListener(ChangeUserEmail);
        }

        protected override void UnsubscribeUI()
        {
            base.UnsubscribeUI();
            _userEmailField.onValueChanged.RemoveListener(ChangeUserEmail);
        }

        private void ChangeUserEmail(string userEmail)
        {
            _userEmail = userEmail;
        }

        protected override UserAccountData GetUserData()
        {
            if (_username == null || _username == "")
            {
                Debug.Log("Username is null or empty");
                return null;
            }
            if (_password == null || _password == "")
            {
                Debug.Log("Password is null or empty");
                return null;
            }
            if (_userEmail == null || _userEmail == "")
            {
                Debug.Log("Email is null or empty");
                return null;
            }

            return new UserAccountData
            {
                Name = _username,
                Password = _password,
                Email = _userEmail
            };
        }
    }
}
