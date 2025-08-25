using System;
using Core.EventBus;
using Core.JSONSaveLoadSystem;
using Core.Other;
using Core.ServiceLocatorSystem;
using UnityEngine;

namespace Content.Features.SmallHelpfulScripts
{
    public class AutosaveCaller : MonoBehaviour
    {
        private IEventBus _eventBus;
        
        private void Awake()
        {
            _eventBus = ServiceLocator.Get<IEventBus>();
        }

        public void CallAutosave()
        {
            _eventBus.Publish(new OnAutosaveCall());
        }
    }
}