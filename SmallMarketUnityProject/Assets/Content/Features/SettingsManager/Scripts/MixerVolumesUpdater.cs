using System;
using Core.EventBus;
using Core.IInitializeQueue;
using Core.JSONSaveLoadSystem;
using Core.Other;
using Core.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.Audio;

namespace Content.Features.SettingsManager.Scripts
{
    public class MixerVolumesUpdater : InitializeableMonoComponent
    {
        [SerializeField] private string _mixerMainValueName = "";
        [SerializeField] private string _mixerMusicValueName = "";
        [SerializeField] private string _mixerVfxValueName = "";
        [SerializeField] private AudioMixer mixer;

        private ISettingsManager _settingsManager;
        private IEventBus _eventBus;
        
        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_INGAME_COMPONENTS;
            base.SetupPriority();
        }

        public override void Initialize()
        {
            _eventBus = ServiceLocator.Get<IEventBus>();
            _eventBus.Subscribe<OnSettingsChanged>(UpdateMixerValues);
            _settingsManager = ServiceLocator.Get<ISettingsManager>();
            var settings = _settingsManager.Settings;
            UpdateMixerValues(settings);
            base.Initialize();
        }

        private void UpdateMixerValues(OnSettingsChanged obj)
        {
            mixer.SetFloat(_mixerMainValueName, LogValueToVolume(obj.newSettings.MasterVolume));
            mixer.SetFloat(_mixerMusicValueName, LogValueToVolume(obj.newSettings.MusicVolume));
            mixer.SetFloat(_mixerVfxValueName, LogValueToVolume(obj.newSettings.SfxVolume));
        }

        private void UpdateMixerValues(SettingsData settingsData)
        {
            mixer.SetFloat(_mixerMainValueName, LogValueToVolume(settingsData.MasterVolume));
            mixer.SetFloat(_mixerMusicValueName, LogValueToVolume(settingsData.MusicVolume));
            mixer.SetFloat(_mixerVfxValueName, LogValueToVolume(settingsData.SfxVolume));
        }

        private float LogValueToVolume(float baseValue)
        {
            return (Mathf.Log10(baseValue) * 20);
        }

        private void OnDestroy()
        {
            _eventBus.Unsubscribe<OnSettingsChanged>(UpdateMixerValues);
        }
    }
}