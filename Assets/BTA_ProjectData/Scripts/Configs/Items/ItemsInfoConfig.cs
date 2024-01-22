using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(ItemsInfoConfig), menuName = "B.T.A/Items" + nameof(ItemsInfoConfig))]
    public class ItemsInfoConfig : ScriptableObject
    {
        [SerializeField] private string _catalogVersion = "bta 1.0.0";   
        [Space(20)]
        [SerializeField] private List<ItemInfo> _consumableItemsInfoCollection;
        [SerializeField] private List<ItemInfo> _ammoItemsInfoCollection;
        [SerializeField] private List<ItemInfo> _shieldItemsInfoCollection;
        [SerializeField] private List<ItemInfo> _weaponItemsInfoCollection;

        public string CatalogVersion => _catalogVersion;
        public List<ItemInfo> ConsumableItemsInfoCollection => _consumableItemsInfoCollection;
        public List<ItemInfo> AmmoItemsInfoCollection => _ammoItemsInfoCollection;
        public List<ItemInfo> ShieldItemsInfoCollection => _shieldItemsInfoCollection;
        public List<ItemInfo> WeaponItemsInfoCollection => _weaponItemsInfoCollection;
       
    }
}
