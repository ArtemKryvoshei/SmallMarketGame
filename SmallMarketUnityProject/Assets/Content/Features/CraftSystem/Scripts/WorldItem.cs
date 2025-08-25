using Core.EventBus;
using Core.Other;
using Core.ServiceLocatorSystem;
using UnityEngine;

namespace Content.Features.CraftSystem.Scripts
{
    public class WorldItem : MonoBehaviour
    {
        private ItemData _item;
        private IEventBus _eventBus;
        
        public void Setup(ItemData item)
        {
            _item = item;
            _eventBus = ServiceLocator.Get<IEventBus>();
            _eventBus.Subscribe<OnObjectClicked>(HandleClick);
        }
        
        private void OnDestroy()
        {
            if (_eventBus != null)
                _eventBus.Unsubscribe<OnObjectClicked>(HandleClick);
        }

        private void HandleClick(OnObjectClicked evt)
        {
            if (evt.clickedObject == gameObject) // кликнули именно по этому предмету
            {
                PickupItem();
            }
        }

        private void PickupItem()
        {
            var inventory = ServiceLocator.Get<InventorySystem>();
            if (inventory == null)
            {
                Debug.LogError("[WorldItem] No InventorySystem found!");
                return;
            }

            inventory.AddItem(_item.itemId);
            Destroy(gameObject);
        }
    }

}