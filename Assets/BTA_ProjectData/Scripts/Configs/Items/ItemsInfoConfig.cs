using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(ItemsInfoConfig), menuName = "B.T.A/Items" + nameof(ItemsInfoConfig))]
    public class ItemsInfoConfig : ScriptableObject
    {
        [SerializeField] private string _catalogVersion = "bta 1.0.0";   
        [Space(20)]
        [SerializeField] private List<ItemInfo> _availableItemsInfo;


        public string CatalogVersion => _catalogVersion;
        public List<ItemInfo> AvailableItemsInfo => _availableItemsInfo;
    }
}
