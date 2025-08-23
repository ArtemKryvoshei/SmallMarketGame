namespace Core.IInitializeQueue
{
    public interface IInitializeableComponent
    {
        int Priority { get; }
        void Initialize();
        void SetupPriority();
    }
}