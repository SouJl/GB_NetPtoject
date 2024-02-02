using UnityEngine;
using TMPro;
using System;

namespace GameLobby
{
    public class ReserveSlotObjectUI : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private TMP_InputField _userName;

        private string _currentName;
        public string CurrentName => _currentName;

        public void InitUI()
        {
            _userName.onValueChanged.AddListener(UpdateUserName);
        }

        private void UpdateUserName(string value)
        {
            _currentName = value;
        }

        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _userName.onValueChanged.RemoveListener(UpdateUserName);

            _isDisposed = true;
        }
    }
}
