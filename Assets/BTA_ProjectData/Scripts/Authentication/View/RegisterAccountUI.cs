using Abstraction;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Authentication.View
{
    public class RegisterAccountUI : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private TMP_InputField _usernameField;
        [SerializeField]
        private TMP_InputField _passwordField;
        [SerializeField]
        private TMP_InputField _emailField;
        [SerializeField]
        private Button _createAccButton;
        [SerializeField]
        private Button _backButton;

        public event Action<UserAccountData> OnCreateAccount;
        public event Action OnBack;

        public void InitUI()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            _createAccButton.onClick.AddListener(CreateAccountPressed);
            _backButton.onClick.AddListener(BackPressed);
        }

        private void Unsubscribe()
        {
            _createAccButton.onClick.RemoveListener(CreateAccountPressed);
            _backButton.onClick.RemoveListener(BackPressed);
        }

        private void CreateAccountPressed()
        {
            var userData = new UserAccountData
            {
                Name = _usernameField.text,
                Password = _passwordField.text,
                Email = _emailField.text
            };

            OnCreateAccount?.Invoke(userData);
        }

        private void BackPressed()
        {
            OnBack?.Invoke();
        }

        #region IDisposable

        private bool _isDisposed  = false;

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _isDisposed = true;

            Unsubscribe();
        }

        #endregion
    }
}
