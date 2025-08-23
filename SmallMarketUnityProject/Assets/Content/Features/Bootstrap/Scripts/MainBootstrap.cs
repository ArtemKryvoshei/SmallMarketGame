using System;
using AddressablesGenerated;
using Content.Features.AutosaveTimer;
using Content.Features.CurrencySystem;
using Content.Features.CursorChangerSystem.Scripts;
using Content.Features.SettingsManager.Scripts;
using Core.AudioManager;
using UnityEngine;
using Core.EventBus;
using Core.IInitializeQueue;
using Core.JSONSaveLoadSystem;
using Core.LoadingScreenService;
using Core.PrefabFactory;
using Core.SceneManagement;
using Core.SceneTransitionMediator;
using Core.ServiceLocatorSystem;

namespace Content.Features.Bootstrap.Scripts
{
    public class MainBootstrap : MonoBehaviour
    {
        [SerializeField] private string _mainSceneName = "MainScene";
        [SerializeField] private ComponentsInitializeManager componentsInitManager;
        [SerializeField] private bool dontDestroy;
        
        private async void Awake()
        {
            ServiceLocator.Register<IEventBus>(new EventBus());
            
            var saveSystem = new SaveLoadSystem();
            var settingsManager = new SettingsManager.Scripts.SettingsManager(saveSystem);
            var prefabFactory = new PrefabFactory();
            
            var loadingScreen = new LoadingScreenService(prefabFactory);
            await loadingScreen.InitializeAsync(Address.UI.LoadingScreen);
            ServiceLocator.Register<ILoadingScreenService>(loadingScreen);
            
            var sceneLoader = new SceneLoader();
            ServiceLocator.Register<ISceneLoader>(sceneLoader);
            var mediator = new SceneTransitionMediator(loadingScreen, sceneLoader);
            ServiceLocator.Register<ISceneTransitionMediator>(mediator);
            
            var audioManagerPrefab = await prefabFactory.SpawnAsync(Address.Managers.AudioManager, Vector3.zero, Quaternion.identity);
            ServiceLocator.Register<IAudioManager>(audioManagerPrefab.GetComponent<AudioManager>());

            var autosaveManager = await prefabFactory.SpawnAsync(Address.Managers.AutosaveManager, Vector3.zero, Quaternion.identity);
            ServiceLocator.Register<AutosaveManager>(autosaveManager.GetComponent<AutosaveManager>());
            
            var currencyManager = await prefabFactory.SpawnAsync(Address.Managers.CurrencyService, Vector3.zero, Quaternion.identity);
            ServiceLocator.Register<ICurrencyService>(currencyManager.GetComponent<CurrencyService>());
            
            var cursorManager = await prefabFactory.SpawnAsync(Address.Managers.CursorManager, Vector3.zero, Quaternion.identity);
            ServiceLocator.Register<CursorManager>(cursorManager.GetComponent<CursorManager>());
            
            ServiceLocator.Register<ISaveLoadSystem>(saveSystem);
            ServiceLocator.Register<IPrefabFactory>(new PrefabFactory());
            ServiceLocator.Register<ISettingsManager>(settingsManager);
            ServiceLocator.Register<IPrefabFactory>(prefabFactory);
            
            //инициализируем компоненты по очереди
            componentsInitManager.InitializeComponents();

            if (dontDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }

            mediator.LoadSceneWithTransitionAsync(_mainSceneName);
        }
    }
}