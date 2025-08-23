using System.Collections.Generic;
using Content.Features.BuildingSystem.Scripts;
using UnityEngine;

namespace Content.Features.UpgradeSystem.Scripts
{
    [System.Serializable]
    public class UpgradeTierData
    {
        public List<ResourceAmount> cost;              // стоимость улучшения
        public List<string> descriptionPrefabAddresses; // UI описания
    }
    
    [CreateAssetMenu(fileName = "UpgradeData", menuName = "Game/Upgrade Data")]
    public class UpgradeData : ScriptableObject
    {
        [Header("ID / Name")]
        [Tooltip("Уникальный ID апгрейда (для сохранения)")]
        public string upgradeId;

        [Min(1)]
        [Tooltip("Максимальный тир (1 = базовый)")]
        public int maxTier = 3;

        [Header("UI Prefabs")]
        [Tooltip("Addressables-адрес UI-префаба элемента стоимости (один и тот же для всех пунктов cost)")]
        public string costItemPrefabAddress; // напр. Address.UI.CostPresetUI

        [Header("Tiers")]
        [Tooltip("Данные по каждому tier (индекс 0 = tier 1)")]
        public List<UpgradeTierData> tiers = new();
    }
}