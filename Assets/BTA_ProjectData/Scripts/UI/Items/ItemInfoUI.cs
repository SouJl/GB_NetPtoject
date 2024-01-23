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

        public GameObject SelfGameObject => gameObject;


        public void InitUI(string name, Sprite icon)
        {
            _itemName.text = name;
            _itemIcon.sprite = icon;
        }

        public void DeinintUi()
        {
            _itemName.text = null;
            _itemIcon.sprite = null;
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
