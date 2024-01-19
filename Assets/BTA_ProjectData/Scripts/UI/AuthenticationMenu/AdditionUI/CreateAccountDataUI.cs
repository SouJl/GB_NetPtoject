using UnityEngine.UI;
using UnityEngine;
using Abstraction;

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

        protected override void UnsubscribeUI()
        {
            base.UnsubscribeUI();
            _userEmailField.onValueChanged.RemoveListener(ChangeUserEmail);
        }

        private void ChangeUserEmail(string userEmail)
        {
            _userEmail = userEmail;
        }

        protected override UserData GetUserData()
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

            return new UserData
            {
                UserName = _username,
                Password = _password,
                UserEmail = _userEmail
            };
        }
    }
}
