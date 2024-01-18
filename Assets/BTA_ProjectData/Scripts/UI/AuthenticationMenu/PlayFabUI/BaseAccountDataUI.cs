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

        protected virtual void AccountProceedAction()
        {
            if (_username == null || _username == "")
            {
                Debug.Log("Username is null or empty");
                return;
            }
            if (_password == null || _password == "")
            {
                Debug.Log("Password is null or empty");
                return;
            }
        }
        
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

        private void OnDestroy()
        {
            OnUIDestoy();
        }

        protected virtual void OnUIDestoy() 
        {
            _proceedButton.onClick.RemoveListener(AccountProceedAction);
            _returnButton.onClick.RemoveListener(Return);

            _usernameField.onValueChanged.RemoveListener(ChangeUsername);
            _passwordField.onValueChanged.RemoveListener(ChangePassword);
        }
    }
}
