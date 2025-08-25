using System.Collections.Generic;
using Core.EventBus;
using Core.IInitializeQueue;
using Core.JSONSaveLoadSystem;
using Core.Other;
using Core.ServiceLocatorSystem;
using UnityEngine;

namespace Content.Features.CraftSystem.Scripts
{
    public class InventorySystem : InitializeableMonoComponent
    {
        private Dictionary<int, int> _items = new();
        private ISaveLoadSystem _saveLoadSystem;
        private IEventBus _eventBus;

        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_INGAME_COMPONENTS;
            base.SetupPriority();
        }

        public override void Initialize()
        {
            _saveLoadSystem = ServiceLocator.Get<ISaveLoadSystem>();
            _eventBus = ServiceLocator.Get<IEventBus>();
            LoadInventory();
            base.Initialize();
        }
        
        public void AddItem(int itemId, int count = 1)
        {
            if (_items.ContainsKey(itemId))
                _items[itemId] += count;
            else
                _items[itemId] = count;

            Debug.Log($"[InventorySystem] Added {count}x {itemId}, total={_items[itemId]}");
            SaveInventory();
        }
        
        public bool HasItem(int itemId, int count = 1)
        {
            return _items.ContainsKey(itemId) && _items[itemId] >= count;
        }

        public void RemoveItem(int itemId, int count = 1)
        {
            if (!_items.ContainsKey(itemId)) return;

            _items[itemId] -= count;
            if (_items[itemId] <= 0)
                _items.Remove(itemId);

            SaveInventory();
        }
        
        private void SaveInventory()
        {
            var data = _saveLoadSystem.Load();
            data.Inventory = new List<InventoryEntry>();

            foreach (var kvp in _items)
            {
                data.Inventory.Add(new InventoryEntry { itemId = kvp.Key, count = kvp.Value });
            }

            _saveLoadSystem.Save(data);
            _eventBus.Publish(new OnInventoryChanged());
        }
        
        private void LoadInventory()
        {
            var data = _saveLoadSystem.Load();
            _items.Clear();

            if (data.Inventory != null)
            {
                foreach (var entry in data.Inventory)
                    _items[entry.itemId] = entry.count;
            }
        }
        
        public Dictionary<int, int> GetAllItems()
        {
            return new Dictionary<int, int>(_items);
        }
    }
}