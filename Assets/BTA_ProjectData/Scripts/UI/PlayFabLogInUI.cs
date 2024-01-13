using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace UI
{
    public class PlayFabLogInUI :MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _userIdInputField;
        [SerializeField]
        private Button _logInButton;

        public Action<string> OnLogIn;


        private void Start()
        {
            _logInButton.onClick.AddListener(ExecuteLogIn);
        }

        private void ExecuteLogIn()
        {
            var userId = _userIdInputField.text;

            OnLogIn?.Invoke(userId);
        }
    }
}
