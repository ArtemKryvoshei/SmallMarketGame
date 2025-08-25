using UnityEngine;
using System.IO;

public class SaveFileDeleter : MonoBehaviour
{
    private string _savePath;
        
    public void ClearSaveFile()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(_savePath))
        {
            File.Delete(_savePath);
            Debug.Log("[SaveFileDeleter] Save file deleted!");
        }
    }
}