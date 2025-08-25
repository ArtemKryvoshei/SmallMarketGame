using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using AddressablesGenerated;
using Content.Features.UpgradeSystem.Scripts;
using Core.EventBus;
using Core.IInitializeQueue;
using Core.JSONSaveLoadSystem;
using Core.Other;
using Core.PrefabFactory;
using Core.ServiceLocatorSystem;

[System.Serializable]
public class TierRewardBonusSpawner
{
    public int tier;
    public float spawnInterval;  // время между спавнами
    public int maxCustomers;     // лимит покупателей
}

namespace Content.Features.NPCSpawner.Scripts
{
    public class NPCSpawner : InitializeableMonoComponent, IUpgradeable
    {
        [Header("Save Settings")]
        [SerializeField] private int saveId;
        
        [Header("Spawner Settings")]
        [SerializeField] private Transform spawnPoint;
        
        [Header("Upgrade Settings")]
        [SerializeField] private UpgradeData upgradeData;
        [SerializeField] private Transform upgradeWindowAnchor;
        [SerializeField] private TierRewardBonusSpawner[] spawnerSettingsByTier;
        
        private readonly Dictionary<int, CustomerNPC> _customers = new();
        private float _timer;
        private int _idCounter = 0;
        private int _currentTier = 1;
        private float spawnInterval;
        private int maxCustomers;
        
        public int GetCurrentTier() => _currentTier;
        private IPrefabFactory _prefabFactory;
        public UpgradeData GetUpgradeData() => upgradeData;
        public Transform GetMenuAnchor() => upgradeWindowAnchor;
        
        private ISaveLoadSystem _saveLoadSystem;
        private IEventBus _eventBus;
        
        public void UpgradeToTier(int newTier)
        {
            if (newTier <= _currentTier || newTier > upgradeData.maxTier) return;
            _currentTier = newTier;
            
            ApplyTierSettings();

            Debug.Log($"[NPCSpawner] Upgraded to tier {_currentTier}, spawnInterval={spawnInterval}, maxCustomers={maxCustomers}");
            SaveTier();
        }

        private void ApplyTierSettings()
        {
            foreach (var tier in spawnerSettingsByTier)
            {
                if (tier.tier == _currentTier)
                {
                    spawnInterval = tier.spawnInterval;
                    maxCustomers = tier.maxCustomers;
                    return;
                }
            }
        }
        
        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_LASTINIT_COMPONENTS;
            base.SetupPriority();
        }

        public override void Initialize()
        {
            _prefabFactory = ServiceLocator.Get<IPrefabFactory>();
            _saveLoadSystem = ServiceLocator.Get<ISaveLoadSystem>();
            _eventBus = ServiceLocator.Get<IEventBus>();
            ApplyTierSettings();
            base.Initialize();
            initialized = true;
            _eventBus.Subscribe<OnAutosaveCall>(_ => SaveTier());
            LoadTier();
        }
        
        private void Update()
        {
            if (!initialized)
            {
                return;
            }

            _timer += Time.deltaTime;

            if (_timer >= spawnInterval && _customers.Count < maxCustomers)
            {
                _timer = 0f;
                SpawnCustomer();
            }
        }
        
        private async void SpawnCustomer()
        {
            var go = await _prefabFactory.SpawnAsync(Address.Gameplay.NPC_Customer, spawnPoint.position, Quaternion.identity);
            var npc = go.GetComponent<CustomerNPC>();

            if (npc == null)
            {
                Debug.LogError("[NPCSpawner] Spawned object has no CustomerNPC component!");
                Destroy(go);
                return;
            }

            int id = _idCounter++;
            npc.Initialize(this, id);
            _customers[id] = npc;
        }
        
        public void UnregisterCustomer(int id)
        {
            if (_customers.ContainsKey(id))
            {
                Destroy(_customers[id].gameObject);
                _customers.Remove(id);
                Debug.Log($"[NPCSpawner] Customer {id} released");
            }
        }
        
        public void ReleaseAll()
        {
            foreach (var npc in _customers.Values)
            {
                Destroy(npc.gameObject);
            }
            _customers.Clear();
            Debug.Log("[NPCSpawner] All customers released");
        }
        
        private void OnDestroy()
        {
            _eventBus.Unsubscribe<OnAutosaveCall>(_ => SaveTier());
        }
        
        private void SaveTier()
        {
            var data = _saveLoadSystem.Load();
            var saved = data.Upgrades.Find(u => u.id == saveId);
            if (saved != null)
            {
                saved.tier = _currentTier;
            }
            else
            {
                data.Upgrades.Add(new UpgradeSaveData
                {
                    id = saveId,
                    tier = _currentTier
                });
            }
            _saveLoadSystem.Save(data);
        }

        private void LoadTier()
        {
            var data = _saveLoadSystem.Load();
            var saved = data.Upgrades.Find(u => u.id == saveId);
            if (saved != null)
            {
                _currentTier = saved.tier;
                ApplyTierSettings();
                Debug.Log($"[NPCSpawner] Loaded tier {_currentTier}, spawnInterval={spawnInterval}, maxCustomers={maxCustomers}");
            }
            else
            {
                ApplyTierSettings();
            }
        }

    }
}