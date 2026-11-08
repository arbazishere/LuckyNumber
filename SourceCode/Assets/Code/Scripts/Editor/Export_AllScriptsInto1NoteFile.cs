using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

namespace ArbazAminKhan.Editor
{
    public class ExportAllScriptsToTxt
    {
        private const string ScriptsFolderPath = "Assets/Code/Scripts";
        private const string OutputFilePath = "Assets/Code/ScriptsTXT.txt";

        [MenuItem("Tools/Export All Scripts To TXT")]
        public static void ExportScripts()
        {
            if (!Directory.Exists(ScriptsFolderPath))
            {
                Debug.LogError("Scripts folder not found: " + ScriptsFolderPath);
                return;
            }

            string[] scriptFiles = Directory.GetFiles(
                ScriptsFolderPath,
                "*.cs",
                SearchOption.AllDirectories
            );

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("====================================");
            sb.AppendLine(" ALL UNITY SCRIPTS EXPORT ");
            sb.AppendLine(" Generated on: " + System.DateTime.Now);
            sb.AppendLine("====================================\n");

            foreach (string filePath in scriptFiles)
            {
                sb.AppendLine("====================================");
                sb.AppendLine(" FILE: " + filePath.Replace("\\", "/"));
                sb.AppendLine("====================================\n");

                try
                {
                    sb.AppendLine(File.ReadAllText(filePath));
                }
                catch (System.Exception e)
                {
                    sb.AppendLine("// ERROR READING FILE");
                    sb.AppendLine("// " + e.Message);
                }

                sb.AppendLine("\n\n");
            }

            File.WriteAllText(OutputFilePath, sb.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();

            Debug.Log($"Export complete! File created at: {OutputFilePath}");
        }
    }
}