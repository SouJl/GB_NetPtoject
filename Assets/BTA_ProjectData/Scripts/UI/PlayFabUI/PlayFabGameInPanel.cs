using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayFabGameInPanel : MonoBehaviour
    {

        [SerializeField]
        private string _titleId;

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

        private void Start()
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

        private void Update()
        {
            _sighInUI.UpdateUI(Time.deltaTime);
        }
    }
}

