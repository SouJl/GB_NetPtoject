using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class DefaultInroUI: MonoBehaviour
    {
        [SerializeField]
        private Button _enterButton;
        [SerializeField]
        private Button _logOutButton;
        [SerializeField]
        private TMP_Text _userName;

        public Button EnterButton => _enterButton;
        public Button LogOutButton => _logOutButton;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetUserName(string userName)
        {
            _userName.text = userName;
        }
    }
}
