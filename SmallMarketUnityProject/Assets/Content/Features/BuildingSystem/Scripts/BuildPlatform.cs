using UnityEngine;

namespace Content.Features.BuildingSystem.Scripts
{
    [RequireComponent(typeof(BoxCollider))]
    public class BuildPlatform : MonoBehaviour, IBuildPlatform
    {
        [SerializeField] private Transform menuAnchor;

        private BuildingSlot futureBuildingSlot;

        public void SetBuildingSlot(BuildingSlot _slot)
        {
            futureBuildingSlot = _slot;
        }

        public BuildingSlot GetBuildingSlot()
        {
            return futureBuildingSlot;
        }

        public Transform GetMenuAnchor()
        {
            return menuAnchor != null ? menuAnchor : transform;
        }
    }
}