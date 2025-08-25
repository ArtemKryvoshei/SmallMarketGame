using Core.EventBus;
using Core.Other;
using Core.PrefabFactory;
using Core.ServiceLocatorSystem;
using UnityEngine;

namespace Content.Features.CraftSystem.Scripts
{
    public class NPCDrop : MonoBehaviour
    {
        [SerializeField] private ItemData[] possibleDrops;
        [Range(0, 1f)]
        [SerializeField] private float dropChance = 0.3f;
        [SerializeField] private int attempts = 3;

        private bool droped = false;
        private IPrefabFactory _prefabFactory;
        private IEventBus _eventBus;

        private void Awake()
        {
            _prefabFactory = ServiceLocator.Get<IPrefabFactory>();
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
            if (evt.clickedObject == gameObject)
            {
                TryDropItem();
            }
        }
        
        public async void TryDropItem()
        {
            if (possibleDrops == null || possibleDrops.Length == 0 || droped)
                return;

            for (int i = 0; i < attempts; i++)
            {
                if (Random.value <= dropChance)
                {
                    var item = possibleDrops[Random.Range(0, possibleDrops.Length)];

                    var go = await _prefabFactory.SpawnAsync(
                        item.itemPrefabAddress,
                        transform.position + Vector3.up, 
                        Quaternion.identity
                    );

                    var worldItem = go.GetComponent<WorldItem>();
                    if (worldItem != null)
                    {
                        worldItem.Setup(item);
                    }
                    else
                    {
                        Debug.LogError($"[NPCDrop] Prefab {item.itemPrefabAddress} has no WorldItem component!");
                    }

                    droped = true;
                    attempts = 0;
                }
            }
        }
    }

}