using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.PrefabFactory
{
    public class PrefabFactory : IPrefabFactory
    {
        private readonly Dictionary<string, GameObject> _cache = new();

        /// <summary>
        /// Загружает префаб (если ещё не загружен) и создаёт экземпляр.
        /// </summary>
        public async Task<GameObject> SpawnAsync(string address, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (!_cache.TryGetValue(address, out var prefab))
            {
                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);
                prefab = await handle.Task;
                _cache[address] = prefab;
            }

            var instance = Object.Instantiate(prefab, position, rotation, parent);
            return instance;
        }

        /// <summary>
        /// То же самое, но сразу возвращает компонент T.
        /// </summary>
        public async Task<T> SpawnAsync<T>(string address, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            var go = await SpawnAsync(address, position, rotation, parent);
            return go.GetComponent<T>();
        }
    }
}