using System;
using Core.EventBus;
using Core.IInitializeQueue;
using Core.Other;
using Core.ServiceLocatorSystem;
using TMPro;
using UnityEngine;

namespace Content.Features.CurrencySystem
{
    public class CurrencyView : InitializeableMonoComponent
    {
        [SerializeField] private CurrencyType currencyType;   
        [SerializeField] private TMP_Text currencyText;      

        private ICurrencyService _currencyService;
        private IEventBus _eventBus;

        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_INGAME_COMPONENTS + 1;
        }

        public override void Initialize()
        {
            _currencyService = ServiceLocator.Get<ICurrencyService>();
            _eventBus = ServiceLocator.Get<IEventBus>();
            _eventBus.Subscribe<OnCurrencyChanged>(OnCurrencyChangedUpdate);
            UpdateView(_currencyService.GetBalance(currencyType));
            base.Initialize();
        }

        private void OnCurrencyChangedUpdate(OnCurrencyChanged obj)
        {
            if (obj.Currency == currencyType)
            {
                UpdateView(obj.NewValue);
            }
        }
        
        private void UpdateView(int value)
        {
            if (currencyText != null)
            {
                currencyText.text = value.ToString();
            }
        }

        private void OnDestroy()
        {
            _eventBus.Unsubscribe<OnCurrencyChanged>(OnCurrencyChangedUpdate);
        }

        [ContextMenu("TestAddCurrency")]
        public void TestAddCurrency()
        {
            _currencyService.Add(currencyType, 20);
        }
    }
}