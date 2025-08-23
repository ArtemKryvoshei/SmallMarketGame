using System.Threading.Tasks;
using UnityEngine;

namespace Core.PrefabFactory
{
    public interface IPrefabFactory
    {
        Task<GameObject> SpawnAsync(string address, Vector3 position, Quaternion rotation, Transform parent = null);
        Task<T> SpawnAsync<T>(string address, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component;
    }
}