using UnityEditor;
using UnityEngine;
using System.IO;

public class SaveLoadWindow : EditorWindow
{
    private string _savePath;

    [MenuItem("Tools/Save System")]
    public static void OpenWindow()
    {
        var window = GetWindow<SaveLoadWindow>("Save System");
        window.Show();
    }

    private void OnEnable()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Save System Debug Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Save Path:", _savePath, EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Open Save Folder"))
        {
            EditorUtility.RevealInFinder(_savePath);
        }

        if (GUILayout.Button("Print Save To Console"))
        {
            if (File.Exists(_savePath))
            {
                string json = File.ReadAllText(_savePath);
                Debug.Log($"[SaveLoadWindow] Save content:\n{json}");
            }
            else
            {
                Debug.LogWarning("[SaveLoadWindow] Save file not found.");
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("⚠ This will permanently delete save.json, including settings!", MessageType.Warning);

        if (GUILayout.Button("Delete Save File"))
        {
            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
                Debug.Log("[SaveLoadWindow] Save file deleted!");
            }
            else
            {
                Debug.LogWarning("[SaveLoadWindow] No save file to delete.");
            }
        }
    }
}