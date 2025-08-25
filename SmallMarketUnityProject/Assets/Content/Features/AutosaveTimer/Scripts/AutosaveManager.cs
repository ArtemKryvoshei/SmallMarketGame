using Content.Features.SettingsManager.Scripts;
using Core.EventBus;
using Core.IInitializeQueue;
using Core.Other;
using Core.ServiceLocatorSystem;
using UnityEngine;

namespace Content.Features.AutosaveTimer
{
    public class AutosaveManager : InitializeableMonoComponent
    {
        private float autosaveInterval;
        private float _timer;

        private IEventBus _eventBus;
        private ISettingsManager _settingsManager;

        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_PREGAME_COMPONENTS - 5;
        }

        public override void Initialize()
        {
            if (initialized)
            {
                Debug.Log("[AutosaveManager] Already initialized");
                return;
            }

            DontDestroyOnLoad(gameObject);
            _eventBus = ServiceLocator.Get<IEventBus>();
            _eventBus.Subscribe<OnSettingsChanged>(OnSettingsChanged);
            
            _settingsManager = ServiceLocator.Get<ISettingsManager>();
            var settings = _settingsManager.Settings;
            autosaveInterval = Mathf.Max(ConstantsHolder.MINIMUM_AUTOSAVE_INTERVAL, settings.AutoSaveIntervalSeconds);
            
            base.Initialize();
            initialized = true;
        }

        private void OnDestroy()
        {
            _eventBus?.Unsubscribe<OnSettingsChanged>(OnSettingsChanged);
        }

        private void Update()
        {
            if (!initialized)
            {
                return;
            }

            _timer += Time.unscaledDeltaTime;

            if (_timer >= autosaveInterval)
            {
                TriggerAutosave();
                _timer = 0f;
            }
        }

        private void TriggerAutosave()
        {
            var evt = new OnAutosaveCall();
            _eventBus.Publish(evt);
            Debug.LogWarning("[AutosaveManager] Autosave!");
        }

        private void OnSettingsChanged(OnSettingsChanged evt)
        {
            autosaveInterval = Mathf.Max(ConstantsHolder.MINIMUM_AUTOSAVE_INTERVAL, evt.newSettings.AutoSaveIntervalSeconds);
            Debug.Log($"[AutosaveManager] Autosave interval updated: {autosaveInterval}s");
        }

        public void ForceAutosave()
        {
            TriggerAutosave();
            _timer = 0f;
        }
    }
}