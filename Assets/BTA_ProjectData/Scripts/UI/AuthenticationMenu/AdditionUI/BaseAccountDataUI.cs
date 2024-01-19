using Abstraction;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public abstract class BaseAccountDataUI : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private TMP_InputField _usernameField;
        [SerializeField]
        private TMP_InputField _passwordField;
        [SerializeField]
        private Button _proceedButton;
        [SerializeField]
        private Button _returnButton;

        protected string _username;
        protected string _password;

        public event Action<UserData> OnProceed;
        public event Action<BaseAccountDataUI> OnReturn;

        private void Start()
        {
            SubscribeUI();
        }

        protected virtual void SubscribeUI()
        {
            _proceedButton.onClick.AddListener(Proceed);
            _returnButton.onClick.AddListener(Return);

            _usernameField.onValueChanged.AddListener(ChangeUsername);
            _passwordField.onValueChanged.AddListener(ChangePassword);   
        }
        protected virtual void UnsubscribeUI()
        {
            _proceedButton.onClick.RemoveListener(Proceed);
            _returnButton.onClick.RemoveListener(Return);

            _usernameField.onValueChanged.RemoveListener(ChangeUsername);
            _passwordField.onValueChanged.RemoveListener(ChangePassword);
        }

        private void Proceed()
        {
            var userData = GetUserData();
            OnProceed?.Invoke(userData);
        }

        protected abstract UserData GetUserData();

        private void Return()
        {
            OnReturn?.Invoke(this);
        }

        private void ChangeUsername(string username)
        {
            _username = username;
        }

        private void ChangePassword(string password)
        {
            _password = password;
        }

        public void Show()
            => gameObject.SetActive(true);
        public void Hide() 
            => gameObject.SetActive(false);

        public void Dispose()
        {
            UnsubscribeUI();
        }
    }
}
