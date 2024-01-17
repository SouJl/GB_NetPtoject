using System;
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
        [SerializeField]
        private Button _proceedButton;
        [SerializeField]
        private Button _returnButton;

        protected string _username;
        protected string _password;

        public event Action<BaseAccountDataUI> OnReturn;

        private void Start()
        {
            SubscribeUI();
        }

        protected virtual void SubscribeUI()
        {
            _proceedButton.onClick.AddListener(AccountProceedAction);
            _returnButton.onClick.AddListener(Return);

            _usernameField.onValueChanged.AddListener(ChangeUsername);
            _passwordField.onValueChanged.AddListener(ChangePassword);   
        }

        protected abstract void AccountProceedAction();
        
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
    }
}
