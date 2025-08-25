using System.Collections.Generic;
using AddressablesGenerated;
using Content.Features.CurrencySystem;
using Core.EventBus;
using Core.IInitializeQueue;
using Core.Other;
using Core.PrefabFactory;
using Core.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Content.Features.BuildingSystem.Scripts
{
    public class BuildManager : InitializeableMonoComponent
    {
        [Header("Configs")]
        [SerializeField] private List<BuildingData> buildings;
        [SerializeField] private CurrencyConfig currencyConfig;

        [Header("UI References")]
        [SerializeField] private GameObject buildMenuUI;
        [SerializeField] private TMP_Text buildingNameText;
        [SerializeField] private Transform costHolder;   
        [SerializeField] private Transform rewardHolder; 
        [SerializeField] private Button nextButton;
        [SerializeField] private Button prevButton;
        [SerializeField] private Button buildButton;
        
        private int _currentIndex = 0;
        private IBuildPlatform _currentPlatform;
        private GameObject currentObject;

        private IEventBus _eventBus;
        private ICurrencyService _currency;
        private IPrefabFactory _prefabFactory;
        private BuildingManager _buildingManager;

        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_INGAME_COMPONENTS + 5;
            base.SetupPriority();
        }

        public override void Initialize()
        {
            _eventBus = ServiceLocator.Get<IEventBus>();
            _currency = ServiceLocator.Get<ICurrencyService>();
            _prefabFactory = ServiceLocator.Get<IPrefabFactory>();
            _buildingManager = ServiceLocator.Get<BuildingManager>();

            _eventBus.Subscribe<OnObjectClicked>(HandleObjectClicked);

            nextButton.onClick.AddListener(NextBuilding);
            prevButton.onClick.AddListener(PrevBuilding);
            buildButton.onClick.AddListener(BuildCurrent);

            HideMenu();
            
            base.Initialize();
        }
        
        private void OnDisable()
        {
            HideMenu(); 
        }
        
        private void OnDestroy()
        {
            if (_eventBus != null)
                _eventBus.Unsubscribe<OnObjectClicked>(HandleObjectClicked);

            _currentPlatform = null;
        }
        
        private void HandleObjectClicked(OnObjectClicked evt)
        {
            if (this == null || gameObject == null || gameObject.Equals(null))
                return;
            
            if (evt.clickedObject == null)
            {
                HideMenu();
                return;
            }

            var platform = evt.clickedObject.GetComponent<IBuildPlatform>();
            if (platform != null)
            {
                ShowMenu(platform);
            }
            else
            {
                HideMenu();
            }
        }
        
        private void ShowMenu(IBuildPlatform platform)
        {
            _currentPlatform = platform;
            buildMenuUI.transform.SetParent(platform.GetMenuAnchor(), false);
            buildMenuUI.SetActive(true);
            UpdateUI();
        }

        private void HideMenu()
        {
            buildMenuUI.SetActive(false);
            _currentPlatform = null;
        }
        
        private async void UpdateUI()
        {
            if (buildings.Count == 0) return;

            var data = buildings[_currentIndex];
            buildingNameText.text = data.buildingName;

            ClearHolder(costHolder);
            ClearHolder(rewardHolder);
            
            foreach (var cost in data.buildCosts)
            {
                var go = await _prefabFactory.SpawnAsync(Address.UI.CostPresetUI, Vector3.zero, Quaternion.identity, costHolder);
                var rect = go.transform as RectTransform;

                // Сбрасываем локальные трансформации
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
                rect.localPosition = Vector3.zero;

                var ui = go.GetComponent<ResourceEntityUIView>();
                ui.Setup(cost.resourceType, cost.amount, currencyConfig.GetIcon(cost.resourceType));
            }
            
            foreach (var reward in data.sellableObject.rewards)
            {
                var go = await _prefabFactory.SpawnAsync(Address.UI.RewardPresetUI, Vector3.zero, Quaternion.identity, rewardHolder);
                var rect = go.transform as RectTransform;

                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
                rect.localPosition = Vector3.zero;

                var ui = go.GetComponent<ResourceEntityUIView>();
                ui.Setup(reward.resourceType, reward.amount, currencyConfig.GetIcon(reward.resourceType));
            }

        }

        
        private void ClearHolder(Transform holder)
        {
            foreach (Transform child in holder)
            {
                Destroy(child.gameObject);
            }
        }
        
        private void NextBuilding()
        {
            _currentIndex = (_currentIndex + 1) % buildings.Count;
            UpdateUI();
        }

        private void PrevBuilding()
        {
            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = buildings.Count - 1;
            UpdateUI();
        }
        
        private void BuildCurrent()
        {
            if (_currentPlatform == null) return;
            
            var data = buildings[_currentIndex];
            if (!_currency.CanAfford(ToDictionary(data.buildCosts)))
            {
                Debug.LogWarning("[BuildManager] Cant afford building");
                return;
            }

            buildMenuUI.transform.SetParent(null, false);
            
            // списываем ресурсы
            foreach (var cost in data.buildCosts)
            {
                _currency.Spend(cost.resourceType, cost.amount);
            }

            BuildingSlot slotToSpawn = _currentPlatform.GetBuildingSlot();
            if (slotToSpawn != null && _buildingManager != null)
            {
                _buildingManager.Build(slotToSpawn, data.id);
                Debug.Log("[BuildManager] Build!");
            }
            else
            {
                Debug.LogError("[BuildManager] Something wrong with build!");
            }
            
            HideMenu();
            
        }

        
        private Dictionary<CurrencyType, int> ToDictionary(List<ResourceAmount> list)
        {
            var dict = new Dictionary<CurrencyType, int>();
            foreach (var res in list)
            {
                if (dict.ContainsKey(res.resourceType))
                    dict[res.resourceType] += res.amount;
                else
                    dict[res.resourceType] = res.amount;
            }
            return dict;
        }
    }
}