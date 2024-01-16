using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayFabGameInPanel : MonoBehaviour
    {
        [SerializeField]
        private Button _signInButton;
        [SerializeField]
        private Button _createAccountButton;

        [SerializeField]
        private Canvas _startCanvas;
        [SerializeField]
        private Canvas _sighInCanvas;
        [SerializeField]
        private Canvas _createAccountCanvas;


        private void Start()
        {
            _signInButton.onClick.AddListener(OpenSignInWindow);
            _createAccountButton.onClick.AddListener(OpenCreateAccountWindow);
        }

        private void OpenSignInWindow()
        {
            _startCanvas.gameObject.SetActive(false);
            _sighInCanvas.gameObject.SetActive(true);
        }

        private void OpenCreateAccountWindow()
        {
            _startCanvas.gameObject.SetActive(false);
            _createAccountCanvas.gameObject.SetActive(true);
        }

    }
}

