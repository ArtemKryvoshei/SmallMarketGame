using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventBus;
using Core.IInitializeQueue;
using Core.JSONSaveLoadSystem;
using Core.Other;
using Core.ServiceLocatorSystem;
using UnityEngine;


namespace Content.Features.CurrencySystem
{
    public class CurrencyService : InitializeableMonoComponent, ICurrencyService
    {
        private IEventBus _eventBus;
        private ISaveLoadSystem _saveLoad;
        
        private Dictionary<CurrencyType, int> _balances = new();

        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_PREGAME_COMPONENTS;
        }

        public override void Initialize()
        {
            if (initialized)
            {
                Load();
                Debug.Log("[CurrencyService] Already initialized");
                return;
            }

            DontDestroyOnLoad(this.gameObject);
            _eventBus = ServiceLocator.Get<IEventBus>();
            _saveLoad = ServiceLocator.Get<ISaveLoadSystem>();
            _eventBus.Subscribe<OnAutosaveCall>(AutosaveCurrency);
            Load();
            base.Initialize();
            initialized = true;
        }

        private void AutosaveCurrency(OnAutosaveCall obj)
        {
            Save();
        }

        public int GetBalance(CurrencyType currency)
        {
            return _balances.TryGetValue(currency, out var value) ? value : 0;
        }

        public void Add(CurrencyType currency, int amount)
        {
            if (!_balances.ContainsKey(currency))
                _balances[currency] = 0;

            _balances[currency] += amount;
            PublishChange(currency);
        }

        public void Spend(CurrencyType currency, int amount)
        {
            if (!CanAfford(currency, amount))
            {
                Debug.LogError("[CurrencyService] Can't afford this buy");
                return;
            }
            
            _balances[currency] -= amount;
            PublishChange(currency);
        }

        public bool CanAfford(Dictionary<CurrencyType, int> costs)
        {
            foreach (var kvp in costs)
            {
                if (GetBalance(kvp.Key) < kvp.Value)
                    return false;
            }
            return true;
        }

        public bool CanAfford(CurrencyType currency, int amount)
        {
            return _balances.ContainsKey(currency) && _balances[currency] >= amount;
        }
        
        private void PublishChange(CurrencyType currency)
        {
            var evt = new OnCurrencyChanged
            {
                Currency = currency,
                NewValue = _balances[currency]
            };
            _eventBus.Publish(evt);
            Save();
        }
        
        public void Save()
        {
            DTOModels dto = _saveLoad.Load();
            if (dto == null) dto = new DTOModels();

            dto.Balance = new CurrencyData
            {
                Balances = _balances
                    .Select(kvp => new CurrencyEntry { Key = kvp.Key.ToString(), Value = kvp.Value })
                    .ToList()
            };
            
            _saveLoad.Save(dto);
            Debug.Log("[CurrencyService] Balance saved!");
        }

        public void Load()
        {
            var dto = _saveLoad.Load();
            if (dto != null && dto.Balance != null && dto.Balance.Balances != null)
            {
                _balances.Clear();
                foreach (var entry in dto.Balance.Balances)
                {
                    if (Enum.TryParse<CurrencyType>(entry.Key, out var type))
                    {
                        _balances[type] = entry.Value;
                        _eventBus.Publish(new OnCurrencyChanged
                        {
                            Currency = type,
                            NewValue = entry.Value
                        });
                        Debug.Log($"[CurrencyService] Loaded {type} = {entry.Value}");
                    }
                }
            }
            if(dto.Balance.Balances == null || dto.Balance.Balances.Count <= 0)
            {
                Debug.LogWarning("[CurrencyService] No balances found! Create empty");
                _balances = new Dictionary<CurrencyType, int>();
                foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
                {
                    _balances[type] = ConstantsHolder.START_CURRENCIES_AMOUNT;
                    _eventBus.Publish(new OnCurrencyChanged
                    {
                        Currency = type,
                        NewValue = ConstantsHolder.START_CURRENCIES_AMOUNT
                    });
                }
            }

            Debug.Log("[CurrencyService] Balance loaded!");
        }


        private void OnDestroy()
        {
            _eventBus.Unsubscribe<OnAutosaveCall>(AutosaveCurrency);
        }
    }
}