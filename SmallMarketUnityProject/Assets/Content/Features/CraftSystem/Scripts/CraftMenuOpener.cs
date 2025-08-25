using System;
using Content.Features.CursorChangerSystem.Scripts;
using Core.EventBus;
using Core.Other;
using Core.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Content.Features.CraftSystem.Scripts
{
    public class CraftMenuOpener : MonoBehaviour
    {
        public UnityEvent OnCraftMenuOpened;
        public UnityEvent OnCraftMenuClosed;
        
        private bool _isOpened = false;
        private IEventBus _eventBus;

        private void Start()
        {
            _eventBus = ServiceLocator.Get<IEventBus>();
            _eventBus.Subscribe<OnObjectClicked>(HandleClick);
        }

        private void Update()
        {
            if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) && _isOpened)
            {
                _isOpened = false;
                OnCraftMenuClosed?.Invoke();
            }
        }

        private void OnDestroy()
        {
            if (_eventBus != null)
                _eventBus.Unsubscribe<OnObjectClicked>(HandleClick);
        }
        
        private void HandleClick(OnObjectClicked evt)
        {
            if (evt.clickType == CursorType.Craft && !_isOpened)
            {
                _isOpened = true;
                OnCraftMenuOpened?.Invoke();
            }
        }
    }
}