using UnityEngine;

namespace Core.IInitializeQueue
{
    public abstract class InitializeableMonoComponent : MonoBehaviour, IInitializeableComponent
    {
        public int Priority => initPriority;
        protected int initPriority;
        protected bool initialized = false;
        
        public virtual void SetupPriority()
        {
            initialized = false;
        }
        
        public virtual void Initialize()
        {
            Debug.Log("[" + gameObject.name + "] Initializing...");
        }
    }
}