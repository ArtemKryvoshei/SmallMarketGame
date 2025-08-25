using System.IO;
using UnityEngine;

namespace Core.JSONSaveLoadSystem
{
    public class SaveLoadSystem : ISaveLoadSystem
    {
        private readonly string _savePath;

        public SaveLoadSystem()
        {
            _savePath = Path.Combine(Application.persistentDataPath, "save.json");
        }

        public void Save(DTOModels data)
        {
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(_savePath, json);
            Debug.Log($"[SaveSystem] Save complete: {_savePath}");
        }

        public DTOModels Load()
        {
            if (!File.Exists(_savePath))
            {
                Debug.LogWarning("[SaveSystem] Save not found, return empty save");
                return new DTOModels();
            }

            var json = File.ReadAllText(_savePath);
            return JsonUtility.FromJson<DTOModels>(json);
        }

        public void DeleteSave()
        {
            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
                Debug.Log("[SaveSystem] All saves have been deleted");
            }
        }
    }
}