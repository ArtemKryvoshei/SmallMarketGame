using Core.JSONSaveLoadSystem;

namespace Content.Features.SettingsManager.Scripts
{
    public class SettingsManager : ISettingsManager
    {
        private readonly ISaveLoadSystem _saveSystem;
        private DTOModels _saveData;

        public SettingsData Settings => _saveData.Settings;

        public SettingsManager(ISaveLoadSystem saveSystem)
        {
            _saveSystem = saveSystem;
            _saveData = _saveSystem.Load();
        }

        public void ApplySettings(SettingsData newSettings)
        {
            var dto = _saveSystem.Load() ?? new DTOModels();
            dto.Settings = newSettings;
            _saveSystem.Save(dto);
        }
    }
}