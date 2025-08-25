using Core.JSONSaveLoadSystem;

namespace Content.Features.SettingsManager.Scripts
{
    public interface ISettingsManager
    {
        SettingsData Settings { get; }
        void ApplySettings(SettingsData newSettings);
    }
}