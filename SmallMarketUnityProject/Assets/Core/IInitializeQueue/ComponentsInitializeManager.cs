using System.Linq;
using Core.EventBus;
using Core.Other;
using Core.ServiceLocatorSystem;
using UnityEngine;

namespace Core.IInitializeQueue
{
    public class ComponentsInitializeManager : MonoBehaviour
    {
        private IEventBus _eventBus;
        
        public void InitializeComponents()
        {
            var componentsToPrepare = FindObjectsOfType<MonoBehaviour>()
                .OfType<IInitializeableComponent>()
                .ToList();

            foreach (var component in componentsToPrepare)
            {
                component.SetupPriority();
            }

            var componentsToInit = componentsToPrepare
                .OrderByDescending(c => c.Priority)
                .ToList();

            string initQueue = "[ComponentsInitializeManager] InitializeQueue: ";
            
            foreach (var component in componentsToInit)
            {
                initQueue += component.GetType().Name + "[P: " + component.Priority + "] -> ";
            }
            
            Debug.LogWarning(initQueue);
            
            foreach (var component in componentsToInit)
            {
                component.Initialize();
            }
            
            _eventBus = ServiceLocator.Get<IEventBus>();
            if (_eventBus != null)
            {
                _eventBus.Publish(new ComponentsInitializeEnd());
                Debug.Log("[ComponentsInitializeManager] Components initialized");
            }
            else
            {
                Debug.LogError("[ComponentsInitializeManager] Event bus is null");
            }
        }
    }
}