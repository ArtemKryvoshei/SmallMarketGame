using System.Collections.Generic;
using System.Linq;
using AddressablesGenerated;
using Core.EventBus;
using Core.IInitializeQueue;
using Core.Other;
using Core.PrefabFactory;
using Core.ServiceLocatorSystem;
using UnityEngine;

namespace Content.Features.CraftSystem.Scripts
{
    public class InventoryUI : InitializeableMonoComponent
    {
        [SerializeField] private ItemData[] allItems;
        [SerializeField] private Transform holder;                  // куда спавним элементы
        
        private IPrefabFactory _prefabFactory;
        private InventorySystem _inventory;
        private IEventBus _eventBus;
        
        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_LASTINIT_COMPONENTS - 20;
            base.SetupPriority();
        }
        
        public override void Initialize()
        {
            _prefabFactory = ServiceLocator.Get<IPrefabFactory>();
            _inventory = ServiceLocator.Get<InventorySystem>();
            _eventBus = ServiceLocator.Get<IEventBus>();

            _eventBus.Subscribe<OnInventoryChanged>(_ => RefreshUI());

            RefreshUI();
            base.Initialize();
        }
        
        private void OnDestroy()
        {
            _eventBus?.Unsubscribe<OnInventoryChanged>(_ => RefreshUI());
        }
        
        private async void RefreshUI()
        {
            // чистим холдер
            foreach (Transform child in holder)
            {
                Destroy(child.gameObject);
            }

            // получаем текущее состояние инвентаря
            Dictionary<int, int> items = _inventory.GetAllItems();

            foreach (var kvp in items)
            {
                var data = FindData(kvp.Key);
                if (data == null) continue;

                var go = await _prefabFactory.SpawnAsync(Address.UI.UIInventoryItem, Vector3.zero, Quaternion.identity, holder);
                var rect = go.transform as RectTransform;
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
                rect.localPosition = Vector3.zero;

                var ui = go.GetComponent<InventoryItemUIView>();
                ui.Setup(data, kvp.Value);
            }
        }

        private ItemData FindData(int id)
        {
            foreach (var item in allItems)
            {
                if (item.itemId == id)
                {
                    return item;
                }
            }

            return allItems.ToArray()[0];
        }
        
    }
}