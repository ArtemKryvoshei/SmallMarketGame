
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor.AddressableAssets;

namespace Content.Global.Scripts.AddressablesTool
{
    public static class AddressablesCodeGenerator
    {
        private const string OUTPUT_PATH = "Assets/Content/Global/Scripts/AddressablesTool/Address.cs";
        private const string NAMESPACE = "AddressablesGenerated";

        [MenuItem("Tools/Addressables/Generate Address Constants")]
        public static void Generate()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("AddressableAssetSettings not found.");
                return;
            }

            var grouped = new Dictionary<string, List<(string key, string path)>>();

            foreach (var group in settings.groups)
            {
                if (group == null || group.ReadOnly) continue;

                string groupName = Sanitize(group.Name);

                if (!grouped.ContainsKey(groupName))
                    grouped[groupName] = new List<(string, string)>();

                foreach (var entry in group.entries)
                {
                    string entryName = Sanitize(Path.GetFileNameWithoutExtension(entry.address));
                    string address = entry.address;

                    if (!string.IsNullOrEmpty(entryName))
                    {
                        grouped[groupName].Add((entryName, address));
                    }
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine("// Auto-generated via AddressablesCodeGenerator");
            sb.AppendLine("namespace " + NAMESPACE);
            sb.AppendLine("{");
            sb.AppendLine("    public static class Address");
            sb.AppendLine("    {");

            foreach (var group in grouped)
            {
                sb.AppendLine($"        public static class {group.Key}");
                sb.AppendLine("        {");

                if (group.Value.Count == 0)
                {
                    sb.AppendLine("            // Empty group");
                }
                else
                {
                    foreach (var (entryName, address) in group.Value.Distinct())
                    {
                        sb.AppendLine($"            public const string {entryName} = \"{address}\";");
                    }
                }

                sb.AppendLine("        }");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            Directory.CreateDirectory(Path.GetDirectoryName(OUTPUT_PATH));
            File.WriteAllText(OUTPUT_PATH, sb.ToString());
            AssetDatabase.Refresh();

            Debug.Log($"✅ Address.cs сгенерирован: {OUTPUT_PATH}");
        }

        private static string Sanitize(string input)
        {
            var safe = new string(input
                .Where(c => char.IsLetterOrDigit(c) || c == '_')
                .ToArray());

            if (string.IsNullOrEmpty(safe)) return "_";

            if (char.IsDigit(safe[0]))
                safe = "_" + safe;

            return safe;
        }
    }
}
#endif