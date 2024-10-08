using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace DBMS.JsonToScriptable
{
    public class JsonToScriptableReader
    {
        public static T LoadFromJson<T>(string filePath) where T : class
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"File not found: {filePath}");
                return null;
            }

            string jsonContent = File.ReadAllText(filePath);

            try
            {
                return JsonConvert.DeserializeObject<T>(jsonContent);
            }
            catch (JsonException e)
            {
                Debug.LogError($"Error parsing JSON: {e.Message}");
                return null;
            }
        }
        
        

        // public static ScriptableObject LoadIntoScriptableObject<T>(string filePath, ScriptableObject scriptableObject)
        //     where T : class
        // {
        //     T data = LoadFromJson<T>(filePath);
        //     if (data == null)
        //     {
        //         return null;
        //     }
        //
        //     var serializedObject = new SerializedObject(scriptableObject);
        //     var property = serializedObject.GetIterator();
        //
        //     while (property.NextVisible(true))
        //     {
        //         if (property.name == "m_Script") continue;
        //
        //         var field = typeof(T).GetField(property.name);
        //         if (field != null)
        //         {
        //             property.SetValue(field.GetValue(data));
        //         }
        //     }
        //
        //     serializedObject.ApplyModifiedProperties();
        //     return scriptableObject;
        // }

        public static void SaveToAsset(ScriptableObject scriptableObject, string assetPath)
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(scriptableObject, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log($"ScriptableObject saved to: {assetPath}");
#else
        Debug.LogWarning("Saving assets is only available in the Unity Editor.");
#endif
        }
    }
}