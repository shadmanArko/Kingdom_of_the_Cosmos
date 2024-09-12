using System.Collections;
using System.Collections.Generic;
using GameData;
using Models;
using SaveAndLoad;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var saveFile = new SaveData();
        saveFile.champion = new Champion()
        {
            name = "bla"
        };
        
        // var gameData =  GameDataLoader.Instance.LoadGameData();
        // Debug.Log(saveData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
