using System.Collections.Generic;
using UnityEngine;

namespace Content.Features.BuildingSystem.Scripts
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "Game/Building")]
    public class BuildingData : ScriptableObject
    {
        public int id;
        public string buildingName;
        public string prefabAddress;

        public List<ResourceAmount> buildCosts; // цена постройки
        public SellableObjectData sellableObject;
    }
}