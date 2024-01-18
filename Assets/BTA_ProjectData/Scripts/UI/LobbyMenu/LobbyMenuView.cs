using UnityEngine;
using TMPro;

namespace UI
{
    public class LobbyMenuView : MonoBehaviour
    {

        [SerializeField]
        private GameObject _userDataContainer;
        [SerializeField]
        private Transform _connetcionProgressPlacement;
        [SerializeField]
        private TMP_Text _userName;
        [SerializeField]
        private TMP_Text _userCreatedTime;

        public Transform ConnetcionProgressPlacement => _connetcionProgressPlacement;

        public void InitUI()
        {
            _userName.text = "";
            _userCreatedTime.text = "";

            HideUserData();
        }

        public void ShowUserData()
        {
            _userDataContainer.SetActive(true);
        }

        public void HideUserData()
        {
            _userDataContainer.SetActive(false);
        }

        public void SetUserName(string userName)
        {
            _userName.text = userName;
        }

        public void SetUserCreatedTime(string createdTime)
        {
            _userCreatedTime.text = createdTime;
        }
    }
}
