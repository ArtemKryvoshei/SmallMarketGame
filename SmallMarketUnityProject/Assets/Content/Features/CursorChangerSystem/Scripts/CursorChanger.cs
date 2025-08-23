using Content.Features.ClickDetector.Scripts;
using Core.EventBus;
using Core.Other;
using UnityEngine;

namespace Content.Features.CursorChangerSystem.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class CursorChanger : MonoBehaviour, ICursorChanger, IClickable
    {
        [SerializeField] private CursorType cursorType = CursorType.Build;

        public CursorType GetCursorType()
        {
            return cursorType;
        }

        public void OnClicked()
        {
            Debug.Log("[CursorChanger] OnClicked");
        }

        public void OnClicked(IEventBus eventBus)
        {
            Debug.Log("[CursorChanger] OnClicked call on " + gameObject.name);
            OnObjectClicked eventOnObjectClicked = new OnObjectClicked();
            eventOnObjectClicked.clickedObject = gameObject;
            eventOnObjectClicked.clickType = cursorType;
            eventBus.Publish(eventOnObjectClicked);
        }
    }
}