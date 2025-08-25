using System.Collections.Generic;
using UnityEngine;

namespace Content.Features.BuildingSystem.Scripts
{
    [CreateAssetMenu(fileName = "SellableObjectData", menuName = "Game/Sellable Object")]
    public class SellableObjectData : ScriptableObject
    {
        public string objectName;
        public List<ResourceAmount> rewards; // награда в ресурсах
    }
}