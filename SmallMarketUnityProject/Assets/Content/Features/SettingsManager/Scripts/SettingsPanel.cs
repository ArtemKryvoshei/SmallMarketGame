using System;
using Core.EventBus;
using Core.IInitializeQueue;
using Core.JSONSaveLoadSystem;
using Core.Other;
using Core.ServiceLocatorSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Features.SettingsManager.Scripts
{
    public class SettingsPanel : InitializeableMonoComponent
    {
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private TMP_InputField autosaveInput;
        
        private ISettingsManager _settingsManager;
        private IEventBus _eventBus;

        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_PREGAME_COMPONENTS;
            base.SetupPriority();
        }

        public override void Initialize()
        {
            _settingsManager = ServiceLocator.Get<ISettingsManager>();
            _eventBus = ServiceLocator.Get<IEventBus>();
            
            var settings = _settingsManager.Settings;
            masterSlider.value = settings.MasterVolume;
            musicSlider.value = settings.MusicVolume;
            sfxSlider.value = settings.SfxVolume;
            autosaveInput.text = settings.AutoSaveIntervalSeconds.ToString();
            
            _eventBus.Subscribe<OnAutosaveCall>(SaveCurrentSettings);
            
            base.Initialize();
        }

        private void SaveCurrentSettings(OnAutosaveCall obj)
        {
            ApplyAndSave();
        }

        private void OnDestroy()
        {
            _eventBus.Unsubscribe<OnAutosaveCall>(SaveCurrentSettings);
        }

        public void ApplyAndSave()
        {
            var newSettings = GetCurrentSettings();

            OnSettingsChanged settingsChangedEvent = new OnSettingsChanged();
            settingsChangedEvent.newSettings = newSettings;
            _eventBus.Publish(settingsChangedEvent);
            _settingsManager.ApplySettings(newSettings);
            Debug.Log("[SettingsPanel] Settings saved");
        }

        public SettingsData GetCurrentSettings()
        {
            var newSettings = new SettingsData
            {
                MasterVolume = masterSlider.value,
                MusicVolume = musicSlider.value,
                SfxVolume = sfxSlider.value,
                AutoSaveIntervalSeconds = int.TryParse(autosaveInput.text, out var interval) 
                    ? Mathf.Max(1, interval) 
                    : 60
            };

            return newSettings;
        }
    }
}