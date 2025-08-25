using System;
using Content.Features.CurrencySystem;
using UnityEngine;

namespace Content.Features.BuildingSystem.Scripts
{
    [Serializable]
    public class ResourceAmount
    {
        public CurrencyType resourceType;
        public int amount;
    }
}