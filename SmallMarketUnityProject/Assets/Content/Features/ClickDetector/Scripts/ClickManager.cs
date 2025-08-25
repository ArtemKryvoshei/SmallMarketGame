using Content.Features.CursorChangerSystem.Scripts;
using Core.EventBus;
using Core.IInitializeQueue;
using Core.Other;
using Core.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Content.Features.ClickDetector.Scripts
{
    public class ClickManager : InitializeableMonoComponent
    {
        [SerializeField] private LayerMask clickableLayer; 
        [SerializeField] private UnityEvent OnAnyObjectClicked;
        
        private Camera _mainCamera;
        private IEventBus _eventBus;
        
        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_INGAME_COMPONENTS;
            base.SetupPriority();
        }

        public override void Initialize()
        {
            _mainCamera = Camera.main;
            _eventBus = ServiceLocator.Get<IEventBus>();
            base.Initialize();
            initialized = true;
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && initialized)
            {
                HandleClick();
            }
        }
        
        private void HandleClick()
        {
            // 1) Если кликаем по UI - ничего не делаем
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("[ClickManager] UI click!");
                return;
            }

            // 2) Проверяем клик по объектам в 3D
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, clickableLayer))
            {
                GameObject clickedObject = hit.collider.gameObject;

                var clickable = clickedObject.GetComponent<IClickable>();
                if (clickable != null)
                {
                    clickable.OnClicked(_eventBus);
                }

                OnAnyObjectClicked?.Invoke();
            }
            else
            {
                // Пустой клик (только если это не UI)
                OnObjectClicked eventOnObjectClicked = new OnObjectClicked();
                eventOnObjectClicked.clickedObject = null;
                eventOnObjectClicked.clickType = CursorType.Default;
                _eventBus.Publish(eventOnObjectClicked);
            }
        }
        
    }
}