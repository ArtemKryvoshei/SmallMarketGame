using System;
using AddressablesGenerated;
using Content.Features.AutosaveTimer;
using Content.Features.BuildingSystem.Scripts;
using Content.Features.ClickDetector.Scripts;
using Content.Features.UpgradeSystem.Scripts;
using Core.IInitializeQueue;
using Core.Other;
using Core.PrefabFactory;
using Core.ServiceLocatorSystem;
using UnityEngine;

namespace Content.Features.Bootstrap.Scripts
{
    public class GameplaySpecificBootstrap : InitializeableMonoComponent
    {
        [SerializeField] private BuildingManager buildingManager;
        [SerializeField] private ComponentsInitializeManager componentsInitManager;
        private IPrefabFactory factory;

        private async void Awake()
        {
            factory = ServiceLocator.Get<IPrefabFactory>();
            
            var clickManagerPrefab = await factory.SpawnAsync(Address.Managers.ClickManager, Vector3.zero, Quaternion.identity);
            ClickManager clickManager = clickManagerPrefab.GetComponent<ClickManager>();
            ServiceLocator.Register<ClickManager>(clickManager);
            
            var buildManagerPrefab = await factory.SpawnAsync(Address.Managers.BuildManager, Vector3.zero, Quaternion.identity);
            BuildManager buildManager = buildManagerPrefab.GetComponent<BuildManager>();
            ServiceLocator.Register<BuildManager>(buildManager);
            
            var upgradeManagerPrefab = await factory.SpawnAsync(Address.Managers.UpgradeManager, Vector3.zero, Quaternion.identity);
            UpgradeManager upgradeManager = upgradeManagerPrefab.GetComponent<UpgradeManager>();
            ServiceLocator.Register<UpgradeManager>(upgradeManager);
            
            ServiceLocator.Register<BuildingManager>(buildingManager);
            
            base.Initialize();
            clickManager.Initialize();
            buildManager.Initialize();
            componentsInitManager.InitializeComponents();
        }
    }
}