using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class ItemInfoUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _itemName;
        [SerializeField]
        private Image _itemIcon;

        private string _itemId;

        public string ItemId => _itemId;

        public void InitUI()
        {
            _itemName.text = "";

            Hide();
        }

        public void SetItemUI(string itemId, string name, Sprite iconSprite)
        {
            _itemId = itemId;
            _itemName.text = name;
            _itemIcon.sprite = iconSprite;
        }

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
