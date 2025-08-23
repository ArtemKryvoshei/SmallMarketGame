using System.Collections.Generic;
using Content.Features.UpgradeSystem.Scripts;
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
        [SerializeField] private int additionalRewardPercent;
        [Header("Upgrade Settings")]
        [SerializeField] private UpgradeData upgradeData; 
        [SerializeField] private Transform upgradeWindowAnchor;
        [SerializeField] private TierRewardBonus[] additionalRewardPercentByTier;
        
        private int _currentTier = 1;
        private SellableObjectData _sellableData;
        
        public UpgradeData GetUpgradeData() => upgradeData;
        public int GetCurrentTier() => _currentTier;
        public Transform GetMenuAnchor() => upgradeWindowAnchor;

        public void UpgradeToTier(int newTier)
        {
            if (newTier <= _currentTier || newTier > upgradeData.maxTier) return;
            _currentTier = newTier;

            Debug.Log($"[ProductStand] Upgraded to tier {_currentTier}, reward bonus={FindBonusPercent()}%");
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
    }
}