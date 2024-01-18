using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AuthenticationMenuView : MonoBehaviour
    {
        [SerializeField]
        private Button _signInButton;
        [SerializeField]
        private Button _createAccountButton;

        [SerializeField]
        private Canvas _startCanvas;
        [SerializeField]
        private BaseAccountDataUI _signInUI;
        [SerializeField]
        private BaseAccountDataUI _createAccountUI;
        [SerializeField]
        private Transform _connetcionProgressPlacement;

        public Transform ConnetcionProgressPlacement => _connetcionProgressPlacement;

        public BaseAccountDataUI SignInUI => _signInUI;
        public BaseAccountDataUI CreateAccountUI => _createAccountUI;

        public void InitView()
        {
            SubscribeUI();

            _signInUI.Hide();
            _createAccountUI.Hide();
        }

        private void SubscribeUI() 
        {
            _signInButton.onClick.AddListener(OpenSignInWindow);
            _createAccountButton.onClick.AddListener(OpenCreateAccountWindow);

            _signInUI.OnReturn += ReturnFromAccountDataUI;
            _createAccountUI.OnReturn += ReturnFromAccountDataUI;
        }

        private void UnsubscribeUI()
        {
            _signInButton.onClick.RemoveListener(OpenSignInWindow);
            _createAccountButton.onClick.RemoveListener(OpenCreateAccountWindow);

            _signInUI.OnReturn -= ReturnFromAccountDataUI;
            _createAccountUI.OnReturn -= ReturnFromAccountDataUI;
        }

        private void OpenSignInWindow()
        {
            _startCanvas.gameObject.SetActive(false);

            _signInUI.Show();
        }

        private void OpenCreateAccountWindow()
        {
            _startCanvas.gameObject.SetActive(false);

            _createAccountUI.Show();
        }

        private void ReturnFromAccountDataUI(BaseAccountDataUI dataUI)
        {
            dataUI.Hide();
            _startCanvas.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            _signInUI?.Dispose();
            _createAccountUI?.Dispose();

            UnsubscribeUI();
        }
    }
}
