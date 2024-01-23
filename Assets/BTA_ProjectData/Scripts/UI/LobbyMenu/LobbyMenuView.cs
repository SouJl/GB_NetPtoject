using UnityEngine;
using TMPro;

namespace UI
{
    public class LobbyMenuView : MonoBehaviour
    {

        [SerializeField]
        private GameObject _userDataContainer;
        [SerializeField]
        private Transform _loadUserInfoProgressPlacement;
        [SerializeField]
        private Transform _loadCatalogItemsProgressPlacement;
        [SerializeField]
        private TMP_Text _userName;
        [SerializeField]
        private TMP_Text _userCreatedTime;
        [SerializeField]
        private ItemsContainerUI _itemsContainer;

        public Transform LoadUserInfoPlacement => _loadUserInfoProgressPlacement;

        public Transform LoadCatalogItemsProgressPlacement => _loadCatalogItemsProgressPlacement;

        public ItemsContainerUI ItemsContainer => _itemsContainer;

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
