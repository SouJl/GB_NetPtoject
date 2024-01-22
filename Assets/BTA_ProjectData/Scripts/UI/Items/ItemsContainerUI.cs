using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ItemsContainerUI : MonoBehaviour
    {
        [SerializeField]
        private List<ItemInfoUI> _items;

        private int _enableItemsCount;

        public void InitUI()
        {
            for(int i =0; i < _items.Count; i++)
            {
                var item = _items[i];
                item.InitUI();
            }

            _enableItemsCount = 0;

            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void AddItem(
            string itemId, 
            string itemName, 
            Sprite itemIcon)
        {
            if (_enableItemsCount >= _items.Count)
                return;

            var item = _items[_enableItemsCount];
            item.SetItemUI(itemId, itemName, itemIcon);
            item.Show();

            _enableItemsCount++;
        }

        public void RemoveItem(string itemId)
        {
            var item = _items.Find(itm => itm.ItemId == itemId);

            item.Hide();
        }
    }
}
