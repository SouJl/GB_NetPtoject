using System;
using UnityEngine;

namespace UI
{
    public class AuthenticationMenuView : MonoBehaviour
    {
        [SerializeField]
        private DefaultInroUI _defaultInroUI;
        [SerializeField]
        private NoDataIntroUI _noDataIntro;
        [SerializeField]
        private BaseAccountDataUI _signInUI;
        [SerializeField]
        private BaseAccountDataUI _createAccountUI;
        [SerializeField]
        private Transform _connetcionProgressPlacement;

        public Transform ConnetcionProgressPlacement => _connetcionProgressPlacement;

        public BaseAccountDataUI SignInUI => _signInUI;
        public BaseAccountDataUI CreateAccountUI => _createAccountUI;

        public event Action OnEnterTheLobby;
        public event Action OnLogOut;

        public void InitView(bool userExistState, string userName)
        {
            SetUpUI(userExistState, userName);
            SubscribeUI();
        }

        private void SubscribeUI() 
        {
            _defaultInroUI.EnterButton.onClick.AddListener(EnterTheLobby);
            _defaultInroUI.LogOutButton.onClick.AddListener(LogOut);

            _noDataIntro.SignInButton.onClick.AddListener(OpenSignInWindow);
            _noDataIntro.CreateAccountButton.onClick.AddListener(OpenCreateAccountWindow);

            _signInUI.OnReturn += ReturnFromAccountDataUI;
            _createAccountUI.OnReturn += ReturnFromAccountDataUI;
        }

        private void UnsubscribeUI()
        {
            _defaultInroUI.EnterButton.onClick.RemoveListener(EnterTheLobby);
            _defaultInroUI.LogOutButton.onClick.RemoveListener(LogOut);

            _noDataIntro.SignInButton.onClick.RemoveListener(OpenSignInWindow);
            _noDataIntro.CreateAccountButton.onClick.RemoveListener(OpenCreateAccountWindow);

            _signInUI.OnReturn -= ReturnFromAccountDataUI;
            _createAccountUI.OnReturn -= ReturnFromAccountDataUI;
        }

        private void EnterTheLobby()
        {
            OnEnterTheLobby?.Invoke();
        }
        private void LogOut()
        {
            OnLogOut?.Invoke();
        }

        private void OpenSignInWindow()
        {
            _noDataIntro.Hide();
            _signInUI.Show();
        }

        private void OpenCreateAccountWindow()
        {
            _noDataIntro.Hide();
            _createAccountUI.Show();
        }

        private void ReturnFromAccountDataUI(BaseAccountDataUI dataUI)
        {
            dataUI.Hide();
            _noDataIntro.Show();
        }

        private void SetUpUI(bool userExistState, string userName)
        {
            _defaultInroUI.Hide();
            _noDataIntro.Hide();
            _signInUI.Hide();
            _createAccountUI.Hide();

            if (userExistState)
            {
                _defaultInroUI.Show();
                
                _defaultInroUI.SetUserName(userName);

                return;
            }

            _noDataIntro.Show();
        }


        private void OnDestroy()
        {
            _signInUI?.Dispose();
            _createAccountUI?.Dispose();

            UnsubscribeUI();
        }
    }
}
