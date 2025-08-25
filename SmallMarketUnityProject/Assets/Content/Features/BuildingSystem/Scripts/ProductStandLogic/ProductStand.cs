using System;
using System.Collections.Generic;
using Content.Features.UpgradeSystem.Scripts;
using Core.EventBus;
using Core.JSONSaveLoadSystem;
using Core.Other;
using Core.ServiceLocatorSystem;
using UnityEngine;

[System.Serializable]
public class TierRewardBonus
{
    public int tier;
    public float rewardBonus;
}

namespace Content.Features.BuildingSystem.Scripts
{
    public class ProductStand : MonoBehaviour, IUpgradeable
    {
        [Header("Upgrade Settings")]
        [SerializeField] private UpgradeData upgradeData; 
        [SerializeField] private Transform upgradeWindowAnchor;
        [SerializeField] private TierRewardBonus[] additionalRewardPercentByTier;

        private int saveId;
        private int _currentTier = 1;
        private SellableObjectData _sellableData;
        
        public UpgradeData GetUpgradeData() => upgradeData;
        public int GetCurrentTier() => _currentTier;
        public Transform GetMenuAnchor() => upgradeWindowAnchor;

        private ISaveLoadSystem _saveLoadSystem;
        private IEventBus _eventBus;
        
        private void Awake()
        {
            _saveLoadSystem = ServiceLocator.Get<ISaveLoadSystem>();
            _eventBus = ServiceLocator.Get<IEventBus>();
            _eventBus.Subscribe<OnAutosaveCall>(_ => SaveTier());
        }

        private void Start()
        {
            LoadTier();
        }

        public void UpgradeToTier(int newTier)
        {
            if (newTier <= _currentTier || newTier > upgradeData.maxTier) return;
            _currentTier = newTier;

            Debug.Log($"[ProductStand] Upgraded to tier {_currentTier}, reward bonus={FindBonusPercent()}%");
            SaveTier();
        }
        
        public void SetSellableData(SellableObjectData sellableData)
        {
            _sellableData = sellableData;
        }

        public List<ResourceAmount> GetSellableRewards()
        {
            List<ResourceAmount> result = new List<ResourceAmount>();
            float bonusPercent = FindBonusPercent();

            foreach (var rew in _sellableData.rewards)
            {
                int bonus = Mathf.CeilToInt(rew.amount * (bonusPercent / 100f));
                result.Add(new ResourceAmount
                {
                    resourceType = rew.resourceType,
                    amount = rew.amount + bonus
                });
            }

            return result;
        }

        public float FindBonusPercent()
        {
            foreach (var tierReward in additionalRewardPercentByTier)
            {
                if (tierReward.tier == _currentTier)
                {
                    return tierReward.rewardBonus;
                }
            }

            return 0;
        }

        public void SetSaveId(int _saveId)
        {
            saveId = _saveId;
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
                UpgradeToTier(saved.tier); 
                Debug.Log("[ProductStand] ID: " + saveId + ", Tier: " + _currentTier);
            }
        }
    }
}