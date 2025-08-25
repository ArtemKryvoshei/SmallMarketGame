using System.Threading.Tasks;
using Core.PrefabFactory;
using UnityEngine;

namespace Core.LoadingScreenService
{
    public class LoadingScreenService : ILoadingScreenService
    {
        private readonly IPrefabFactory _factory;
        private GameObject _loadingScreenInstance;

        public LoadingScreenService(IPrefabFactory factory)
        {
            _factory = factory;
        }

        public async Task InitializeAsync(string address)
        {
            if (_loadingScreenInstance == null)
            {
                _loadingScreenInstance = await _factory.SpawnAsync(address, Vector3.zero, Quaternion.identity);
                Object.DontDestroyOnLoad(_loadingScreenInstance);
                _loadingScreenInstance.SetActive(false);
            }
        }

        public void Show()
        {
            if (_loadingScreenInstance != null)
                _loadingScreenInstance.SetActive(true);
        }

        public void Hide()
        {
            if (_loadingScreenInstance != null)
                _loadingScreenInstance.SetActive(false);
        }
    }
}