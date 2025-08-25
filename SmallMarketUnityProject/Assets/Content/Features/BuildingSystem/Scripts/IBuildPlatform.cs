using UnityEngine;

namespace Content.Features.BuildingSystem.Scripts
{
    public interface IBuildPlatform
    {
        public void SetBuildingSlot(BuildingSlot _slot);
        public BuildingSlot GetBuildingSlot();
        Transform GetMenuAnchor(); 
    }
}