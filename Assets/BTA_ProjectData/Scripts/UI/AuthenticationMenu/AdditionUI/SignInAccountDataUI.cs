using Abstraction;
using UnityEngine;

namespace UI
{
    public class SignInAccountDataUI : BaseAccountDataUI
    {
        protected override void SubscribeUI()
        {
            base.SubscribeUI();
        }

        protected override void UnsubscribeUI()
        {
            base.UnsubscribeUI();
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

            return new UserAccountData
            {
                Name = _username,
                Password = _password
            };
        }
    }
}
