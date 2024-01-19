using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NoDataIntroUI : MonoBehaviour
    {
        [SerializeField]
        private Button _signInButton;
        [SerializeField]
        private Button _createAccountButton;

        public Button SignInButton => _signInButton;
        public Button CreateAccountButton => _createAccountButton;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
