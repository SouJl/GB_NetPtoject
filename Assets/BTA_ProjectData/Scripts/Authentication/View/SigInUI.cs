using UnityEngine;
using TMPro;
using System;

namespace Authentication.View
{
    public class SigInUI : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private TMP_Text _connectionStateText;

        [SerializeField]
        private Transform _connectionProgress;
        public Transform ConnectionProgress => _connectionProgress;

        public void InitUI()
        {
            _connectionStateText.text = "LOADING...";
        }

        public void UpdateConnectionState(string message)
        {
            _connectionStateText.text = $"{message}...";
        }


        #region IDisposable

        private bool _isDisposed = false;

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _isDisposed = true;
        }

        #endregion
    }
}
