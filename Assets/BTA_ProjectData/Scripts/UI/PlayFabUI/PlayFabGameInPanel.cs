using System;
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
        private BaseAccountDataUI _sighInUI;
        [SerializeField]
        private BaseAccountDataUI _createAccountUI;


        private void Start()
        {
            _signInButton.onClick.AddListener(OpenSignInWindow);
            _createAccountButton.onClick.AddListener(OpenCreateAccountWindow);

            _sighInUI.OnReturn += ReturnFromAccountDataUI;
            _createAccountUI.OnReturn += ReturnFromAccountDataUI;

            _sighInUI.Hide();
            _createAccountUI.Hide();
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

    }
}

