using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class DebugConsoleUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject _messagePrefab;
        [SerializeField] 
        private GameObject _messageContainer;

        private readonly List<DebugMessageUI> _messageCollection = new();

        public void Log(string message)
        {
            var messageUI = CreateMessage();

            var datedMessage = SetDataToMessage(message);

            messageUI.SetMessage(datedMessage, Color.white);

            _messageCollection.Add(messageUI);
        }

        public void LogError(string message)
        {
            var messageUI = CreateMessage();

            var datedMessage = SetDataToMessage(message);

            messageUI.SetMessage(datedMessage, Color.red);

            _messageCollection.Add(messageUI);
        }

        public void LogWarning(string message)
        {
            var messageUI = CreateMessage();

            var datedMessage = SetDataToMessage(message);

            messageUI.SetMessage(datedMessage, Color.yellow);

            _messageCollection.Add(messageUI);
        }

        private DebugMessageUI CreateMessage()
        {
            var messageUI 
                = Instantiate(_messagePrefab, _messageContainer.transform).GetComponent<DebugMessageUI>();

            return messageUI;
        }

        private string SetDataToMessage(string message)
        {
            return $"{DateTime.Now} | {message}";
        }
    }
}
