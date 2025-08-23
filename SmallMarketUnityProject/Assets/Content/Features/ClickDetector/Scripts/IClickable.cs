using Core.EventBus;

namespace Content.Features.ClickDetector.Scripts
{
    public interface IClickable
    {
        void OnClicked();
        void OnClicked(IEventBus eventBus);
    }
}