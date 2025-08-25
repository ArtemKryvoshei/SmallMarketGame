using Core.IInitializeQueue;
using Core.Other;
using Core.ServiceLocatorSystem;
using UnityEngine;

namespace Core.SceneTransitionMediator
{
    public class SceneLoaderWithScreen : InitializeableMonoComponent
    {
        [SerializeField] private string _sceneName;
        private ISceneTransitionMediator _mediator;
        
        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_INGAME_COMPONENTS + 1;
            base.SetupPriority();
        }

        public override void Initialize()
        {
            _mediator = ServiceLocator.Get<ISceneTransitionMediator>();
            base.Initialize();
        }

        public void TryLoadScene()
        {
            _mediator.LoadSceneWithTransitionAsync(_sceneName);
        }
    }
}