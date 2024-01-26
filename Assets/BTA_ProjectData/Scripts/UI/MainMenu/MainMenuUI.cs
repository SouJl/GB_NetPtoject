﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField]
        private Button _joinGameButton;
        [SerializeField]
        private Button _createGameButton;
        [SerializeField]
        private Button _exitButton;

        public event Action OnJoinGamePressed;
        public event Action OnCreateGamePressed;
        public event Action OnExitGamePressed;

        public void InitUI()
        {
            SubscibeUI();
        }

        private void SubscibeUI()
        {
            _joinGameButton.onClick.AddListener(() => OnJoinGamePressed?.Invoke());
            _createGameButton.onClick.AddListener(() => OnCreateGamePressed?.Invoke());
            _exitButton.onClick.AddListener(() => OnExitGamePressed?.Invoke());
        }
        private void UnsubscibeUI()
        {
            _joinGameButton.onClick.RemoveAllListeners();
            _createGameButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            UnsubscibeUI();
        }
    }
}
