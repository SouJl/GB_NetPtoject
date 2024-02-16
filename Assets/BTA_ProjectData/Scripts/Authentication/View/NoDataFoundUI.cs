using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Authentication.View
{
    public class NoDataFoundUI : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private TMP_InputField _nameField;
        [SerializeField]
        private TMP_InputField _passwordField;
        [SerializeField]
        private Button _signInButton;
        [SerializeField]
        private Button _registerButton;

        public event Action OnSignIn;
        public event Action OnRegister;

        private string _name;
        public string Name => _name;

        private string _password;
        public string Password => _password;

        public void InitUI()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            _nameField.onValueChanged.AddListener(ChangeName);
            _passwordField.onValueChanged.AddListener(ChangePassword);

            _signInButton.onClick.AddListener(() => OnSignIn?.Invoke());
            _registerButton.onClick.AddListener(() => OnRegister?.Invoke());
        }

        private void Unsubscribe()
        {
            _nameField.onValueChanged.RemoveListener(ChangeName);
            _passwordField.onValueChanged.RemoveListener(ChangePassword);

            _signInButton.onClick.RemoveAllListeners();
            _registerButton.onClick.RemoveAllListeners();
        }

        private void ChangeName(string name)
        {
            _name = name;
        }

        private void ChangePassword(string password)
        {
            _password = password;
        }

        #region IDisposable

        private bool _isDisposed = false;

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
