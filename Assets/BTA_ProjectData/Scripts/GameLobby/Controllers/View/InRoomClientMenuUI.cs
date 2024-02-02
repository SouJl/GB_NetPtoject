using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameLobby
{
    public class InRoomClientMenuUI : InRoomBaseMenuUI
    {    
        [SerializeField]
        private Button _readyButton;
        [SerializeField]
        private Button _exitButton;

        public override event Action OnStartGamePressed;
        public override event Action OnExitPressed;

        protected override void SubscribeUI()
        {
            _readyButton.onClick.AddListener(ExecuteStartGameLogic);
            _exitButton.onClick.AddListener(() => OnExitPressed?.Invoke());
        }

        protected override void UnsubscribeUI()
        {
            _readyButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
        }
        private void ExecuteStartGameLogic()
        {
            OnStartGamePressed?.Invoke();
        }
        protected override void OnDestroyUI()
        {
            base.OnDestroyUI();
        }
    }
}
