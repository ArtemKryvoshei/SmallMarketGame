using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using AddressablesGenerated;
using Content.Features.UpgradeSystem.Scripts;
using Core.IInitializeQueue;
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
        [Header("Spawner Settings")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private int maxCustomers = 5;
        
        [Header("Upgrade Settings")]
        [SerializeField] private UpgradeData upgradeData;
        [SerializeField] private Transform upgradeWindowAnchor;
        [SerializeField] private TierRewardBonusSpawner[] spawnerSettingsByTier;
        
        private IPrefabFactory _prefabFactory;
        private readonly Dictionary<int, CustomerNPC> _customers = new();
        private float _timer;
        private int _idCounter = 0;
        
        private int _currentTier = 1;
        
        public UpgradeData GetUpgradeData() => upgradeData;
        public int GetCurrentTier() => _currentTier;
        public Transform GetMenuAnchor() => upgradeWindowAnchor;
        
        public void UpgradeToTier(int newTier)
        {
            if (newTier <= _currentTier || newTier > upgradeData.maxTier) return;
            _currentTier = newTier;
            
            ApplyTierSettings();

            Debug.Log($"[NPCSpawner] Upgraded to tier {_currentTier}, spawnInterval={spawnInterval}, maxCustomers={maxCustomers}");
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
            ApplyTierSettings();
            base.Initialize();
            initialized = true;
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
    }
}