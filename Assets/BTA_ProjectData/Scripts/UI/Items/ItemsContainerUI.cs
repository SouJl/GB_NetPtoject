using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ItemsContainerUI : MonoBehaviour
    {
        private readonly Dictionary<string, ItemInfoUI> _itemsCollection = new();
        
        [SerializeField]
        private GameObject _itemInfoPrefab;
        [SerializeField]
        private Transform _placeForItems;

        public void Display(IList<ItemModel> items)
        {
            Clear();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                _itemsCollection[item.Id] = CreateItemView(item);
            }
        }

        public void Clear()
        {
            foreach (var buttonView in _itemsCollection.Values)
                DestroyItemView(buttonView);

            _itemsCollection.Clear();
        }

        private void DestroyItemView(ItemInfoUI buttonView)
        {
            buttonView.DeinintUi();
            Destroy(buttonView.SelfGameObject);
        }

        private ItemInfoUI CreateItemView(ItemModel item)
        {
            GameObject objectView = Instantiate(_itemInfoPrefab, _placeForItems, false);
            var buttonView = objectView.GetComponent<ItemInfoUI>();

            buttonView.InitUI(item.Name, item.Icon);

            return buttonView;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy() => Clear();
    }
}
