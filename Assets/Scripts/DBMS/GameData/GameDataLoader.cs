using System;
using System.IO;
using UnityEngine;
using Zenject;

namespace GameData
{
    public class GameDataLoader 
    {
        private GameDataScriptable _gameDataScriptable;
        public GameDataLoader(GameDataScriptable gameDataScriptable)
        {
            _gameDataScriptable = gameDataScriptable;
        }

        [Inject]
        public void LoadGameData()
        {
            Debug.Log("Game Data Loaded"); 
            // Assuming the JSON file is in the StreamingAssets folder.
            string path = Path.Combine(Application.streamingAssetsPath, "GameData.json");

            if (File.Exists(path))
            {
                // Read the JSON file content.
                string json = File.ReadAllText(path);

                // Deserialize the JSON into the GameData object.
                _gameDataScriptable.gameData = JsonUtility.FromJson<GameData>(json);
                Debug.Log("Game data loaded successfully.");
                

            }
            else
            {
                Debug.LogError("GameData.json file not found at " + path);
            }
        }
    }
}