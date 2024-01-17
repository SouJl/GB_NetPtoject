using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    public class CreateAccountDataUI : BaseAccountDataUI
    {
        [SerializeField]
        private InputField _userEmailField;

        private string _userEmail;

        protected override void SubscribeUI()
        {
            base.SubscribeUI();
            _userEmailField.onValueChanged.AddListener(ChangeUserEmail);
        }

        private void ChangeUserEmail(string userEmail)
        {
            _userEmail = userEmail;
        }

        protected override void AccountProceedAction()
        {
            
        }
    }
}
