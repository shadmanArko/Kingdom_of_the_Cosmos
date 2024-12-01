using Champions.Models;
using DBMS.SaveAndLoad;
using UnityEngine;

namespace TestScripts
{
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
}
