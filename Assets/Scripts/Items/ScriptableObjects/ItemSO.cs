using UnityEngine;

namespace Items
{
    public enum ItemType
    {
        Consumable,
        Weapon,
        Armor
    }
    
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
    public abstract class ItemSO : ScriptableObject
    {
        [Tooltip("The name of the item")]
        [SerializeField] private string _name = default;
        
        [Tooltip("A preview image for the item")]
        [SerializeField]
        private Sprite _previewImage = default;
        
        [Tooltip("The type of item")]
        public ItemType itemType;
        
        [Tooltip("A prefab reference for the model of the item")]
        public GameObject prefab;
        
        [Tooltip("A description of the item")]
        [TextArea(15,20)] public string description;
    }
}