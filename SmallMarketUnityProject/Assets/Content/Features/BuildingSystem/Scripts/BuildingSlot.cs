using UnityEngine;

namespace Content.Features.BuildingSystem.Scripts
{
    public class BuildingSlot : MonoBehaviour
    {
        [SerializeField] private int id;
        [SerializeField] private Transform spawnPoint;

        public int Id => id;
        public int BuildingId { get; private set; } = -1;
        public int BuildTier { get; private set; } = 0;
        public bool IsEmpty => BuildingId < 0;
        public Transform SpawnPoint => spawnPoint != null ? spawnPoint : transform;

        private GameObject _spawned;

        public void AssignBuilding(int buildingId)
        {
            BuildingId = buildingId;
        }

        public void Clear()
        {
            BuildingId = -1;
            BuildTier = 0;
        }

        public void SetSpawned(GameObject go)
        {
            _spawned = go;
        }

        public void DestroySpawned()
        {
            if (_spawned != null)
            {
                GameObject.Destroy(_spawned);
                _spawned = null;
            }
        }
    }


}