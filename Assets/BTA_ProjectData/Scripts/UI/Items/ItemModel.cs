using Enumerators;
using UnityEngine;

namespace UI
{
    public class ItemModel
    {
        private string _id;
        private string _name;
        private ItemType _type;
        private Sprite _icon;

        public string Id
        {
            get => _id;
            set => _id = value;
        }
        
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public ItemType Type 
        { 
            get => _type; 
            set => _type = value; 
        }
        
        public Sprite Icon 
        {
            get => _icon; 
            set => _icon = value; 
        }
    }
}
