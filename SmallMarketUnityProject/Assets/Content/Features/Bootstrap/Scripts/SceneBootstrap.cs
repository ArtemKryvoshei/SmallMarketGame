using Core.IInitializeQueue;
using UnityEngine;

namespace Content.Features.Bootstrap.Scripts
{
    public class SceneBootstrap : MonoBehaviour
    {
        [SerializeField] private ComponentsInitializeManager componentsInitManager;
        
        private void Awake()
        {
            componentsInitManager.InitializeComponents();
        }
    }
}