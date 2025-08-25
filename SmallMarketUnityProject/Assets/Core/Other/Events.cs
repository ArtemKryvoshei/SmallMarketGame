using Content.Features.CurrencySystem;
using Content.Features.CursorChangerSystem.Scripts;
using Core.JSONSaveLoadSystem;
using UnityEngine;

namespace Core.Other
{
    public struct ComponentsInitializeEnd { }
    
    public struct OnSettingsChanged
    {
        public SettingsData newSettings;
    }
    
    public struct OnAutosaveCall { }
    
    public struct OnCurrencyChanged
    {
        public CurrencyType Currency;
        public int NewValue;
    }

    public struct OnObjectClicked
    {
        public GameObject clickedObject;
        public CursorType clickType;
    }

    public struct OnInventoryChanged { }
}