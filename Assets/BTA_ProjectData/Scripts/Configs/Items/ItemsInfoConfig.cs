using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(ItemsInfoConfig), menuName = "B.T.A/Items" + nameof(ItemsInfoConfig))]
    public class ItemsInfoConfig : ScriptableObject
    {
        [SerializeField] private List<ItemInfo> _consumableItemsInfoCollection;
        [SerializeField] private List<ItemInfo> _ammoItemsInfoCollection;
        [SerializeField] private List<ItemInfo> _shieldItemsInfoCollection;
        [SerializeField] private List<ItemInfo> _weaponItemsInfoCollection;

        public IList<ItemInfo> ConsumableItemsInfoCollection => _consumableItemsInfoCollection;
        public IList<ItemInfo> AmmoItemsInfoCollection => _ammoItemsInfoCollection;
        public IList<ItemInfo> ShieldItemsInfoCollection => _shieldItemsInfoCollection;
        public IList<ItemInfo> WeaponItemsInfoCollection => _weaponItemsInfoCollection;
       
    }
}
