using System.Collections.Generic;
using System.Linq;
using AddressablesGenerated;
using Core.EventBus;
using Core.IInitializeQueue;
using Core.JSONSaveLoadSystem;
using Core.Other;
using Core.PrefabFactory;
using Core.ServiceLocatorSystem;
using UnityEngine;

namespace Content.Features.BuildingSystem.Scripts
{
    public class BuildingManager : InitializeableMonoComponent
    {
        [SerializeField] private List<BuildingData> allBuildings;
        [SerializeField] private List<BuildingSlot> slots;

        private ISaveLoadSystem _saveLoad;
        private IPrefabFactory _prefabFactory;
        private IEventBus _eventBus;

        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_INGAME_COMPONENTS - 15;
        }

        public override void Initialize()
        {
            _saveLoad = ServiceLocator.Get<ISaveLoadSystem>();
            _prefabFactory = ServiceLocator.Get<IPrefabFactory>();
            _eventBus = ServiceLocator.Get<IEventBus>();

            Load();
            _eventBus.Subscribe<OnAutosaveCall>(_ => Save());

            base.Initialize();
        }

        public async void Build(BuildingSlot slot, int buildingId)
        {
            slot.DestroySpawned();

            if (buildingId < 0)
            {
                var platform = await _prefabFactory.SpawnAsync(Address.Gameplay.BuildPlatform, slot.SpawnPoint.position, Quaternion.identity, slot.SpawnPoint.parent);
                slot.SetSpawned(platform);
                slot.Clear();
                platform.GetComponent<IBuildPlatform>().SetBuildingSlot(slot);
                return;
            }

            var data = allBuildings.FirstOrDefault(b => b.id == buildingId);
            if (data == null)
            {
                Debug.LogError($"[BuildingManager] Unknown buildingId {buildingId}");
                return;
            }

            var building = await _prefabFactory.SpawnAsync(data.prefabAddress, slot.SpawnPoint.position, Quaternion.identity, slot.SpawnPoint.parent);
            
            ProductStand productStand = building.gameObject.GetComponent<ProductStand>();
            productStand?.SetSellableData(data.sellableObject);
            
            slot.SetSpawned(building);
            slot.AssignBuilding(data.id);

            
            
            Save();
        }

        private async void Load()
        {
            var dto = _saveLoad.Load();
            if (dto?.BuildingSlots == null || dto.BuildingSlots.Count == 0)
            {
                foreach (var slot in slots)
                {
                    var platform = await _prefabFactory.SpawnAsync(Address.Gameplay.BuildPlatform, slot.SpawnPoint.position, Quaternion.identity, slot.SpawnPoint.parent);
                    slot.SetSpawned(platform);
                    slot.Clear();
                    platform.GetComponent<IBuildPlatform>().SetBuildingSlot(slot);
                }
                return;
            }

            foreach (var slot in slots)
            {
                var save = dto.BuildingSlots.FirstOrDefault(s => s.slotId == slot.Id);
                if (save == null || save.buildingId < 0)
                {
                    var platform = await _prefabFactory.SpawnAsync(Address.Gameplay.BuildPlatform, slot.SpawnPoint.position, Quaternion.identity, slot.SpawnPoint.parent);
                    slot.SetSpawned(platform);
                    slot.Clear();
                    platform.GetComponent<IBuildPlatform>().SetBuildingSlot(slot);
                }
                else
                {
                    var data = allBuildings.FirstOrDefault(b => b.id == save.buildingId);
                    if (data != null)
                    {
                        var building = await _prefabFactory.SpawnAsync(data.prefabAddress, slot.SpawnPoint.position, Quaternion.identity, slot.SpawnPoint.parent);
                        slot.SetSpawned(building);
                        slot.AssignBuilding(data.id);
                        ProductStand productStand = building.gameObject.GetComponent<ProductStand>();
                        productStand?.SetSellableData(data.sellableObject);
                    }
                    else
                    {
                        Debug.LogError($"[BuildingManager] BuildingData with id {save.buildingId} not found, spawn platform");
                        var platform = await _prefabFactory.SpawnAsync(Address.Gameplay.BuildPlatform, slot.SpawnPoint.position, Quaternion.identity, slot.SpawnPoint.parent);
                        slot.SetSpawned(platform);
                        slot.Clear();
                        platform.GetComponent<IBuildPlatform>().SetBuildingSlot(slot);
                    }
                }
            }
        }

        private void Save()
        {
            var dto = _saveLoad.Load() ?? new DTOModels();
            dto.BuildingSlots = new List<BuildingSlotSave>();

            foreach (var slot in slots)
            {
                dto.BuildingSlots.Add(new BuildingSlotSave
                {
                    slotId = slot.Id,
                    buildingId = slot.IsEmpty ? -1 : slot.BuildingId,
                    buildTier = slot.BuildTier
                });
            }

            _saveLoad.Save(dto);
            Debug.Log("[BuildingManager] Building slots saved");
        }

        private void OnDestroy()
        {
            _eventBus.Unsubscribe<OnAutosaveCall>(_ => Save());
        }
    }

}