using System;
using System.Collections.Generic;
using Content.Features.CurrencySystem;
using UnityEngine;

namespace Content.Features.CurrencySystem
{
    
    [Serializable]
    public class ResourceIcon
    {
        public CurrencyType type;
        public Sprite icon;
    }
    
    [CreateAssetMenu(fileName = "ResourceConfig", menuName = "Game/Resource Config")]
    public class CurrencyConfig : ScriptableObject
    {
        [SerializeField] private List<ResourceIcon> resources = new();
        
        private Dictionary<CurrencyType, Sprite> _lookup;

        public void Initialize()
        {
            _lookup = new Dictionary<CurrencyType, Sprite>();
            foreach (var res in resources)
            {
                if (!_lookup.ContainsKey(res.type))
                    _lookup.Add(res.type, res.icon);
            }
        }

        public Sprite GetIcon(CurrencyType type)
        {
            if (_lookup == null) Initialize();
            return _lookup.TryGetValue(type, out var icon) ? icon : null;
        }
    }
}