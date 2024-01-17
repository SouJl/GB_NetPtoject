using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class BaseAccountDataUI : MonoBehaviour
    {
        [SerializeField]
        private InputField _usernameField;

        [SerializeField]
        private InputField _passwordField;

        protected string _username;
        protected string _password;

        private void Start()
        {
            SubscribeUI();
        }

        protected virtual void SubscribeUI()
        {
            _usernameField.onValueChanged.AddListener(ChangeUsername);
            _passwordField.onValueChanged.AddListener(ChangePassword);
        }

        private void ChangeUsername(string username)
        {
            _username = username;
            Debug.Log(_username);
        }

        private void ChangePassword(string password)
        {
            _password = password;
            Debug.Log(_password);
        }
    }
}
