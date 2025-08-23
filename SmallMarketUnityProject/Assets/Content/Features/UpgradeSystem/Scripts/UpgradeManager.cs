using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Content.Features.UpgradeSystem;
using Content.Features.BuildingSystem.Scripts;
using Content.Features.CurrencySystem;
using Core.EventBus;
using Core.IInitializeQueue;
using Core.Other;
using Core.PrefabFactory;
using Core.ServiceLocatorSystem;


namespace Content.Features.UpgradeSystem.Scripts
{
    public class UpgradeManager : InitializeableMonoComponent
    {
        [Header("Configs")]
        [SerializeField] private CurrencyConfig currencyConfig;

        [Header("UI References")]
        [SerializeField] private GameObject upgradeMenuUI;
        [SerializeField] private TMP_Text upgradeNameText;
        [SerializeField] private Transform costHolder;
        [SerializeField] private Transform descriptionHolder;
        [SerializeField] private Button upgradeButton;

        private IUpgradeable _currentUpgradeable; // интерфейс для апгрейда
        private UpgradeData _currentData;
        private int _currentTier;

        private IEventBus _eventBus;
        private ICurrencyService _currency;
        private IPrefabFactory _prefabFactory;

        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_INGAME_COMPONENTS + 6;
            base.SetupPriority();
        }

        public override void Initialize()
        {
            _eventBus = ServiceLocator.Get<IEventBus>();
            _currency = ServiceLocator.Get<ICurrencyService>();
            _prefabFactory = ServiceLocator.Get<IPrefabFactory>();

            _eventBus.Subscribe<OnObjectClicked>(HandleObjectClicked);
            upgradeButton.onClick.AddListener(UpgradeCurrent);

            HideMenu();

            base.Initialize();
        }

        private void OnDestroy()
        {
            _eventBus.Unsubscribe<OnObjectClicked>(HandleObjectClicked);
        }

        private void HandleObjectClicked(OnObjectClicked evt)
        {
            if (evt.clickedObject == null)
            {
                HideMenu();
                return;
            }

            var upgradeable = evt.clickedObject.GetComponent<IUpgradeable>();
            if (upgradeable != null)
            {
                ShowMenu(upgradeable);
            }
            else
            {
                HideMenu();
            }
        }

        private void ShowMenu(IUpgradeable upgradeable)
        {
            _currentUpgradeable = upgradeable;
            _currentData = upgradeable.GetUpgradeData();
            _currentTier = upgradeable.GetCurrentTier();

            if (_currentData == null)
            {
                HideMenu();
                return;
            }

            upgradeMenuUI.transform.SetParent(upgradeable.GetMenuAnchor(), false);
            upgradeMenuUI.SetActive(true);

            UpdateUI();
        }

        private void HideMenu()
        {
            upgradeMenuUI.SetActive(false);
            _currentUpgradeable = null;
            _currentData = null;
        }

        private async void UpdateUI()
        {
            if (_currentData == null) return;

            upgradeNameText.text = $"Tier {_currentTier}";

            ClearHolder(costHolder);
            ClearHolder(descriptionHolder);

            // Если достигнут maxTier
            if (_currentTier >= _currentData.maxTier)
            {
                upgradeButton.interactable = false;
                return;
            }

            var nextTierData = _currentData.tiers[_currentTier]; // индекс совпадает: tier=1 => index=0
            upgradeButton.interactable = true;

            // Косты
            foreach (var cost in nextTierData.cost)
            {
                var go = await _prefabFactory.SpawnAsync(
                    _currentData.costItemPrefabAddress,
                    Vector3.zero,
                    Quaternion.identity,
                    costHolder
                );
                var rect = go.transform as RectTransform;
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
                rect.localPosition = Vector3.zero;

                var ui = go.GetComponent<ResourceEntityUIView>();
                ui.Setup(cost.resourceType, cost.amount, currencyConfig.GetIcon(cost.resourceType));
            }

            // Описания
            foreach (var descAddr in nextTierData.descriptionPrefabAddresses)
            {
                var go = await _prefabFactory.SpawnAsync(
                    descAddr,
                    Vector3.zero,
                    Quaternion.identity,
                    descriptionHolder
                );
                var rect = go.transform as RectTransform;
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
                rect.localPosition = Vector3.zero;
            }
        }

        private void UpgradeCurrent()
        {
            if (_currentUpgradeable == null || _currentData == null) return;

            if (_currentTier >= _currentData.maxTier) return;

            var nextTierData = _currentData.tiers[_currentTier];

            // Проверяем деньги
            if (!_currency.CanAfford(ToDictionary(nextTierData.cost)))
            {
                Debug.LogWarning("[UpgradeManager] Can't afford upgrade");
                return;
            }

            // Списываем ресурсы
            foreach (var cost in nextTierData.cost)
            {
                _currency.Spend(cost.resourceType, cost.amount);
            }

            _currentUpgradeable.UpgradeToTier(_currentTier + 1);

            HideMenu();
        }

        private void ClearHolder(Transform holder)
        {
            foreach (Transform child in holder)
            {
                Destroy(child.gameObject);
            }
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