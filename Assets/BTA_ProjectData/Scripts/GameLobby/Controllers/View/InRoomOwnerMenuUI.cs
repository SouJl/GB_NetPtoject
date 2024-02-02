using UnityEngine;
using UnityEngine.UI;
using System;

namespace GameLobby
{
    public class InRoomOwnerMenuUI : InRoomBaseMenuUI
    {
        [SerializeField]
        private Button _startGameButton;
        [SerializeField]
        private Button _exitButton;

        public override event Action OnStartGamePressed;
        public override event Action OnExitPressed;

        protected override void SubscribeUI()
        {
            _startGameButton.onClick.AddListener(ExecuteStartGameLogic);
            _exitButton.onClick.AddListener(() => OnExitPressed?.Invoke());
        }

        protected override void UnsubscribeUI()
        {
            _startGameButton.onClick.RemoveAllListeners();
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
