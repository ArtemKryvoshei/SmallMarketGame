using System;
using System.Collections.Generic;
using Content.Features.CurrencySystem;
using UnityEngine;

namespace Core.JSONSaveLoadSystem
{
    [Serializable]
    public class DTOModels
    {
        public SettingsData Settings = new();
        public CurrencyData Balance = new();
        public List<BuildingSlotSave> BuildingSlots = new();
        public List<UpgradeSaveData> Upgrades = new();
        public List<InventoryEntry> Inventory = new();
    }
    
    [Serializable]
    public class SettingsData
    {
        public float MasterVolume = 1.0f;
        public float MusicVolume = 0.5f;
        public float SfxVolume = 0.5f;
        public int AutoSaveIntervalSeconds = 120;
    }
    
    [Serializable]
    public class CurrencyData
    {
        public List<CurrencyEntry> Balances = new();
    }
    
    [Serializable]
    public class BuildingSlotSave
    {
        public int slotId;
        public int buildingId;  // 0 или -1 = пусто
        public int buildTier;
    }
    
    [Serializable]
    public class UpgradeSaveData
    {
        public int id;
        public int tier;
    }
    
    [System.Serializable]
    public class InventoryEntry
    {
        public int itemId;
        public int count;
    }
}