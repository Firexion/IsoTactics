using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Inventory", menuName = "Inventory/Inventory")]
    public class ItemInventorySo : ScriptableObject
    {
         private readonly Dictionary<ItemSO, int> _defaultItems = new Dictionary<ItemSO, int>();

         [field: Tooltip("The collection of items and their quantities.")]
         private Dictionary<ItemSO, int> Items { get; set; } = new Dictionary<ItemSO, int>();

         public void Add(ItemSO item, int count = 1)
        {
            if (count <= 0)
                return;

            if (Items.TryGetValue(item, out var itemCount))
            {
                itemCount += count;
                Items.Add(item, itemCount);
            }
            else
            {
                Items.Add(item, count);
            }
        }

        public void Remove(ItemSO item, int count = 1)
        {
            if (count <= 0)
                return;

            if (!Items.TryGetValue(item, out var itemCount)) return;
            itemCount -= count;
            if (itemCount <= 0)
            {
                Items.Remove(item);
            }
            else
            {
                Items.Add(item, itemCount);
            }
        }

        public bool Contains(ItemSO item)
        {
            return Items.TryGetValue(item, out var count) && count > 0;
        }

        public int Count(ItemSO item)
        {
            return Items.TryGetValue(item, out var count) ? count : 0;
        }
        
        public void Init()
        {
            Items ??= new Dictionary<ItemSO, int>();
            Items.Clear();
            foreach (var item in _defaultItems)
            {
                Items.Add(item.Key, item.Value);
            }
        }
    }
}