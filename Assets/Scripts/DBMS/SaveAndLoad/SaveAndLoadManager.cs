using System.IO;
using UnityEngine;
using Zenject;

namespace DBMS.SaveAndLoad
{
    public  class SaveAndLoadManager
    {
        private SaveDataScriptable _saveDataScriptable;
        public SaveAndLoadManager(SaveDataScriptable saveDataScriptable)
        {
            _saveDataScriptable = saveDataScriptable;
            InitializeSavePath();
        }
        // Define a class to hold the save data.
        // The name of the folder where we will save our files.
        private string saveFolder = "SaveData";
        private string saveFileName = "Save.json";

        // The full path where the save file will be stored.
        private string saveFilePath;

        [Inject]
        void LoadData()
        {
            _saveDataScriptable.saveData = LoadDataFromFile();
        }

        private void InitializeSavePath()
        {
            // Combine the persistent data path with the save folder and file name.
            string folderPath = Path.Combine(Application.persistentDataPath, saveFolder);

            // Ensure the save folder exists; create it if not.
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            saveFilePath = Path.Combine(folderPath, saveFileName);
        }

        // Method to save the data.
        public void SaveDataToFile()
        {
            // Convert the object to a JSON string.
            string json = JsonUtility.ToJson(_saveDataScriptable.saveData);

            // Write the JSON string to a file.
            File.WriteAllText(saveFilePath, json);
            Debug.Log("Data saved to: " + saveFilePath);
        }

        // Method to load the data.
        public SaveData LoadDataFromFile()
        {
            // Check if the save file exists.
            if (File.Exists(saveFilePath))
            {
                // Read the file and convert the JSON string back into an object.
                string json = File.ReadAllText(saveFilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log("Data loaded from: " + saveFilePath);
                return data;
            }
            else
            {
                Debug.LogWarning("Save file not found. Returning default data.");
                return new SaveData(); // Return default data if no save file exists.
            }
        }
    }
}