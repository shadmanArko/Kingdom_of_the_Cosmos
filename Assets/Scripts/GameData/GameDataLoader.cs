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

                // You can now access _gameData.champions and _gameData.weapons
                foreach (var champion in _gameDataScriptable.gameData.champions)
                {
                    Debug.Log($"Loaded Champion: {champion.name}, Tribe: {champion.tribe}");
                }

                foreach (var weapon in _gameDataScriptable.gameData.weapons)
                {
                    Debug.Log($"Loaded Weapon: {weapon.weaponName}, Damage: {weapon.damage}");
                }

            }
            else
            {
                Debug.LogError("GameData.json file not found at " + path);
            }
        }
    }
}