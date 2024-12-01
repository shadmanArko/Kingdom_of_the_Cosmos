using UnityEngine;

namespace DBMS.GameData
{
    [CreateAssetMenu(fileName = "GameDataScriptable", menuName = "ScriptableObjects/GameDataScriptable", order = 0)]
    public class GameDataScriptable : ScriptableObject
    {
        public GameData gameData;
    }
}