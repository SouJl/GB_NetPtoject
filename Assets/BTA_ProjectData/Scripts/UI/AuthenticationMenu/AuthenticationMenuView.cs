using Abstraction;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AuthenticationMenuView : MonoBehaviour, IOnUpdate
    {
        [SerializeField]
        private Button _signInButton;
        [SerializeField]
        private Button _createAccountButton;

        [SerializeField]
        private Canvas _startCanvas;
        [SerializeField]
        private SignInAccountDataUI _sighInUI;
        [SerializeField]
        private CreateAccountDataUI _createAccountUI;
        [SerializeField]
        private LogInProgressSlider _logInProgress;

        public void InitView()
        {
            _signInButton.onClick.AddListener(OpenSignInWindow);
            _createAccountButton.onClick.AddListener(OpenCreateAccountWindow);

            _sighInUI.OnReturn += ReturnFromAccountDataUI;
            _createAccountUI.OnReturn += ReturnFromAccountDataUI;

            _sighInUI.Hide();
            _createAccountUI.Hide();

            _sighInUI.InitUI(_logInProgress);
        }

        private void OpenSignInWindow()
        {
            _startCanvas.gameObject.SetActive(false);

            _sighInUI.Show();
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

        public void ExecuteUpdate(float deltaTime)
        {
            _sighInUI.UpdateUI(deltaTime);
        }
    }
}
